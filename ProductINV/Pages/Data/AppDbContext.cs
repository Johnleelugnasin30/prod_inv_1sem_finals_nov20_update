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
    public DbSet<BorrowRequest> BorrowRequests { get; set; }

    // *** FIXED HERE ***
    public DbSet<Admin> Admins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToTable("products");
        modelBuilder.Entity<User>().ToTable("users");
        
        // AdminKey entity mapping with explicit column names
        modelBuilder.Entity<AdminKey>().ToTable("admin_keys");
        modelBuilder.Entity<AdminKey>().HasKey(a => a.Id);
        modelBuilder.Entity<AdminKey>().Property(a => a.Id).HasColumnName("Id").ValueGeneratedOnAdd();
        modelBuilder.Entity<AdminKey>().Property(a => a.key_hash).HasColumnName("key_hash").IsRequired();
        modelBuilder.Entity<AdminKey>().Property(a => a.is_active).HasColumnName("is_active");
        modelBuilder.Entity<AdminKey>().Property(a => a.created_at).HasColumnName("created_at");
        
        modelBuilder.Entity<VerificationCode>().ToTable("verificationcodes");
        modelBuilder.Entity<PasswordResetToken>().ToTable("password_reset_tokens");
        modelBuilder.Entity<AuditLog>().ToTable("audit_logs");
        modelBuilder.Entity<BorrowRequest>().ToTable("borrow_requests");

        // *** FIXED MAPPING TO MATCH DATABASE TABLE ***
        modelBuilder.Entity<Admin>().ToTable("admin");
    }
}
