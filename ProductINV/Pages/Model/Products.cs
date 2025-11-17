using System;

namespace ProductINV.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductId { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Category { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public string? Location { get; set; }
        public string? SerialNumber { get; set; }
        public string ConditionStatus { get; set; } = "Good";
        public bool IsAvailable { get; set; } = true;

        public string? ImageUrl { get; set; }
        public string? QrCodeUrl { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
