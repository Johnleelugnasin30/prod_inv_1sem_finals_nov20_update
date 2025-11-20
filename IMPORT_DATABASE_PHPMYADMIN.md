# 📥 HOW TO IMPORT DATABASE SA XAMPP phpMyAdmin

## 🚀 EASY METHOD (Recommended)

### Step 1: Start XAMPP MySQL
1. Open **XAMPP Control Panel**
2. Click **"Start"** sa MySQL row
3. Wait for green indicator ✅

### Step 2: Open phpMyAdmin
1. Open browser
2. Go to: **http://localhost/phpMyAdmin**
3. Login (usually no password needed):
   - Username: `root`
   - Password: (leave blank)

### Step 3: Create Database
1. Click **"New"** sa left sidebar
2. Database name: **`productinv_db`**
3. Collation: **`utf8mb4_general_ci`**
4. Click **"Create"** button

### Step 4: Import SQL File
1. **Select** `productinv_db` database sa left sidebar
2. Click **"Import"** tab sa top menu
3. Click **"Choose File"** button
4. Navigate to: **`database_setup.sql`** file
5. Click **"Go"** button sa bottom

### Step 5: Verify
1. Check sa left sidebar - dapat may 8 tables:
   - ✅ users
   - ✅ admin
   - ✅ products
   - ✅ borrow_requests
   - ✅ admin_keys
   - ✅ verificationcodes
   - ✅ password_reset_tokens
   - ✅ audit_logs

---

## 📋 ALTERNATIVE METHOD (Manual Copy-Paste)

### Step 1: Open SQL File
1. Open **`database_setup.sql`** file sa text editor
2. Copy **ALL** content (Ctrl+A, Ctrl+C)

### Step 2: Run SQL sa phpMyAdmin
1. Open phpMyAdmin: **http://localhost/phpMyAdmin**
2. Create database **`productinv_db`** (if not exists)
3. Select **`productinv_db`** database
4. Click **"SQL"** tab
5. Paste lahat ng SQL code
6. Click **"Go"** button

---

## ✅ VERIFICATION

### Check Tables Created
```sql
SHOW TABLES;
```

Dapat makita mo:
- users
- admin
- products
- borrow_requests
- admin_keys
- verificationcodes
- password_reset_tokens
- audit_logs

### Check Sample Data
```sql
SELECT * FROM admin;
SELECT * FROM users;
SELECT * FROM admin_keys;
```

---

## 🔧 TROUBLESHOOTING

### ❌ "Cannot connect to MySQL"
**Solution:**
- Check XAMPP MySQL is running
- Verify port 3306 is available
- Restart XAMPP MySQL service

### ❌ "Access denied for user 'root'"
**Solution:**
- Leave password blank sa phpMyAdmin login
- Or reset MySQL password sa XAMPP

### ❌ "Database already exists"
**Solution:**
- Either drop existing database:
  ```sql
  DROP DATABASE productinv_db;
  ```
- Or use different database name

### ❌ "Table already exists"
**Solution:**
- Drop existing tables first:
  ```sql
  DROP TABLE IF EXISTS users, admin, products, borrow_requests, admin_keys, verificationcodes, password_reset_tokens, audit_logs;
  ```
- Then import again

### ❌ "Import file too large"
**Solution:**
- Increase upload limit sa phpMyAdmin
- Or use SQL tab method (copy-paste)

---

## 📝 QUICK REFERENCE

**phpMyAdmin URL:** http://localhost/phpMyAdmin  
**Database Name:** productinv_db  
**SQL File:** database_setup.sql  
**Default MySQL User:** root (no password)

---

## 🎯 AFTER IMPORT

1. ✅ Database created
2. ✅ All tables created
3. ✅ Sample data inserted
4. ✅ Ready to use!

**Next Step:** Run the application and test!

---

**Need Help?** Check `XAMPP_SETUP_GUIDE.md` for more details.




