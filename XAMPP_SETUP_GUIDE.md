# XAMPP DATABASE SETUP GUIDE

## 📋 REQUIREMENTS

- XAMPP installed sa computer
- MySQL service running sa XAMPP
- phpMyAdmin access

---

## 🚀 STEP-BY-STEP SETUP

### STEP 1: Start XAMPP Services

1. **Open XAMPP Control Panel**
2. **Start MySQL** - Click "Start" button sa MySQL row
3. **Start Apache** (optional, para sa phpMyAdmin)
4. Verify na running ang MySQL (green indicator)

---

### STEP 2: Access phpMyAdmin

1. **Open browser**
2. Go to: `http://localhost/phpMyAdmin`
3. Login credentials:
   - **Username:** `root`
   - **Password:** (leave blank - default XAMPP)

---

### STEP 3: Create Database

**Option A: Using phpMyAdmin (GUI)**

1. Click **"New"** sa left sidebar
2. Enter database name: `productinv_db`
3. Select collation: `utf8mb4_general_ci`
4. Click **"Create"**

**Option B: Using SQL Query**

```sql
CREATE DATABASE productinv_db CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
```

---

### STEP 4: Import Database Structure

**Option A: Using SQL Script File**

1. Open `database_setup.sql` file
2. Copy lahat ng SQL code
3. Sa phpMyAdmin:
   - Select `productinv_db` database
   - Click **"SQL"** tab
   - Paste SQL code
   - Click **"Go"**

**Option B: Manual Table Creation**

Follow the SQL statements sa `database_setup.sql` file.

---

### STEP 5: Configure Application

1. **Check appsettings.json**
   - Location: `ProductINV/appsettings.json`
   - Verify connection string:
   ```json
   "DefaultConnection": "Server=localhost;Port=3306;Database=productinv_db;User Id=root;Password=;"
   ```

2. **Update Password** (if may password ang MySQL):
   ```json
   "DefaultConnection": "Server=localhost;Port=3306;Database=productinv_db;User Id=root;Password=your_password;"
   ```

---

### STEP 6: Test Connection

1. **Run the application:**
   ```bash
   cd ProductINV
   dotnet run
   ```

2. **Check for errors:**
   - Kung may database connection error, verify:
     - MySQL service is running
     - Database name is correct
     - Username/password is correct
     - Port 3306 is not blocked

---

## 🔧 TROUBLESHOOTING

### Problem: Cannot connect to MySQL

**Solutions:**
1. Check if MySQL service is running sa XAMPP
2. Verify port 3306 is not used by another application
3. Check firewall settings
4. Verify connection string sa appsettings.json

### Problem: Access denied for user 'root'

**Solutions:**
1. Check password sa appsettings.json
2. Reset MySQL password:
   ```sql
   ALTER USER 'root'@'localhost' IDENTIFIED BY '';
   FLUSH PRIVILEGES;
   ```

### Problem: Database not found

**Solutions:**
1. Verify database name: `productinv_db`
2. Create database manually sa phpMyAdmin
3. Check database name sa connection string

### Problem: Table doesn't exist

**Solutions:**
1. Run SQL script sa `database_setup.sql`
2. Verify table names match sa AppDbContext.cs
3. Check for typos sa SQL script

---

## 📝 DEFAULT XAMPP CONFIGURATION

- **MySQL Host:** `localhost`
- **MySQL Port:** `3306`
- **Default Username:** `root`
- **Default Password:** (blank/empty)
- **phpMyAdmin URL:** `http://localhost/phpMyAdmin`

---

## 🔐 SECURITY NOTES

⚠️ **IMPORTANT:** Default XAMPP configuration ay hindi secure para sa production!

**Para sa Production:**
1. Change MySQL root password
2. Create dedicated database user
3. Limit user permissions
4. Use strong passwords
5. Enable SSL connections

**Example Production User:**
```sql
CREATE USER 'productinv_user'@'localhost' IDENTIFIED BY 'strong_password_here';
GRANT ALL PRIVILEGES ON productinv_db.* TO 'productinv_user'@'localhost';
FLUSH PRIVILEGES;
```

---

## ✅ VERIFICATION CHECKLIST

- [ ] XAMPP MySQL service is running
- [ ] Database `productinv_db` created
- [ ] All tables created successfully
- [ ] appsettings.json configured correctly
- [ ] Application connects to database
- [ ] No connection errors sa console

---

## 📞 NEED HELP?

1. Check XAMPP logs: `C:\xampp\mysql\data\mysql_error.log`
2. Check application logs sa console
3. Verify MySQL service status
4. Test connection using MySQL Workbench or command line

---

**Last Updated:** November 2024  
**XAMPP Version:** Latest  
**MySQL Version:** 8.0+





