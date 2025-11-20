-- ============================================
-- CWTP ASSET INVENTORY SYSTEM
-- Database Setup Script for XAMPP MySQL
-- ============================================

-- Create Database
CREATE DATABASE IF NOT EXISTS `productinv_db` CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;

-- Use Database
USE `productinv_db`;

-- ============================================
-- TABLE: users
-- Purpose: User account information
-- ============================================
CREATE TABLE IF NOT EXISTS `users` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `FullName` VARCHAR(255) NOT NULL,
  `Email` VARCHAR(255) NOT NULL UNIQUE,
  `Username` VARCHAR(255) NOT NULL UNIQUE,
  `PasswordHash` VARCHAR(255) NOT NULL,
  `Position` VARCHAR(255) NULL,
  `IdNumber` VARCHAR(50) NULL,
  `Role` VARCHAR(50) NOT NULL DEFAULT 'User',
  `IsEmailVerified` BOOLEAN NOT NULL DEFAULT FALSE,
  `IsIdVerified` BOOLEAN NOT NULL DEFAULT FALSE,
  `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
  `CreatedAt` DATETIME NOT NULL,
  `UpdatedAt` DATETIME NOT NULL,
  `LastLogin` DATETIME NULL,
  INDEX `idx_email` (`Email`),
  INDEX `idx_username` (`Username`),
  INDEX `idx_role` (`Role`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ============================================
-- TABLE: admin
-- Purpose: Admin account credentials
-- ============================================
CREATE TABLE IF NOT EXISTS `admin` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `Username` VARCHAR(255) NOT NULL UNIQUE,
  `PasswordHash` VARCHAR(255) NOT NULL,
  INDEX `idx_username` (`Username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ============================================
-- TABLE: products
-- Purpose: Inventory product information
-- ============================================
CREATE TABLE IF NOT EXISTS `products` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `ProductId` VARCHAR(255) NOT NULL UNIQUE,
  `Name` VARCHAR(255) NOT NULL,
  `Description` TEXT NULL,
  `Category` VARCHAR(255) NULL,
  `Price` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
  `Quantity` INT NOT NULL DEFAULT 0,
  `Location` VARCHAR(255) NULL,
  `SerialNumber` VARCHAR(255) NULL,
  `ConditionStatus` VARCHAR(50) NOT NULL DEFAULT 'Good',
  `IsAvailable` BOOLEAN NOT NULL DEFAULT TRUE,
  `ImageUrl` VARCHAR(500) NULL,
  `QrCodeUrl` VARCHAR(500) NULL,
  `CreatedBy` INT NULL,
  `CreatedAt` DATETIME NOT NULL,
  `UpdatedAt` DATETIME NOT NULL,
  INDEX `idx_productid` (`ProductId`),
  INDEX `idx_category` (`Category`),
  INDEX `idx_available` (`IsAvailable`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ============================================
-- TABLE: borrow_requests
-- Purpose: Product borrow requests from users
-- ============================================
CREATE TABLE IF NOT EXISTS `borrow_requests` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `UserId` INT NOT NULL,
  `ProductId` INT NOT NULL,
  `ProductName` VARCHAR(255) NOT NULL,
  `Purpose` TEXT NOT NULL,
  `RequestDate` DATETIME NOT NULL,
  `BorrowDate` DATETIME NULL,
  `ReturnDate` DATETIME NULL,
  `Status` VARCHAR(50) NOT NULL DEFAULT 'Pending',
  `AdminNotes` TEXT NULL,
  `CreatedAt` DATETIME NOT NULL,
  `UpdatedAt` DATETIME NOT NULL,
  FOREIGN KEY (`UserId`) REFERENCES `users`(`Id`) ON DELETE CASCADE,
  FOREIGN KEY (`ProductId`) REFERENCES `products`(`Id`) ON DELETE CASCADE,
  INDEX `idx_userid` (`UserId`),
  INDEX `idx_productid` (`ProductId`),
  INDEX `idx_status` (`Status`),
  INDEX `idx_requestdate` (`RequestDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ============================================
-- TABLE: admin_keys
-- Purpose: Admin access keys for authentication
-- ============================================
CREATE TABLE IF NOT EXISTS `admin_keys` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `key_hash` VARCHAR(255) NOT NULL,
  `is_active` BOOLEAN NOT NULL DEFAULT TRUE,
  `created_at` DATETIME NOT NULL,
  INDEX `idx_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ============================================
-- TABLE: verificationcodes
-- Purpose: Email verification codes and OTPs
-- ============================================
CREATE TABLE IF NOT EXISTS `verificationcodes` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `Email` VARCHAR(255) NOT NULL,
  `Code` VARCHAR(10) NOT NULL,
  `Type` VARCHAR(50) NOT NULL,
  `ExpiresAt` DATETIME NOT NULL,
  `IsUsed` BOOLEAN NOT NULL DEFAULT FALSE,
  `CreatedAt` DATETIME NOT NULL,
  INDEX `idx_email` (`Email`),
  INDEX `idx_code` (`Code`),
  INDEX `idx_type` (`Type`),
  INDEX `idx_expires` (`ExpiresAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ============================================
-- TABLE: password_reset_tokens
-- Purpose: Password reset tokens
-- ============================================
CREATE TABLE IF NOT EXISTS `password_reset_tokens` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `UserId` INT NOT NULL,
  `Token` VARCHAR(255) NOT NULL UNIQUE,
  `ExpiresAt` DATETIME NOT NULL,
  `IsUsed` BOOLEAN NOT NULL DEFAULT FALSE,
  `CreatedAt` DATETIME NOT NULL,
  FOREIGN KEY (`UserId`) REFERENCES `users`(`Id`) ON DELETE CASCADE,
  INDEX `idx_userid` (`UserId`),
  INDEX `idx_token` (`Token`),
  INDEX `idx_expires` (`ExpiresAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ============================================
-- TABLE: audit_logs
-- Purpose: System activity logs (login history, actions, etc.)
-- ============================================
CREATE TABLE IF NOT EXISTS `audit_logs` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `UserId` INT NULL,
  `Action` VARCHAR(100) NOT NULL,
  `TableName` VARCHAR(100) NULL,
  `RecordId` INT NULL,
  `OldValues` TEXT NULL,
  `NewValues` TEXT NULL,
  `IpAddress` VARCHAR(50) NULL,
  `CreatedAt` DATETIME NOT NULL,
  FOREIGN KEY (`UserId`) REFERENCES `users`(`Id`) ON DELETE SET NULL,
  INDEX `idx_userid` (`UserId`),
  INDEX `idx_action` (`Action`),
  INDEX `idx_createdat` (`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ============================================
-- SAMPLE DATA (Optional - for testing)
-- ============================================

-- Insert Sample Admin Account
-- Password: admin123 (BCrypt hash)
INSERT INTO `admin` (`Username`, `PasswordHash`) VALUES
('admin', '$2a$11$KIXLQZJXJXJXJXJXJXJXJOmXJXJXJXJXJXJXJXJXJXJXJXJXJXJXJXJX');

-- Insert Sample Admin Key
-- Key: ADMIN2024
INSERT INTO `admin_keys` (`key_hash`, `is_active`, `created_at`) VALUES
('ADMIN2024', TRUE, NOW());

-- Insert Sample User (for testing)
-- Password: user123 (BCrypt hash)
-- Note: Replace with actual BCrypt hash when creating real users
INSERT INTO `users` (`FullName`, `Email`, `Username`, `PasswordHash`, `Position`, `IdNumber`, `Role`, `IsEmailVerified`, `IsActive`, `CreatedAt`, `UpdatedAt`) VALUES
('Test User', 'test@example.com', 'testuser', '$2a$11$KIXLQZJXJXJXJXJXJXJXJOmXJXJXJXJXJXJXJXJXJXJXJXJXJXJXJXJX', 'Missionary', '21-0001', 'User', TRUE, TRUE, NOW(), NOW());

-- ============================================
-- VERIFICATION QUERIES
-- ============================================

-- Check if all tables are created
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'productinv_db'
ORDER BY TABLE_NAME;

-- Count records in each table
SELECT 'users' AS TableName, COUNT(*) AS RecordCount FROM users
UNION ALL
SELECT 'admin', COUNT(*) FROM admin
UNION ALL
SELECT 'products', COUNT(*) FROM products
UNION ALL
SELECT 'borrow_requests', COUNT(*) FROM borrow_requests
UNION ALL
SELECT 'admin_keys', COUNT(*) FROM admin_keys
UNION ALL
SELECT 'verificationcodes', COUNT(*) FROM verificationcodes
UNION ALL
SELECT 'password_reset_tokens', COUNT(*) FROM password_reset_tokens
UNION ALL
SELECT 'audit_logs', COUNT(*) FROM audit_logs;

-- ============================================
-- END OF SCRIPT
-- ============================================





