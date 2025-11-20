using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProductINV.Pages
{
    public class IndexModel : PageModel
    {
        public string Username { get; set; } = "";

        public IActionResult OnGet()
        {
            // Check if user is logged in
            var sessionUsername = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(sessionUsername))
            {
                // User not logged in, redirect to login page
                return RedirectToPage("/Login");
            }

            // User is logged in, set the username
            Username = sessionUsername;
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            // Clear session and redirect to login
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}