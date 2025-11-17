using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using InventoryRazor.Models;
using System;
using System.Collections.Generic;

namespace InventoryRazor.Pages
{
    public class ManageProductModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ManageProductModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionStatus { get; private set; }
        public List<Product> Products { get; private set; } = new List<Product>();

        [BindProperty]
        public Product NewProduct { get; set; }

        [BindProperty]
        public Product EditProduct { get; set; }  // For editing

        [BindProperty(SupportsGet = true)]
        public int? EditProductId { get; set; }   // To trigger edit modal open

        public bool ShowEditModal { get; set; } = false;

        public void OnGet()
        {
            LoadProducts();

            // If EditProductId is set, load that product for editing and show modal
            if (EditProductId.HasValue)
            {
                LoadEditProduct(EditProductId.Value);
                ShowEditModal = true;
            }
        }

        public IActionResult OnPostCreate()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "INSERT INTO products (name, price, quantity) VALUES (@name, @price, @quantity)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@name", NewProduct.Name);
                    cmd.Parameters.AddWithValue("@price", NewProduct.Price);
                    cmd.Parameters.AddWithValue("@quantity", NewProduct.Quantity);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }

                return RedirectToPage(); // Auto-refresh
            }
            catch (Exception ex)
            {
                ConnectionStatus = "? Failed to add product: " + ex.Message;
                LoadProducts();
                return Page();
            }
        }

        public IActionResult OnPostUpdate()
        {
            if (EditProduct == null || EditProduct.Id == 0)
            {
                ConnectionStatus = "? Invalid product data.";
                LoadProducts();
                return Page();
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "UPDATE products SET name = @name, price = @price, quantity = @quantity WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@name", EditProduct.Name);
                    cmd.Parameters.AddWithValue("@price", EditProduct.Price);
                    cmd.Parameters.AddWithValue("@quantity", EditProduct.Quantity);
                    cmd.Parameters.AddWithValue("@id", EditProduct.Id);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }

                return RedirectToPage(); // Refresh list after update
            }
            catch (Exception ex)
            {
                ConnectionStatus = "? Failed to update product: " + ex.Message;
                LoadProducts();
                return Page();
            }
        }

        public void select(string tablename,string fields1, string fields2, string fields3)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            Products.Clear();


            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    ConnectionStatus = "";

                    string sql = $"SELECT  {fields1},{fields2 },{fields3}, quantity FROM {tablename}";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();




                    while (reader.Read())
                    {
                        Products.Add(new Product
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Price = Convert.ToDecimal(reader["price"]),
                            Quantity = Convert.ToInt32(reader["quantity"])
                        });
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = "? Connection failed: " + ex.Message;
            }
        }






        private void LoadProducts()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            Products.Clear();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    ConnectionStatus = "";

                    string sql = "SELECT id, name, price, quantity FROM products";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Products.Add(new Product
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"].ToString(),
                            Price = Convert.ToDecimal(reader["price"]),
                            Quantity = Convert.ToInt32(reader["quantity"])
                        });
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = "? Connection failed: " + ex.Message;
            }
        }

        // Add inside your ManageProductModel class

        public IActionResult OnPostDelete(int id)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "DELETE FROM products WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }

                return RedirectToPage(); // Refresh list
            }
            catch (Exception ex)
            {
                ConnectionStatus = "? Failed to delete product: " + ex.Message;
                LoadProducts();
                return Page();
            }
        }


        private void LoadEditProduct(int id)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT id, name, price, quantity FROM products WHERE id = @id LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            EditProduct = new Product
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString(),
                                Price = Convert.ToDecimal(reader["price"]),
                                Quantity = Convert.ToInt32(reader["quantity"])
                            };
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = "? Failed to load product for edit: " + ex.Message;
            }
        }
    }
}
