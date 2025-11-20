using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductINV.Models;
using ProductINV.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductINV.Pages
{
    public class UserViewModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly EmailService _emailService;

        public UserViewModel(AppDbContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        // Product list
        public List<Product> Products { get; set; } = new List<Product>();

        // Product Info
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = "";
        public Product SelectedProduct { get; set; }

        // QR Code
        [BindProperty(SupportsGet = true)]
        public string QRProductId { get; set; } = "";
        public string QRCodeUrl { get; set; } = "";

        // User Info
        public User CurrentUser { get; set; }
        public string Username { get; set; } = "";

        // Login History
        public List<AuditLog> LoginHistory { get; set; } = new List<AuditLog>();

        // Borrow Requests
        public List<BorrowRequest> BorrowRequests { get; set; } = new List<BorrowRequest>();
        [BindProperty] public BorrowRequest NewBorrowRequest { get; set; } = new BorrowRequest();

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

        public async Task OnGet()
        {
            var sessionUsername = HttpContext.Session.GetString("Username");
            var sessionRole = HttpContext.Session.GetString("UserRole");
            
            if (string.IsNullOrEmpty(sessionUsername))
            {
                Response.Redirect("/Login");
                return;
            }

            // Redirect admin users to AdminDashboard
            if (sessionRole == "Admin")
            {
                Response.Redirect("/AdminDashboard");
                return;
            }

            Username = sessionUsername;
            CurrentUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == Username);
            
            // Ensure user exists and is not an admin
            if (CurrentUser == null || CurrentUser.Role == "Admin")
            {
                Response.Redirect("/Login");
                return;
            }

            await LoadProducts();
            await LoadLoginHistory();
            await LoadBorrowRequests();

            if (!string.IsNullOrEmpty(SearchTerm))
                await SearchProduct();

            if (!string.IsNullOrEmpty(QRProductId))
                GenerateQR();
        }

        public async Task<IActionResult> OnGetSearchProduct(string searchTerm)
        {
            SearchTerm = searchTerm;
            await LoadProducts();
            await SearchProduct();
            return Page();
        }

        public async Task<IActionResult> OnGetGenerateQR(string productId)
        {
            QRProductId = productId;
            await LoadProducts();
            GenerateQR();
            return Page();
        }

        public async Task<IActionResult> OnPostBorrowRequest()
        {
            try
            {
                var sessionUsername = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(sessionUsername))
                    return RedirectToPage("/Login");

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == sessionUsername);
                if (user == null)
                {
                    ErrorMessage = "User not found.";
                    await LoadProducts();
                    await LoadBorrowRequests();
                    return Page();
                }

                var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == NewBorrowRequest.ProductId);
                if (product == null)
                {
                    ErrorMessage = "Product not found.";
                    await LoadProducts();
                    await LoadBorrowRequests();
                    return Page();
                }

                if (!product.IsAvailable)
                {
                    ErrorMessage = "This product is currently not available.";
                    await LoadProducts();
                    await LoadBorrowRequests();
                    return Page();
                }

                var borrowRequest = new BorrowRequest
                {
                    UserId = user.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Purpose = NewBorrowRequest.Purpose,
                    RequestDate = DateTime.UtcNow,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.BorrowRequests.Add(borrowRequest);
                await _db.SaveChangesAsync();

                // Send email to admin
                await SendBorrowRequestEmailToAdmin(user, product, borrowRequest);

                SuccessMessage = "Borrow request submitted successfully! Admin will be notified.";
                await LoadProducts();
                await LoadBorrowRequests();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to submit borrow request: " + ex.Message;
                await LoadProducts();
                await LoadBorrowRequests();
                return Page();
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        private async Task LoadProducts()
        {
            Products = await _db.Products
                .Where(p => p.IsAvailable)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        private async Task SearchProduct()
        {
            if (string.IsNullOrEmpty(SearchTerm))
                return;

            SelectedProduct = await _db.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId.Contains(SearchTerm) ||
                    p.Name.Contains(SearchTerm) ||
                    (p.Category != null && p.Category.Contains(SearchTerm)));
        }

        private void GenerateQR()
        {
            if (!string.IsNullOrEmpty(QRProductId))
        {
            QRCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={QRProductId}";
        }
    }

        private async Task LoadLoginHistory()
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == Username);
            if (user != null)
            {
                LoginHistory = await _db.AuditLogs
                    .Where(a => a.UserId == user.Id && a.Action == "Login")
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(20)
                    .ToListAsync();
            }
        }

        private async Task LoadBorrowRequests()
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == Username);
            if (user != null)
            {
                BorrowRequests = await _db.BorrowRequests
                    .Where(b => b.UserId == user.Id)
                    .OrderByDescending(b => b.RequestDate)
                    .ToListAsync();
            }
        }

        private async Task SendBorrowRequestEmailToAdmin(User user, Product product, BorrowRequest request)
        {
            try
            {
                // Get admin emails
                var admins = await _db.Users
                    .Where(u => u.Role == "Admin" && u.IsActive)
                    .Select(u => u.Email)
                    .ToListAsync();

                if (admins.Any())
                {
                    string subject = $"New Borrow Request - {product.Name}";
                    string body = $@"
                        <html>
                        <body style='font-family: Arial;'>
                            <h2>New Borrow Request</h2>
                            <p><strong>User:</strong> {user.FullName} ({user.Username})</p>
                            <p><strong>Product:</strong> {product.Name} (ID: {product.ProductId})</p>
                            <p><strong>Purpose:</strong> {request.Purpose}</p>
                            <p><strong>Request Date:</strong> {request.RequestDate:yyyy-MM-dd HH:mm}</p>
                            <p>Please review and approve/reject this request in the admin dashboard.</p>
                        </body>
                        </html>";

                    var mail = new System.Net.Mail.MailMessage();
                    mail.From = new System.Net.Mail.MailAddress("johnlee.lugnasin@student.pnm.edu.ph", "CWTP Inventory");
                    foreach (var adminEmail in admins)
                    {
                        mail.To.Add(adminEmail);
                    }
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new System.Net.NetworkCredential("johnlee.lugnasin@student.pnm.edu.ph", "przo aaig mpxx ktvc"),
                        EnableSsl = true
                    };

                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request
                System.Diagnostics.Debug.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
