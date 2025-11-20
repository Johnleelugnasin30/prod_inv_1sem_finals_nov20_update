using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductINV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductINV.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly AppDbContext _db;

        public AdminDashboardModel(AppDbContext db)
        {
            _db = db;
        }

        // Products
        public List<Product> Products { get; set; } = new List<Product>();
        [BindProperty] public Product NewProduct { get; set; } = new Product();
        [BindProperty] public Product EditProduct { get; set; } = new Product();
        [BindProperty(SupportsGet = true)] public int? EditProductId { get; set; }

        // Users (Manage Account CRUD)
        public List<User> Users { get; set; } = new List<User>();
        [BindProperty] public User NewUser { get; set; } = new User();
        [BindProperty] public User EditUser { get; set; } = new User();
        [BindProperty(SupportsGet = true)] public int? EditUserId { get; set; }
        [BindProperty(SupportsGet = true)] public int? DeleteUserId { get; set; }

        // Admins (Manage Admin Account CRUD)
        public List<Admin> Admins { get; set; } = new List<Admin>();
        [BindProperty] public Admin NewAdmin { get; set; } = new Admin();
        [BindProperty] public Admin EditAdmin { get; set; } = new Admin();
        [BindProperty] public string NewAdminPassword { get; set; } = "";
        [BindProperty] public string EditAdminPassword { get; set; } = "";
        [BindProperty(SupportsGet = true)] public int? EditAdminId { get; set; }
        [BindProperty(SupportsGet = true)] public int? DeleteAdminId { get; set; }

        // Reports
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBorrowRequests { get; set; }
        public List<BorrowRequest> BorrowRequests { get; set; } = new List<BorrowRequest>();

        // Login History
        public List<AuditLog> LoginHistory { get; set; } = new List<AuditLog>();

        // QR Generation
        [BindProperty(SupportsGet = true)] public string QRProductId { get; set; } = "";
        public string QRCodeUrl { get; set; } = "";

        public string Username { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

        public async Task OnGet()
        {
            var sessionUsername = HttpContext.Session.GetString("Username");
            var sessionRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(sessionUsername) || sessionRole != "Admin")
            {
                Response.Redirect("/Login");
                return;
            }

            Username = sessionUsername;
            await LoadAllData();
        }

        // ========== PRODUCT MANAGEMENT ==========
        public async Task<IActionResult> OnPostCreateProduct()
        {
            try
            {
                NewProduct.CreatedAt = DateTime.UtcNow;
                NewProduct.UpdatedAt = DateTime.UtcNow;
                _db.Products.Add(NewProduct);
                await _db.SaveChangesAsync();
                SuccessMessage = "Product created successfully!";
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to create product: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateProduct()
        {
            try
            {
                var product = await _db.Products.FindAsync(EditProduct.Id);
                if (product != null)
                {
                    product.Name = EditProduct.Name;
                    product.Category = EditProduct.Category;
                    product.Location = EditProduct.Location;
                    product.Price = EditProduct.Price;
                    product.Quantity = EditProduct.Quantity;
                    product.IsAvailable = EditProduct.IsAvailable;
                    product.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                    SuccessMessage = "Product updated successfully!";
                }
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to update product: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteProduct(int id)
        {
            try
            {
                var product = await _db.Products.FindAsync(id);
                if (product != null)
                {
                    _db.Products.Remove(product);
                    await _db.SaveChangesAsync();
                    SuccessMessage = "Product deleted successfully!";
                }
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to delete product: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        // ========== USER MANAGEMENT (CRUD) ==========
        public async Task<IActionResult> OnPostCreateUser()
        {
            try
            {
                if (await _db.Users.AnyAsync(u => u.Username == NewUser.Username || u.Email == NewUser.Email))
                {
                    ErrorMessage = "Username or Email already exists.";
                    await LoadAllData();
                    return Page();
                }

                NewUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewUser.PasswordHash);
                NewUser.CreatedAt = DateTime.UtcNow;
                NewUser.UpdatedAt = DateTime.UtcNow;
                _db.Users.Add(NewUser);
                await _db.SaveChangesAsync();
                SuccessMessage = "User created successfully!";
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to create user: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateUser()
        {
            try
            {
                var user = await _db.Users.FindAsync(EditUser.Id);
                if (user != null)
                {
                    user.FullName = EditUser.FullName;
                    user.Email = EditUser.Email;
                    user.Username = EditUser.Username;
                    user.Position = EditUser.Position;
                    user.Role = EditUser.Role;
                    user.IsActive = EditUser.IsActive;
                    user.IsEmailVerified = EditUser.IsEmailVerified;
                    if (!string.IsNullOrEmpty(EditUser.PasswordHash))
                    {
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(EditUser.PasswordHash);
                    }
                    user.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                    SuccessMessage = "User updated successfully!";
                }
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to update user: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteUser(int id)
        {
            try
            {
                var user = await _db.Users.FindAsync(id);
                if (user != null)
                {
                    _db.Users.Remove(user);
                    await _db.SaveChangesAsync();
                    SuccessMessage = "User deleted successfully!";
                }
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to delete user: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        // ========== QR GENERATION ==========
        public async Task<IActionResult> OnGetGenerateQR(string productId)
        {
            QRProductId = productId;
            GenerateQR();
            await LoadAllData();
            return Page();
        }

        // ========== BORROW REQUEST APPROVAL ==========
        public async Task<IActionResult> OnPostApproveBorrowRequest(int id)
        {
            try
            {
                var request = await _db.BorrowRequests.FindAsync(id);
                if (request != null)
                {
                    request.Status = "Approved";
                    request.BorrowDate = DateTime.UtcNow;
                    request.UpdatedAt = DateTime.UtcNow;
                    
                    var product = await _db.Products.FindAsync(request.ProductId);
                    if (product != null)
                    {
                        product.IsAvailable = false;
                    }
                    
                    await _db.SaveChangesAsync();
                    SuccessMessage = "Borrow request approved!";
                }
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to approve request: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRejectBorrowRequest(int id)
        {
            try
            {
                var request = await _db.BorrowRequests.FindAsync(id);
                if (request != null)
                {
                    request.Status = "Rejected";
                    request.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                    SuccessMessage = "Borrow request rejected.";
                }
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to reject request: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        // ========== ADMIN ACCOUNT MANAGEMENT (CRUD) ==========
        public async Task<IActionResult> OnPostCreateAdmin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewAdmin.Username) || string.IsNullOrWhiteSpace(NewAdminPassword))
                {
                    ErrorMessage = "Username and password are required.";
                    await LoadAllData();
                    return Page();
                }

                if (await _db.Admins.AnyAsync(a => a.Username == NewAdmin.Username))
                {
                    ErrorMessage = "Admin username already exists.";
                    await LoadAllData();
                    return Page();
                }

                var admin = new Admin
                {
                    Username = NewAdmin.Username.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewAdminPassword)
                };

                _db.Admins.Add(admin);
                await _db.SaveChangesAsync();
                SuccessMessage = $"Admin account '{admin.Username}' created successfully!";
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to create admin account: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateAdmin()
        {
            try
            {
                var admin = await _db.Admins.FindAsync(EditAdmin.Id);
                if (admin != null)
                {
                    admin.Username = EditAdmin.Username.Trim();
                    
                    // Only update password if provided
                    if (!string.IsNullOrWhiteSpace(EditAdminPassword))
                    {
                        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(EditAdminPassword);
                    }
                    
                    await _db.SaveChangesAsync();
                    SuccessMessage = "Admin account updated successfully!";
                }
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to update admin account: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAdmin(int id)
        {
            try
            {
                var admin = await _db.Admins.FindAsync(id);
                if (admin != null)
                {
                    _db.Admins.Remove(admin);
                    await _db.SaveChangesAsync();
                    SuccessMessage = "Admin account deleted successfully!";
                }
                await LoadAllData();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to delete admin account: " + ex.Message;
                await LoadAllData();
                return Page();
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        private async Task LoadAllData()
        {
            Products = await _db.Products.OrderBy(p => p.Name).ToListAsync();
            Users = await _db.Users.OrderBy(u => u.Username).ToListAsync();
            Admins = await _db.Admins.OrderBy(a => a.Username).ToListAsync();
            BorrowRequests = await _db.BorrowRequests
                .OrderByDescending(b => b.RequestDate)
                .ToListAsync();
            LoginHistory = await _db.AuditLogs
                .Where(a => a.Action == "Login")
                .OrderByDescending(a => a.CreatedAt)
                .Take(50)
                .ToListAsync();

            TotalProducts = Products.Count;
            TotalUsers = Users.Count;
            TotalBorrowRequests = BorrowRequests.Count;

            if (EditProductId.HasValue)
            {
                EditProduct = await _db.Products.FindAsync(EditProductId.Value);
            }

            if (EditUserId.HasValue)
            {
                EditUser = await _db.Users.FindAsync(EditUserId.Value);
            }

            if (EditAdminId.HasValue)
            {
                EditAdmin = await _db.Admins.FindAsync(EditAdminId.Value);
            }
        }

        private void GenerateQR()
        {
            if (!string.IsNullOrEmpty(QRProductId))
            {
                QRCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={QRProductId}";
            }
        }
    }
}

