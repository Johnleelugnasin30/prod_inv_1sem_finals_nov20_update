using System;

namespace ProductINV.Models
{
    public class AdminKey
    {
        public int Id { get; set; }
        public string key_hash { get; set; } = "";
        public bool is_active { get; set; } = true;
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
