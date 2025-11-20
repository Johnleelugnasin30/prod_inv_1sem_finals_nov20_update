using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ProductINV.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductINV.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _db;

        public RegisterModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty] public string FullName { get; set; } = "";
        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Username { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        [BindProperty] public string? Position { get; set; }
        [BindProperty] public string? IdNumber { get; set; }
        [BindProperty] public bool TermsAccepted { get; set; }

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

        // ===================================
        // SHOW ID NUMBER IN TEXTBOX ON PAGE LOAD
        // ===================================
        public async Task OnGet()
        {
            IdNumber = await GenerateNextIdNumber();
        }

        // ===================================
        // CREATE ACCOUNT (called by handler)
        // ===================================
        public async Task<IActionResult> OnPostCreateAccount()
        {
            try
            {
                // Regenerate ID to ensure it's correct
                IdNumber = await GenerateNextIdNumber();

                if (string.IsNullOrWhiteSpace(FullName) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please fill in all required fields.";
                    return Page();
                }

                if (!TermsAccepted)
                {
                    ErrorMessage = "You must accept the Terms and Conditions to create an account.";
                    return Page();
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password.Trim());
                string position = Position?.Trim() ?? "";

                // Debug logging
                System.Diagnostics.Debug.WriteLine($"=== REGISTRATION DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Position received: '{position}'");
                System.Diagnostics.Debug.WriteLine($"Position length: {position.Length}");
                System.Diagnostics.Debug.WriteLine($"FullName: {FullName}, Username: {Username}, Email: {Email}");

                // Validate position
                if (string.IsNullOrWhiteSpace(position))
                {
                    ErrorMessage = "Please select a position.";
                    return Page();
                }

                // Normalize position for comparison (trim and case-insensitive)
                string normalizedPosition = position.Trim();

                // Check if position is Admin Finance or ICT Head - save to admin table (case-insensitive)
                bool isAdminFinance = normalizedPosition.Equals("Admin Finance", StringComparison.OrdinalIgnoreCase);
                bool isIctHead = normalizedPosition.Equals("ICT Head", StringComparison.OrdinalIgnoreCase);
                bool isAdminPosition = isAdminFinance || isIctHead;
                
                System.Diagnostics.Debug.WriteLine($"Position check - Admin Finance: {isAdminFinance}, ICT Head: {isIctHead}, Is Admin: {isAdminPosition}");

                // CRITICAL: Save Admin Finance and ICT Head to ADMIN table ONLY
                if (isAdminPosition)
                {
                    System.Diagnostics.Debug.WriteLine("*** SAVING TO ADMIN TABLE ***");
                    
                    // Check for duplicate username in admin table
                    if (await _db.Admins.AnyAsync(a => a.Username == Username.Trim()))
                    {
                        ErrorMessage = "Username already exists in admin accounts.";
                        return Page();
                    }

                    // Check for duplicate username in users table
                    if (await _db.Users.AnyAsync(u => u.Username == Username.Trim()))
                    {
                        ErrorMessage = "Username already exists in user accounts.";
                        return Page();
                    }

                    // Create admin account - ONLY save to admin table
                    var admin = new Admin
                    {
                        Username = Username.Trim(),
                        PasswordHash = hashedPassword
                    };

                    _db.Admins.Add(admin);
                    var saveResult = await _db.SaveChangesAsync();

                    System.Diagnostics.Debug.WriteLine($"*** ADMIN ACCOUNT SAVED TO ADMIN TABLE! Rows affected: {saveResult}, Admin ID: {admin.Id} ***");
                    SuccessMessage = $"Admin account '{admin.Username}' created successfully! You can now login using Admin Login tab.";
                    return Page(); // IMPORTANT: Return here, do NOT continue to user table
                }
                // If position is Missionary - save to users table (case-insensitive)
                else if (normalizedPosition.Equals("Missionary", StringComparison.OrdinalIgnoreCase))
                {
                    // SAFETY CHECK: Make absolutely sure this is NOT an admin position
                    if (isAdminPosition)
                    {
                        System.Diagnostics.Debug.WriteLine($"*** ERROR: Attempted to save admin position '{position}' to users table! Blocked! ***");
                        ErrorMessage = "System error: Invalid position routing. Please contact administrator.";
                        return Page();
                    }

                    System.Diagnostics.Debug.WriteLine("Saving to USERS table...");
                    // Check for duplicate email in users table
                    if (await _db.Users.AnyAsync(u => u.Email == Email.Trim()))
                    {
                        ErrorMessage = "Email already exists.";
                        return Page();
                    }

                    // Check for duplicate username in users table
                    if (await _db.Users.AnyAsync(u => u.Username == Username.Trim()))
                    {
                        ErrorMessage = "Username already exists.";
                        return Page();
                    }

                    // Check for duplicate username in admin table
                    if (await _db.Admins.AnyAsync(a => a.Username == Username.Trim()))
                    {
                        ErrorMessage = "Username already exists.";
                        return Page();
                    }

                    // Create user account - ONLY for Missionary position
                    var user = new User
                    {
                        FullName = FullName.Trim(),
                        Email = Email.Trim(),
                        Username = Username.Trim(),
                        PasswordHash = hashedPassword,
                        Position = position,
                        IdNumber = IdNumber,     // Auto-generated ID
                        Role = "User",
                        IsActive = true,
                        IsEmailVerified = false,
                        IsIdVerified = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _db.Users.Add(user);
                    var userSaveResult = await _db.SaveChangesAsync();

                    System.Diagnostics.Debug.WriteLine($"User account saved! Rows affected: {userSaveResult}, User ID: {user.Id}");
                    SuccessMessage = "Account created successfully! Redirecting to login...";
                    return Page();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Invalid position selected: '{position}'");
                    ErrorMessage = $"Invalid position selected: '{position}'. Please select Missionary, Admin Finance, or ICT Head.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** EXCEPTION IN REGISTRATION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"*** STACK TRACE: {ex.StackTrace}");
                ErrorMessage = "Registration failed: " + ex.GetBaseException().Message;
                return Page();
            }
        }

        // ===================================
        // GENERATE NEXT AUTO ID
        // ===================================
        private async Task<string> GenerateNextIdNumber()
        {
            var lastUser = await _db.Users
                .OrderByDescending(u => u.Id)
                .FirstOrDefaultAsync();

            if (lastUser == null || string.IsNullOrEmpty(lastUser.IdNumber))
                return "21-0001";

            var parts = lastUser.IdNumber.Split('-');
            int num = int.Parse(parts[1]) + 1;

            return $"21-{num:D4}";
        }
    }
}
