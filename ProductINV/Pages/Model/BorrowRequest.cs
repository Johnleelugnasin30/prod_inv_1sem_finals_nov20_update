using System;

namespace ProductINV.Models
{
    public class BorrowRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Purpose { get; set; } = "";
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Borrowed, Returned
        public string? AdminNotes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

