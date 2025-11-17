using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ProductINV.Models;

namespace ProductINV.Services
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private MySqlConnection GetConnection() => new MySqlConnection(_connectionString);

        /// <summary>
        /// Get user by email OR username (for login).
        /// </summary>
        public User? GetByEmailOrUsername(string identifier)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT 
                    id,
                    full_name,
                    email,
                    username,
                    password_hash,
                    position,
                    id_number,
                    role,
                    is_email_verified,
                    is_id_verified,
                    is_active,
                    created_at,
                    updated_at,
                    last_login
                FROM users
                WHERE email = @identifier OR username = @identifier
                LIMIT 1;
            ";
            cmd.Parameters.AddWithValue("@identifier", identifier);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return new User
            {
                Id = reader.GetInt32("id"),
                FullName = reader.GetString("full_name"),
                Email = reader.GetString("email"),
                Username = reader.GetString("username"),
                PasswordHash = reader.GetString("password_hash"),
                Position = reader["position"] as string,
                IdNumber = reader["id_number"] as string,
                Role = reader.GetString("role"),
                IsEmailVerified = reader.GetBoolean("is_email_verified"),
                IsIdVerified = reader.GetBoolean("is_id_verified"),
                IsActive = reader.GetBoolean("is_active"),
                CreatedAt = reader.GetDateTime("created_at"),
                UpdatedAt = reader.GetDateTime("updated_at"),
                LastLogin = reader["last_login"] == DBNull.Value
                    ? null
                    : reader.GetDateTime("last_login")
            };
        }

        /// <summary>
        /// Check if email OR username is already taken.
        /// </summary>
        public bool EmailOrUsernameExists(string email, string username)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT COUNT(*) 
                FROM users
                WHERE email = @email OR username = @username;
            ";
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@username", username);

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        /// <summary>
        /// Create a new user with bcrypt-hashed password.
        /// </summary>
        public User CreateUser(
            string fullName,
            string email,
            string username,
            string plainPassword,
            string? position,
            string? idNumber,
            string role = "User")
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO users (
                    full_name,
                    email,
                    username,
                    password_hash,
                    position,
                    id_number,
                    role,
                    is_email_verified,
                    is_id_verified,
                    is_active,
                    created_at,
                    updated_at
                )
                VALUES (
                    @full_name,
                    @email,
                    @username,
                    @password_hash,
                    @position,
                    @id_number,
                    @role,
                    0,
                    0,
                    1,
                    NOW(),
                    NOW()
                );
                SELECT LAST_INSERT_ID();
            ";

            cmd.Parameters.AddWithValue("@full_name", fullName);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password_hash", passwordHash);
            cmd.Parameters.AddWithValue("@position", (object?)position ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id_number", (object?)idNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@role", role);

            var newId = Convert.ToInt32(cmd.ExecuteScalar());

            return new User
            {
                Id = newId,
                FullName = fullName,
                Email = email,
                Username = username,
                PasswordHash = passwordHash,
                Position = position,
                IdNumber = idNumber,
                Role = role,
                IsEmailVerified = false,
                IsIdVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Mark last successful login.
        /// </summary>
        public void MarkSuccessfulLogin(int userId)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE users
                SET last_login = NOW()
                WHERE id = @id;
            ";
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Verify bcrypt password.
        /// </summary>
        public bool VerifyPassword(string hash, string plainPassword)
        {
            if (string.IsNullOrEmpty(hash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
