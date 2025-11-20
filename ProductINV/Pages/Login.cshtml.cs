using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Antiforgery;
using ProductINV.Models;
   // ✅ REQUIRED for AppDbContext
using BCryptNet = BCrypt.Net.BCrypt;

// ===============================================
// LOGIN MODEL
// ===============================================
namespace ProductINV.Pages
{
    [IgnoreAntiforgeryToken]
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

        public async Task<IActionResult> OnGet()
        {
            var sessionUsername = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(sessionUsername))
                return RedirectBasedOnRole(HttpContext.Session.GetString("UserRole"));

            // Test database connection
            try
            {
                await _context.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Database connection error: {ex.GetBaseException().Message}. Please check your database connection.";
            }

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

                // Send email (don't fail login if email fails)
                try
                {
                    await SendVerificationEmailAsync(user.Email, otp);
                    return JsonSuccess("Verification code sent to your email.");
                }
                catch (Exception emailEx)
                {
                    // Log email error but don't fail login
                    System.Diagnostics.Debug.WriteLine($"Email sending failed: {emailEx.Message}");
                    return JsonSuccess($"Verification code: {otp} (Email sending failed, but you can still verify)");
                }
            }
            catch (Exception ex)
            {
                // Log full error details
                System.Diagnostics.Debug.WriteLine($"User Login Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return JsonError("Login failed: " + ex.GetBaseException().Message);
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
                    redirect = "/AdminDashboard"
                });
            }
            catch (Exception ex)
            {
                // Log full error details
                System.Diagnostics.Debug.WriteLine($"Admin Login Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return JsonError("Admin login failed: " + ex.GetBaseException().Message);
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

                // Debug logging
                System.Diagnostics.Debug.WriteLine($"Session OTP: {sessionOtp}");
                System.Diagnostics.Debug.WriteLine($"Submitted Code: {VerificationCode}");
                System.Diagnostics.Debug.WriteLine($"Pending User ID: {pendingId}");

                if (pendingId == null)
                    return JsonError("Session expired. Please login again.");

                if (string.IsNullOrWhiteSpace(sessionOtp))
                    return JsonError("Verification code expired. Please login again.");

                if (string.IsNullOrWhiteSpace(VerificationCode))
                    return JsonError("Please enter the verification code.");

                // Compare codes (case-insensitive, trimmed)
                var sessionCode = sessionOtp?.Trim();
                var submittedCode = VerificationCode?.Trim();

                if (sessionCode != submittedCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Code mismatch: Session='{sessionCode}' vs Submitted='{submittedCode}'");
                    return JsonError("Invalid verification code. Please check and try again.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == pendingId);
                if (user == null)
                    return JsonError("User not found.");

                user.IsEmailVerified = true;
                user.LastLogin = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Set session BEFORE removing verification data
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);

                // Clean up verification session data
                HttpContext.Session.Remove("VerificationCode");
                HttpContext.Session.Remove("PendingUserId");
                HttpContext.Session.Remove("PendingUserRole");

                // Debug logging
                System.Diagnostics.Debug.WriteLine($"Session set - Username: {user.Username}, Role: {user.Role}");
                
                // Determine redirect URL based on user role
                string redirectUrl = user.Role == "User" ? "/UserView" : "/AdminDashboard";
                System.Diagnostics.Debug.WriteLine($"Redirecting to: {redirectUrl}");

                return new JsonResult(new
                {
                    success = true,
                    message = "Email verified successfully! Redirecting...",
                    redirect = redirectUrl
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
        // CREATE ADMIN KEY
        // =====================================================
        public async Task<IActionResult> OnPostCreateAdminKey()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MasterKey) || string.IsNullOrWhiteSpace(NewAdminKey))
                    return JsonError("Master key and new admin key are required.");

                // Verify master key
                if (MasterKey.Trim() != MASTER_ADMIN_KEY)
                    return JsonError("Invalid master key.");

                var newKeyValue = NewAdminKey.Trim();
                
                // Check if admin key already exists
                var existingKey = await _context.AdminKeys
                    .FirstOrDefaultAsync(k => k.key_hash == newKeyValue && k.is_active);

                if (existingKey != null)
                    return JsonError("Admin key already exists.");

                // Create new admin key
                var adminKey = new AdminKey
                {
                    key_hash = newKeyValue,
                    is_active = true,
                    created_at = DateTime.UtcNow
                };

                // Log before save
                System.Diagnostics.Debug.WriteLine($"Attempting to save admin key: {newKeyValue}");
                System.Diagnostics.Debug.WriteLine($"AdminKey entity before Add: Id={adminKey.Id}, key_hash={adminKey.key_hash}, is_active={adminKey.is_active}, created_at={adminKey.created_at}");
                
                _context.AdminKeys.Add(adminKey);
                
                // Log after Add
                System.Diagnostics.Debug.WriteLine($"AdminKey entity after Add: Id={adminKey.Id}, key_hash={adminKey.key_hash}");
                
                try
                {
                    var result = await _context.SaveChangesAsync();
                    
                    // Log after save
                    System.Diagnostics.Debug.WriteLine($"SaveChangesAsync result: {result} rows affected");
                    System.Diagnostics.Debug.WriteLine($"AdminKey entity after save: Id={adminKey.Id}, key_hash={adminKey.key_hash}");

                    // Verify it was saved
                    var verifyKey = await _context.AdminKeys
                        .AsNoTracking()
                        .FirstOrDefaultAsync(k => k.key_hash == newKeyValue);
                    
                    if (verifyKey == null)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: Admin key was not found after save!");
                        return JsonError("Admin key was created but could not be verified. Please check the database connection.");
                    }

                    return JsonSuccess($"Admin key '{newKeyValue}' created successfully! (ID: {verifyKey.Id})");
                }
                catch (Exception saveEx)
                {
                    System.Diagnostics.Debug.WriteLine($"SaveChangesAsync exception: {saveEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {saveEx.InnerException?.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception creating admin key: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return JsonError("Failed to create admin key: " + ex.GetBaseException().Message);
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
            var result = new JsonResult(new { success = false, message });
            result.ContentType = "application/json";
            return result;
        }

        private JsonResult JsonSuccess(string message)
        {
            SuccessMessage = message;
            var result = new JsonResult(new { success = true, message });
            result.ContentType = "application/json";
            return result;
        }

        private IActionResult RedirectBasedOnRole(string? role)
        {
            return role switch
            {
                "Admin" => RedirectToPage("/AdminDashboard"),
                "User" => RedirectToPage("/UserView"),
                _ => RedirectToPage("/Index"),
            };
        }
    }
}
