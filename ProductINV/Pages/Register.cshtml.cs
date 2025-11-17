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

                if (await _db.Users.AnyAsync(u => u.Email == Email.Trim()))
                {
                    ErrorMessage = "Email already exists.";
                    return Page();
                }

                if (await _db.Users.AnyAsync(u => u.Username == Username.Trim()))
                {
                    ErrorMessage = "Username already exists.";
                    return Page();
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password.Trim());

                var user = new User
                {
                    FullName = FullName.Trim(),
                    Email = Email.Trim(),
                    Username = Username.Trim(),
                    PasswordHash = hashedPassword,
                    Position = Position?.Trim(),
                    IdNumber = IdNumber,     // Auto-generated ID
                    Role = "User",
                    IsActive = true,
                    IsEmailVerified = false,
                    IsIdVerified = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                SuccessMessage = "Account created successfully! Redirecting...";
                return Page();
            }
            catch (Exception ex)
            {
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
