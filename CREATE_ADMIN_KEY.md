# CREATE ADMIN KEY - Quick Guide

## Problem
Walang laman ang `admin_keys` table sa database, kaya hindi makapag-login ang admin.

## Solution: Gumawa ng Admin Key

---

## Method 1: Direct SQL sa phpMyAdmin (RECOMMENDED - Mas Madali)

### Step 1: Open phpMyAdmin
1. Buksan ang browser: `http://localhost/phpmyadmin`
2. Piliin ang database: `productinv_db`
3. I-click ang "SQL" tab

### Step 2: Copy at Paste ang SQL Script

```sql
USE `productinv_db`;

-- Create Admin Key
-- Key: ADMIN2024
INSERT INTO `admin_keys` (`key_hash`, `is_active`, `created_at`) VALUES
('ADMIN2024', TRUE, NOW());
```

### Step 3: I-click ang "Go" button

### Step 4: Verify
Run this query para makita ang admin key:
```sql
SELECT * FROM `admin_keys` WHERE `is_active` = TRUE;
```

### Step 5: Gamitin ang Admin Key
Kapag nag-login ka bilang admin, gamitin ang:
- **Admin Key:** `ADMIN2024`

---

## Method 2: Via Application UI (Kailangan ng Master Key)

### Step 1: Open Login Page
1. Buksan ang browser: `http://localhost:5072/Login`
2. I-click ang "Admin Login" tab
3. I-click ang "Create Admin Key" button sa baba

### Step 2: Fill up ang Form
- **Master Key:** `MASTER2024` (ito ang default master key)
- **New Admin Key:** Ilagay ang gusto mong admin key (hal. `ADMIN2024`)

### Step 3: I-click ang "Create Key" button

---

## Method 3: Multiple Admin Keys (Optional)

Kung gusto mong gumawa ng maraming admin keys, pwede mong i-insert multiple:

```sql
USE `productinv_db`;

-- Create Multiple Admin Keys
INSERT INTO `admin_keys` (`key_hash`, `is_active`, `created_at`) VALUES
('ADMIN2024', TRUE, NOW()),
('ADMIN2025', TRUE, NOW()),
('CWTP2024', TRUE, NOW());
```

---

## Important Notes:

1. **Master Key:** Ang master key ay `MASTER2024` (nakalagay sa code)
2. **Admin Key Format:** Pwede mong gamitin ang kahit anong format (letters, numbers, special characters)
3. **Active Keys:** Tanging active keys lang (`is_active = TRUE`) ang gagana
4. **Security:** Huwag ibahagi ang admin key sa iba

---

## Troubleshooting:

### Problem: "Invalid admin key" error
**Solution:** 
- Siguraduhin na tama ang spelling ng admin key
- Check kung active ang key: `SELECT * FROM admin_keys WHERE is_active = TRUE;`

### Problem: Hindi makagawa ng admin key via UI
**Solution:** 
- Gamitin ang Method 1 (Direct SQL) - mas reliable
- O i-check kung tama ang master key (`MASTER2024`)

### Problem: Walang admin_keys table
**Solution:** 
- Run ang `database_setup.sql` script muna
- O gumawa ng table manually:
```sql
CREATE TABLE IF NOT EXISTS `admin_keys` (
  `Id` INT AUTO_INCREMENT PRIMARY KEY,
  `key_hash` VARCHAR(255) NOT NULL,
  `is_active` BOOLEAN NOT NULL DEFAULT TRUE,
  `created_at` DATETIME NOT NULL,
  INDEX `idx_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
```

---

## After Creating Admin Key:

1. **Create Admin Account** (kung wala pa):
   ```sql
   INSERT INTO `admin` (`Username`, `PasswordHash`) VALUES
   ('admin', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy');
   ```
   (Password: `admin123`)

2. **Login Credentials:**
   - Username: `admin`
   - Password: `admin123`
   - Admin Key: `ADMIN2024` (o kung ano ang ginawa mo)

3. **Test Login:**
   - Pumunta sa: `http://localhost:5072/Login`
   - I-click ang "Admin Login" tab
   - Ilagay ang credentials
   - Dapat makapasok ka na!




