@echo off
echo ============================================
echo XAMPP Database Setup Script
echo ============================================
echo.

REM Check common XAMPP MySQL paths
set MYSQL_PATH=
if exist "C:\xampp\mysql\bin\mysql.exe" set MYSQL_PATH=C:\xampp\mysql\bin\mysql.exe
if exist "C:\Program Files\xampp\mysql\bin\mysql.exe" set MYSQL_PATH=C:\Program Files\xampp\mysql\bin\mysql.exe

if "%MYSQL_PATH%"=="" (
    echo ERROR: MySQL not found in common XAMPP locations!
    echo.
    echo Please run the SQL script manually via phpMyAdmin:
    echo 1. Open http://localhost/phpMyAdmin
    echo 2. Click "New" to create database "productinv_db"
    echo 3. Select "productinv_db" database
    echo 4. Click "Import" tab
    echo 5. Choose file: database_setup.sql
    echo 6. Click "Go"
    echo.
    pause
    exit /b 1
)

echo Found MySQL at: %MYSQL_PATH%
echo.
echo Creating database and tables...
echo.

REM Run SQL script
"%MYSQL_PATH%" -u root -e "source database_setup.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ============================================
    echo SUCCESS! Database created successfully!
    echo ============================================
    echo.
    echo Database: productinv_db
    echo Tables: users, admin, products, borrow_requests, admin_keys, verificationcodes, password_reset_tokens, audit_logs
    echo.
) else (
    echo.
    echo ERROR: Failed to create database!
    echo.
    echo Please check:
    echo 1. XAMPP MySQL service is running
    echo 2. MySQL root password (if set, update the script)
    echo 3. Or use phpMyAdmin method instead
    echo.
)

pause




