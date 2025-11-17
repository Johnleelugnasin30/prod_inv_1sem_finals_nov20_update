using System;

namespace ProductINV.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";

        /// <summary>
        /// Bcrypt password hash
        /// </summary>
        public string PasswordHash { get; set; } = "";

        public string? Position { get; set; }
        public string? IdNumber { get; set; }

        /// <summary>
        /// "User" or "Admin"
        /// </summary>
        public string Role { get; set; } = "User";

        public bool IsEmailVerified { get; set; } = false;
        public bool IsIdVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
    }
}
