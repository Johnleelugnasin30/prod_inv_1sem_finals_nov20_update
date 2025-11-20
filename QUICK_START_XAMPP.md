# 🚀 QUICK START GUIDE - XAMPP Setup

## ⚡ FAST SETUP (5 Minutes)

### 1️⃣ Start XAMPP MySQL
```
1. Open XAMPP Control Panel
2. Click "Start" sa MySQL
3. Wait for green indicator
```

### 2️⃣ Create Database sa phpMyAdmin
```
1. Open: http://localhost/phpMyAdmin
2. Click "New" sa left sidebar
3. Database name: productinv_db
4. Collation: utf8mb4_general_ci
5. Click "Create"
```

### 3️⃣ Import SQL Script
```
1. Sa phpMyAdmin, select "productinv_db" database
2. Click "Import" tab
3. Click "Choose File"
4. Select: database_setup.sql
5. Click "Go"
```

### 4️⃣ Configure Application
```
1. Open: ProductINV/appsettings.json
2. Verify connection string:
   "Server=localhost;Port=3306;Database=productinv_db;User Id=root;Password=;"
```

### 5️⃣ Run Application
```bash
cd ProductINV
dotnet run
```

### 6️⃣ Access Application
```
Open browser: http://localhost:5072
```

---

## ✅ VERIFICATION

### Check Database Connection
1. Run application
2. Check console for errors
3. Kung walang error = Connected! ✅

### Test Login
1. Go to Login page
2. Try to register new account
3. Kung successful = Database working! ✅

---

## 🔧 COMMON ISSUES

### ❌ "Cannot connect to MySQL"
**Fix:**
- Check XAMPP MySQL is running
- Verify port 3306 is available
- Check appsettings.json connection string

### ❌ "Access denied for user 'root'"
**Fix:**
- Leave password blank sa appsettings.json
- Or reset MySQL password sa XAMPP

### ❌ "Database doesn't exist"
**Fix:**
- Create database manually sa phpMyAdmin
- Name: `productinv_db`

### ❌ "Table doesn't exist"
**Fix:**
- Import `database_setup.sql` file
- Verify all tables created

---

## 📝 DEFAULT CREDENTIALS

### XAMPP MySQL
- **Host:** localhost
- **Port:** 3306
- **Username:** root
- **Password:** (blank)

### Application
- **URL:** http://localhost:5072
- **Admin Login:** Check database para sa admin credentials

---

## 🎯 NEXT STEPS

1. ✅ Database setup complete
2. ✅ Application running
3. 📝 Create admin account
4. 📝 Add sample products
5. 🚀 Start using the system!

---

**Need Help?** Check `XAMPP_SETUP_GUIDE.md` for detailed instructions.





