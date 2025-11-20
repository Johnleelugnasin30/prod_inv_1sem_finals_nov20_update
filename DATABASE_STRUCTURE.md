# DATABASE STRUCTURE - CWTP Asset Inventory System

## Overview
Ito ang complete database structure ng application. Lahat ng tables ay naka-map sa MySQL database.

---

## 📊 TABLES

### 1. **users** Table
**Purpose:** Stores user account information

| Column Name | Data Type | Nullable | Description |
|------------|-----------|----------|-------------|
| `Id` | INT | NO | Primary Key, Auto Increment |
| `FullName` | VARCHAR | NO | User's full name |
| `Email` | VARCHAR | NO | User's email address (unique) |
| `Username` | VARCHAR | NO | Username for login (unique) |
| `PasswordHash` | VARCHAR | NO | BCrypt hashed password |
| `Position` | VARCHAR | YES | User's position (e.g., Missionary, Admin Finance, ICT Head) |
| `IdNumber` | VARCHAR | YES | Auto-generated ID number (format: 21-0001) |
| `Role` | VARCHAR | NO | User role: "User" or "Admin" (default: "User") |
| `IsEmailVerified` | BOOLEAN | NO | Email verification status (default: false) |
| `IsIdVerified` | BOOLEAN | NO | ID verification status (default: false) |
| `IsActive` | BOOLEAN | NO | Account active status (default: true) |
| `CreatedAt` | DATETIME | NO | Account creation timestamp |
| `UpdatedAt` | DATETIME | NO | Last update timestamp |
| `LastLogin` | DATETIME | YES | Last login timestamp |

**Indexes:**
- Primary Key: `Id`
- Unique: `Email`, `Username`

---

### 2. **admin** Table
**Purpose:** Stores admin account credentials

| Column Name | Data Type | Nullable | Description |
|------------|-----------|----------|-------------|
| `Id` | INT | NO | Primary Key, Auto Increment |
| `Username` | VARCHAR | NO | Admin username (unique) |
| `PasswordHash` | VARCHAR | NO | BCrypt hashed password |

**Indexes:**
- Primary Key: `Id`
- Unique: `Username`

---

### 3. **products** Table
**Purpose:** Stores inventory product information

| Column Name | Data Type | Nullable | Description |
|------------|-----------|----------|-------------|
| `Id` | INT | NO | Primary Key, Auto Increment |
| `ProductId` | VARCHAR | NO | Unique product identifier |
| `Name` | VARCHAR | NO | Product name |
| `Description` | TEXT | YES | Product description |
| `Category` | VARCHAR | YES | Product category |
| `Price` | DECIMAL(18,2) | NO | Product price |
| `Quantity` | INT | NO | Available quantity |
| `Location` | VARCHAR | YES | Storage location |
| `SerialNumber` | VARCHAR | YES | Product serial number |
| `ConditionStatus` | VARCHAR | NO | Condition status (default: "Good") |
| `IsAvailable` | BOOLEAN | NO | Availability status (default: true) |
| `ImageUrl` | VARCHAR | YES | Product image URL |
| `QrCodeUrl` | VARCHAR | YES | QR code image URL |
| `CreatedBy` | INT | YES | User ID who created the product |
| `CreatedAt` | DATETIME | NO | Creation timestamp |
| `UpdatedAt` | DATETIME | NO | Last update timestamp |

**Indexes:**
- Primary Key: `Id`
- Unique: `ProductId`

---

### 4. **borrow_requests** Table
**Purpose:** Stores product borrow requests from users

| Column Name | Data Type | Nullable | Description |
|------------|-----------|----------|-------------|
| `Id` | INT | NO | Primary Key, Auto Increment |
| `UserId` | INT | NO | Foreign Key to users.Id |
| `ProductId` | INT | NO | Foreign Key to products.Id |
| `ProductName` | VARCHAR | NO | Product name (denormalized for quick access) |
| `Purpose` | TEXT | NO | Reason for borrowing |
| `RequestDate` | DATETIME | NO | Request submission date |
| `BorrowDate` | DATETIME | YES | Date when item was borrowed |
| `ReturnDate` | DATETIME | YES | Date when item was returned |
| `Status` | VARCHAR | NO | Status: "Pending", "Approved", "Rejected", "Borrowed", "Returned" (default: "Pending") |
| `AdminNotes` | TEXT | YES | Admin notes/comments |
| `CreatedAt` | DATETIME | NO | Creation timestamp |
| `UpdatedAt` | DATETIME | NO | Last update timestamp |

**Indexes:**
- Primary Key: `Id`
- Foreign Keys: `UserId` → `users.Id`, `ProductId` → `products.Id`
- Index: `UserId`, `ProductId`, `Status`

---

### 5. **admin_keys** Table
**Purpose:** Stores admin access keys for authentication

| Column Name | Data Type | Nullable | Description |
|------------|-----------|----------|-------------|
| `Id` | INT | NO | Primary Key, Auto Increment |
| `key_hash` | VARCHAR | NO | Hashed admin key |
| `is_active` | BOOLEAN | NO | Key active status (default: true) |
| `created_at` | DATETIME | NO | Key creation timestamp |

**Indexes:**
- Primary Key: `Id`
- Index: `is_active`

---

### 6. **verificationcodes** Table
**Purpose:** Stores email verification codes and OTPs

| Column Name | Data Type | Nullable | Description |
|------------|-----------|----------|-------------|
| `Id` | INT | NO | Primary Key, Auto Increment |
| `Email` | VARCHAR | NO | Email address |
| `Code` | VARCHAR | NO | 6-digit verification code |
| `Type` | VARCHAR | NO | Type: "email_verification", "password_reset", "login_otp" |
| `ExpiresAt` | DATETIME | NO | Code expiration timestamp |
| `IsUsed` | BOOLEAN | NO | Whether code has been used (default: false) |
| `CreatedAt` | DATETIME | NO | Creation timestamp |

**Indexes:**
- Primary Key: `Id`
- Index: `Email`, `Code`, `Type`, `ExpiresAt`

---

### 7. **password_reset_tokens** Table
**Purpose:** Stores password reset tokens

| Column Name | Data Type | Nullable | Description |
|------------|-----------|----------|-------------|
| `Id` | INT | NO | Primary Key, Auto Increment |
| `UserId` | INT | NO | Foreign Key to users.Id |
| `Token` | VARCHAR | NO | Unique reset token |
| `ExpiresAt` | DATETIME | NO | Token expiration timestamp |
| `IsUsed` | BOOLEAN | NO | Whether token has been used (default: false) |
| `CreatedAt` | DATETIME | NO | Creation timestamp |

**Indexes:**
- Primary Key: `Id`
- Foreign Key: `UserId` → `users.Id`
- Unique: `Token`
- Index: `UserId`, `ExpiresAt`

---

### 8. **audit_logs** Table
**Purpose:** Stores system activity logs (login history, actions, etc.)

| Column Name | Data Type | Nullable | Description |
|------------|-----------|----------|-------------|
| `Id` | INT | NO | Primary Key, Auto Increment |
| `UserId` | INT | YES | Foreign Key to users.Id (nullable for system actions) |
| `Action` | VARCHAR | NO | Action performed (e.g., "Login", "Create", "Update", "Delete") |
| `TableName` | VARCHAR | YES | Table affected |
| `RecordId` | INT | YES | Record ID affected |
| `OldValues` | TEXT | YES | Previous values (JSON format) |
| `NewValues` | TEXT | YES | New values (JSON format) |
| `IpAddress` | VARCHAR | YES | User's IP address |
| `CreatedAt` | DATETIME | NO | Log timestamp |

**Indexes:**
- Primary Key: `Id`
- Foreign Key: `UserId` → `users.Id`
- Index: `UserId`, `Action`, `CreatedAt`

---

## 🔗 RELATIONSHIPS

```
users (1) ──→ (N) borrow_requests
products (1) ──→ (N) borrow_requests
users (1) ──→ (N) audit_logs
users (1) ──→ (N) password_reset_tokens
```

---

## 📝 NOTES

1. **Password Security:** Lahat ng passwords ay naka-hash gamit ang BCrypt algorithm
2. **Timestamps:** Lahat ng tables ay may `CreatedAt` at `UpdatedAt` fields para sa tracking
3. **Soft Delete:** Hindi nag-delete ng records, nag-set lang ng `IsActive = false` para sa users
4. **Status Fields:** Maraming tables ang may status fields para sa state management
5. **Foreign Keys:** May foreign key relationships para sa data integrity

---

## 🗄️ SQL CREATE TABLE STATEMENTS

Para sa reference, narito ang SQL statements para sa pag-create ng tables:

```sql
-- Users Table
CREATE TABLE `users` (
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
  `LastLogin` DATETIME NULL
);

-- Admin Table
CREATE TABLE `admin` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `Username` VARCHAR(255) NOT NULL UNIQUE,
  `PasswordHash` VARCHAR(255) NOT NULL
);

-- Products Table
CREATE TABLE `products` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `ProductId` VARCHAR(255) NOT NULL UNIQUE,
  `Name` VARCHAR(255) NOT NULL,
  `Description` TEXT NULL,
  `Category` VARCHAR(255) NULL,
  `Price` DECIMAL(18,2) NOT NULL,
  `Quantity` INT NOT NULL,
  `Location` VARCHAR(255) NULL,
  `SerialNumber` VARCHAR(255) NULL,
  `ConditionStatus` VARCHAR(50) NOT NULL DEFAULT 'Good',
  `IsAvailable` BOOLEAN NOT NULL DEFAULT TRUE,
  `ImageUrl` VARCHAR(500) NULL,
  `QrCodeUrl` VARCHAR(500) NULL,
  `CreatedBy` INT NULL,
  `CreatedAt` DATETIME NOT NULL,
  `UpdatedAt` DATETIME NOT NULL
);

-- Borrow Requests Table
CREATE TABLE `borrow_requests` (
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
  FOREIGN KEY (`UserId`) REFERENCES `users`(`Id`),
  FOREIGN KEY (`ProductId`) REFERENCES `products`(`Id`)
);

-- Admin Keys Table
CREATE TABLE `admin_keys` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `key_hash` VARCHAR(255) NOT NULL,
  `is_active` BOOLEAN NOT NULL DEFAULT TRUE,
  `created_at` DATETIME NOT NULL
);

-- Verification Codes Table
CREATE TABLE `verificationcodes` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `Email` VARCHAR(255) NOT NULL,
  `Code` VARCHAR(10) NOT NULL,
  `Type` VARCHAR(50) NOT NULL,
  `ExpiresAt` DATETIME NOT NULL,
  `IsUsed` BOOLEAN NOT NULL DEFAULT FALSE,
  `CreatedAt` DATETIME NOT NULL
);

-- Password Reset Tokens Table
CREATE TABLE `password_reset_tokens` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `UserId` INT NOT NULL,
  `Token` VARCHAR(255) NOT NULL UNIQUE,
  `ExpiresAt` DATETIME NOT NULL,
  `IsUsed` BOOLEAN NOT NULL DEFAULT FALSE,
  `CreatedAt` DATETIME NOT NULL,
  FOREIGN KEY (`UserId`) REFERENCES `users`(`Id`)
);

-- Audit Logs Table
CREATE TABLE `audit_logs` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `UserId` INT NULL,
  `Action` VARCHAR(100) NOT NULL,
  `TableName` VARCHAR(100) NULL,
  `RecordId` INT NULL,
  `OldValues` TEXT NULL,
  `NewValues` TEXT NULL,
  `IpAddress` VARCHAR(50) NULL,
  `CreatedAt` DATETIME NOT NULL,
  FOREIGN KEY (`UserId`) REFERENCES `users`(`Id`)
);
```

---

## 📌 IMPORTANT REMINDERS

1. **Database Connection:** I-configure ang connection string sa `appsettings.json`
2. **Migrations:** Gamitin ang Entity Framework migrations para sa database updates
3. **Backup:** Regular na mag-backup ng database
4. **Indexes:** May mga indexes na para sa performance optimization
5. **Foreign Keys:** May foreign key constraints para sa data integrity

---

**Last Updated:** November 2024
**Database:** MySQL 8.0+
**ORM:** Entity Framework Core (Pomelo.EntityFrameworkCore.MySql)

