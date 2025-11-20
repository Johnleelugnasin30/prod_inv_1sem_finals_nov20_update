using System;

namespace ProductINV.Models
{
    public class VerificationCode
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Code { get; set; } = "";
        public string Type { get; set; } = ""; // email_verification, password_reset, login_otp
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
