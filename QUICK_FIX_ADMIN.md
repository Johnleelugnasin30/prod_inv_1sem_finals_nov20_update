# QUICK FIX: Create Admin Account

## Problem
"Admin account not found" error kapag nag-login sa Admin Login.

## Solution: Run SQL Script sa phpMyAdmin

### Step 1: Open phpMyAdmin
1. Open browser: `http://localhost/phpmyadmin`
2. Select database: `productinv_db`
3. Click "SQL" tab

### Step 2: Copy and Paste SQL Script

```sql
USE `productinv_db`;

-- Create Admin Account
-- Username: admin
-- Password: admin123
INSERT INTO `admin` (`Username`, `PasswordHash`) VALUES
('admin', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy')
ON DUPLICATE KEY UPDATE `PasswordHash` = VALUES(`PasswordHash`);

-- Create Admin Key
-- Key: ADMIN2024
INSERT INTO `admin_keys` (`key_hash`, `is_active`, `created_at`) VALUES
('ADMIN2024', TRUE, NOW())
ON DUPLICATE KEY UPDATE `is_active` = TRUE;
```

### Step 3: Click "Go" button

### Step 4: Verify
Run this query to check:
```sql
SELECT * FROM `admin`;
SELECT * FROM `admin_keys` WHERE `is_active` = TRUE;
```

### Step 5: Login
- **Username:** `admin`
- **Password:** `admin123`
- **Admin Key:** `ADMIN2024`

---

## Alternative: Direct Insert sa phpMyAdmin

1. Open phpMyAdmin: `http://localhost/phpmyadmin`
2. Select database: `productinv_db`
3. Click `admin` table
4. Click "Insert" tab
5. Fill in:
   - `Username`: `admin`
   - `PasswordHash`: `$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy`
6. Click "Go"

Then create admin key:
1. Click `admin_keys` table
2. Click "Insert" tab
3. Fill in:
   - `key_hash`: `ADMIN2024`
   - `is_active`: `1` (or check the checkbox)
   - `created_at`: Leave blank (will auto-fill)
4. Click "Go"

---

## After Login

Once logged in, you should be redirected to `/AdminDashboard`.

⚠️ **IMPORTANT:** Change the default password after first login for security!




