using Microsoft.EntityFrameworkCore;
using ProductINV.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AdminKey> AdminKeys { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    // *** FIXED HERE ***
    public DbSet<Admin> Admins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToTable("products");
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<AdminKey>().ToTable("admin_keys");
        modelBuilder.Entity<VerificationCode>().ToTable("verificationcodes");
        modelBuilder.Entity<PasswordResetToken>().ToTable("password_reset_tokens");
        modelBuilder.Entity<AuditLog>().ToTable("audit_logs");

        // *** FIXED MAPPING TO MATCH DATABASE TABLE ***
        modelBuilder.Entity<Admin>().ToTable("admin");
    }
}
