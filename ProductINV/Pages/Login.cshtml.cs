using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductINV.Models;
   // ✅ REQUIRED for AppDbContext
using BCryptNet = BCrypt.Net.BCrypt;

// ===============================================
// ADMIN MODEL (kept exactly as you want it)
// ===============================================
namespace ProductINV.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
    }
}

// ===============================================
// LOGIN MODEL
// ===============================================
namespace ProductINV.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // BIND PROPERTIES
        // ============================
        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        [BindProperty] public string Username { get; set; } = "";
        [BindProperty] public string AdminKey { get; set; } = "";
        [BindProperty] public string VerificationCode { get; set; } = "";
        [BindProperty] public string FullName { get; set; } = "";
        [BindProperty] public string Position { get; set; } = "";
        [BindProperty] public string IdNumber { get; set; } = "";
        [BindProperty] public string MasterKey { get; set; } = "";
        [BindProperty] public string NewAdminKey { get; set; } = "";

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

        private const string MASTER_ADMIN_KEY = "MASTER2024";

        public IActionResult OnGet()
        {
            var sessionUsername = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(sessionUsername))
                return RedirectBasedOnRole(HttpContext.Session.GetString("UserRole"));

            return Page();
        }

        // =====================================================
        // USER LOGIN → SEND OTP EMAIL
        // =====================================================
        public async Task<IActionResult> OnPostUserLogin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password))
                    return JsonError("Email/Username and password are required.");

                var identifier = Email.Trim();

                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.IsActive &&
                    (u.Email == identifier || u.Username == identifier));

                if (user == null)
                    return JsonError("Account not found.");

                if (!BCryptNet.Verify(Password, user.PasswordHash))
                    return JsonError("Invalid email/username or password.");

                // Generate OTP
                var otp = GenerateOTP();

                HttpContext.Session.SetInt32("PendingUserId", user.Id);
                HttpContext.Session.SetString("PendingUserRole", user.Role);
                HttpContext.Session.SetString("VerificationCode", otp);

                // Send email
                await SendVerificationEmailAsync(user.Email, otp);

                return JsonSuccess("Verification code sent to your email.");
            }
            catch (Exception ex)
            {
                return JsonError("Login failed: " + ex.Message);
            }
        }

        // =====================================================
        // ADMIN LOGIN (BCrypt + Plain Password + Plain AdminKey)
        // =====================================================
        public async Task<IActionResult> OnPostAdminLogin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(AdminKey))
                {
                    return JsonError("All admin login fields are required.");
                }

                var adminUser = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Username == Username);

                if (adminUser == null)
                    return JsonError("Admin account not found.");

                bool passwordMatch = false;

                try { passwordMatch = BCryptNet.Verify(Password, adminUser.PasswordHash); }
                catch { passwordMatch = false; }

                if (!passwordMatch)
                    passwordMatch = (Password == adminUser.PasswordHash);

                if (!passwordMatch)
                    return JsonError("Invalid admin username or password.");

                var keyList = await _context.AdminKeys
                    .Where(k => k.is_active == true)
                    .ToListAsync();

                bool keyMatch = false;

                foreach (var key in keyList)
                {
                    if (AdminKey == key.key_hash)
                    {
                        keyMatch = true;
                        break;
                    }
                }

                if (!keyMatch)
                    return JsonError("Invalid admin key.");

                HttpContext.Session.SetString("Username", adminUser.Username);
                HttpContext.Session.SetString("UserRole", "Admin");

                return new JsonResult(new
                {
                    success = true,
                    redirect = "/ManageProduct"
                });
            }
            catch (Exception ex)
            {
                return JsonError("Admin login failed: " + ex.Message);
            }
        }

        // =====================================================
        // VERIFY OTP
        // =====================================================
        public async Task<IActionResult> OnPostVerifyEmail()
        {
            try
            {
                var sessionOtp = HttpContext.Session.GetString("VerificationCode");
                var pendingId = HttpContext.Session.GetInt32("PendingUserId");

                if (pendingId == null || sessionOtp != VerificationCode)
                    return JsonError("Invalid or expired verification code.");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == pendingId);
                if (user == null)
                    return JsonError("User not found.");

                user.IsEmailVerified = true;
                user.LastLogin = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);

                HttpContext.Session.Remove("VerificationCode");
                HttpContext.Session.Remove("PendingUserId");
                HttpContext.Session.Remove("PendingUserRole");

                return new JsonResult(new
                {
                    success = true,
                    message = "Email verified successfully!",
                    redirect = user.Role == "User" ? "/UserView" : "/ManageProduct"
                });
            }
            catch (Exception ex)
            {
                return JsonError("Verification failed: " + ex.Message);
            }
        }

        // =====================================================
        // SEND EMAIL (OTP)
        // =====================================================
        private async Task SendVerificationEmailAsync(string toEmail, string otp)
        {
            try
            {
                string subject = "Your CWTP Login Verification Code";
                string body = $@"
                <html>
                <body style='font-family: Arial;'>
                    <h2>CWTP Asset Inventory System</h2>
                    <p>Your verification code is:</p>
                    <h1 style='font-size:32px;letter-spacing:5px;'>{otp}</h1>
                    <p>This code expires in 10 minutes.</p>
                </body>
                </html>";

                var mail = new MailMessage();
                mail.From = new MailAddress("johnlee.lugnasin@student.pnm.edu.ph", "CWTP Inventory");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("johnlee.lugnasin@student.pnm.edu.ph", "przo aaig mpxx ktvc"),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("SMTP ERROR: " + ex.Message);
            }
        }

        // =====================================================
        // LOGOUT
        // =====================================================
        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        // =====================================================
        // HELPERS
        // =====================================================
        private string GenerateOTP()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        private JsonResult JsonError(string message)
        {
            ErrorMessage = message;
            return new JsonResult(new { success = false, message });
        }

        private JsonResult JsonSuccess(string message)
        {
            SuccessMessage = message;
            return new JsonResult(new { success = true, message });
        }

        private IActionResult RedirectBasedOnRole(string? role)
        {
            return role switch
            {
                "Admin" => RedirectToPage("/ManageProduct"),
                "User" => RedirectToPage("/UserView"),
                _ => RedirectToPage("/Index"),
            };
        }
    }
}
