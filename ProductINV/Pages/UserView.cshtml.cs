using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductINV.Pages
{
    public class UserViewModel : PageModel
    {
        // Product list
        public List<Product> Products { get; set; }

        // Product Info
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public Product SelectedProduct { get; set; }

        // QR Code
        [BindProperty(SupportsGet = true)]
        public string QRProductId { get; set; }
        public string QRCodeUrl { get; set; }

        public string Username { get; set; }

        public void OnGet()
        {
            Username = "User";

            LoadProducts();

            if (!string.IsNullOrEmpty(SearchTerm))
                SearchProduct();

            if (!string.IsNullOrEmpty(QRProductId))
                GenerateQR();
        }

        public IActionResult OnGetSearchProduct(string searchTerm)
        {
            SearchTerm = searchTerm;
            LoadProducts();
            SearchProduct();
            return Page();
        }

        public IActionResult OnGetGenerateQR(string productId)
        {
            QRProductId = productId;
            LoadProducts();
            GenerateQR();
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            return RedirectToPage("/Login");
        }

        private void LoadProducts()
        {
            Products = new List<Product>
            {
                new Product { ProductId="PROD001", Name="Dell Laptop", Category="Laptop", Location="IT Room", IsAvailable=true },
                new Product { ProductId="PROD002", Name="HP Printer", Category="Printer", Location="Admin Office", IsAvailable=false },
                new Product { ProductId="PROD003", Name="Samsung Monitor", Category="Monitor", Location="Conference Room", IsAvailable=true },
                new Product { ProductId="PROD004", Name="Logitech Mouse", Category="Accessory", Location="Warehouse", IsAvailable=true },
            };
        }

        private void SearchProduct()
        {
            SelectedProduct = Products.FirstOrDefault(p =>
                p.ProductId.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        private void GenerateQR()
        {
            QRCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={QRProductId}";
        }
    }

    public class Product
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public bool IsAvailable { get; set; }
    }
}
