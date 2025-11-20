# CODE STRUCTURE GUIDE - CWTP Asset Inventory System

## 📁 PROJECT OVERVIEW

Ito ay isang **ASP.NET Core Razor Pages** application na ginagamit para sa asset inventory management system. Built gamit ang .NET 8.0 at MySQL database.

---

## 🗂️ FOLDER STRUCTURE

```
ProductINV/
├── Pages/                    # Razor Pages (Main Application Pages)
│   ├── Data/                 # Database Context
│   ├── Model/                # Data Models
│   ├── Shared/               # Shared Layouts & Components
│   └── [PageName].cshtml     # View Files
│   └── [PageName].cshtml.cs  # Page Model Files
├── Services/                 # Business Logic Services
├── wwwroot/                  # Static Files (CSS, JS, Images)
├── Properties/               # Configuration Files
├── Program.cs                # Application Entry Point
├── EmailService.cs           # Email Service
└── ProductINV.csproj         # Project Configuration
```

---

## 📄 CORE FILES

### 1. **Program.cs** (Application Entry Point)
**Location:** `ProductINV/Program.cs`

**Purpose:** Main application configuration at startup

**Key Responsibilities:**
- Configure services (Razor Pages, Sessions, Database, Email)
- Setup middleware pipeline
- Register dependency injection

**Key Components:**
```csharp
- builder.Services.AddRazorPages()          // Razor Pages support
- builder.Services.AddSession()               // Session management
- builder.Services.AddDbContext<AppDbContext>() // Database context
- builder.Services.AddScoped<EmailService>()  // Email service
```

**Middleware Order:**
1. Exception Handler (Production)
2. HTTPS Redirection
3. Static Files
4. Routing
5. Session
6. Authorization
7. Razor Pages Mapping

---

### 2. **AppDbContext.cs** (Database Context)
**Location:** `ProductINV/Pages/Data/AppDbContext.cs`

**Purpose:** Entity Framework database context

**Key Responsibilities:**
- Define DbSets para sa lahat ng tables
- Configure table mappings
- Handle database connections

**DbSets:**
- `Products` → `products` table
- `Users` → `users` table
- `Admins` → `admin` table
- `AdminKeys` → `admin_keys` table
- `VerificationCodes` → `verificationcodes` table
- `PasswordResetTokens` → `password_reset_tokens` table
- `AuditLogs` → `audit_logs` table
- `BorrowRequests` → `borrow_requests` table

---

## 📋 PAGES STRUCTURE

### Authentication Pages

#### **Login.cshtml / Login.cshtml.cs**
**Location:** `Pages/Login.cshtml`

**Purpose:** User at Admin login page

**Features:**
- Tab switcher para sa User/Admin login
- Email verification (OTP) para sa users
- Admin key validation
- Session management
- Password toggle visibility
- Help Center modal
- Terms & Conditions modal
- Forgot Password modal

**Handlers:**
- `OnPostUserLogin()` - User login with email verification
- `OnPostAdminLogin()` - Admin login with key validation
- `OnPostVerifyEmail()` - Email OTP verification

**Session Variables:**
- `Username` - Current logged-in username
- `UserRole` - "User" or "Admin"
- `PendingUserId` - For email verification flow
- `VerificationCode` - OTP code

---

#### **Register.cshtml / Register.cshtml.cs**
**Location:** `Pages/Register.cshtml`

**Purpose:** New user registration

**Features:**
- Auto-generated ID Number (format: 21-0001)
- Terms & Conditions checkbox (required)
- Email validation
- Username uniqueness check
- Password hashing (BCrypt)
- Beautiful modal UI

**Handlers:**
- `OnGet()` - Generate next ID number
- `OnPostCreateAccount()` - Create new user account

**Validation:**
- All required fields
- Email format
- Username uniqueness
- Email uniqueness
- Terms & Conditions acceptance

---

#### **Logout.cshtml.cs**
**Location:** `Pages/Logout.cshtml.cs`

**Purpose:** User logout functionality

**Features:**
- Clear all session data
- Redirect to login page

---

### User Pages

#### **UserView.cshtml / UserView.cshtml.cs**
**Location:** `Pages/UserView.cshtml`

**Purpose:** Main user dashboard

**Features:**
- **Dashboard Tab** - Overview statistics
- **Product Availability** - Browse available products
- **Product Information** - Search and view product details
- **QR Generated** - Generate QR codes for products
- **Borrow Form** - Submit borrow requests
- **Profile Dashboard** - View account information
- **Login History** - View login sessions
- **Light/Dark Mode** - Theme toggle

**Handlers:**
- `OnGet()` - Load all data
- `OnGetSearchProduct()` - Search products
- `OnGetGenerateQR()` - Generate QR code
- `OnPostBorrowRequest()` - Submit borrow request
- `OnPostLogout()` - Logout

**Dependencies:**
- `AppDbContext` - Database access
- `EmailService` - Email notifications

---

### Admin Pages

#### **AdminDashboard.cshtml / AdminDashboard.cshtml.cs**
**Location:** `Pages/AdminDashboard.cshtml`

**Purpose:** Comprehensive admin dashboard

**Features:**
- **Manage Products** - CRUD operations for products
- **Manage Accounts** - CRUD operations for users
- **Generate QR** - Generate QR codes
- **View Reports** - Statistics and reports
- **Borrow Requests** - Approve/Reject requests
- **Login History** - View all login activities
- **Light/Dark Mode** - Theme toggle

**Handlers:**
- `OnGet()` - Load all data
- `OnPostCreateProduct()` - Create product
- `OnPostUpdateProduct()` - Update product
- `OnPostDeleteProduct()` - Delete product
- `OnPostCreateUser()` - Create user
- `OnPostUpdateUser()` - Update user
- `OnPostDeleteUser()` - Delete user
- `OnGetGenerateQR()` - Generate QR code
- `OnPostApproveBorrowRequest()` - Approve request
- `OnPostRejectBorrowRequest()` - Reject request
- `OnPostLogout()` - Logout

---

#### **ManageProduct.cshtml / ManageProduct.cshtml.cs**
**Location:** `Pages/ManageProduct.cshtml`

**Purpose:** Legacy product management (using raw SQL)

**Note:** This uses MySqlConnection directly instead of Entity Framework

**Features:**
- Create products
- Update products
- Delete products
- List all products

---

#### **AddItem.cshtml / AddItem.cshtml.cs**
**Location:** `Pages/AddItem.cshtml`

**Purpose:** Quick add item page

**Features:**
- Simple form to add products
- Navigation back to home

---

### Other Pages

#### **Index.cshtml / Index.cshtml.cs**
**Location:** `Pages/Index.cshtml`

**Purpose:** Main dashboard landing page

**Features:**
- Welcome dashboard
- Navigation cards
- Light/Dark mode
- Session check (redirects to login if not authenticated)

---

#### **Error.cshtml / Error.cshtml.cs**
**Location:** `Pages/Error.cshtml`

**Purpose:** Error page handler

**Features:**
- Display error messages
- Error logging

---

#### **Privacy.cshtml / Privacy.cshtml.cs**
**Location:** `Pages/Privacy.cshtml`

**Purpose:** Privacy policy page

---

#### **Product.cshtml / Product.cshtml.cs**
**Location:** `Pages/Product.cshtml`

**Purpose:** Individual product view page

---

## 🎨 SHARED COMPONENTS

### **Shared/_Layout.cshtml**
**Location:** `Pages/Shared/_Layout.cshtml`

**Purpose:** Main layout template

**Features:**
- Common HTML structure
- Bootstrap integration
- Animated gradient background
- Responsive design

---

### **_ViewStart.cshtml**
**Location:** `Pages/_ViewStart.cshtml`

**Purpose:** Sets default layout for all pages

---

### **_ViewImports.cshtml**
**Location:** `Pages/_ViewImports.cshtml`

**Purpose:** Import common namespaces and tag helpers

---

### **_ValidationScriptsPartial.cshtml**
**Location:** `Pages/Shared/_ValidationScriptsPartial.cshtml`

**Purpose:** Client-side validation scripts

---

## 📦 MODELS

### Location: `Pages/Model/`

#### **User.cs**
**Purpose:** User account model

**Properties:**
- Id, FullName, Email, Username
- PasswordHash (BCrypt)
- Position, IdNumber
- Role ("User" or "Admin")
- IsEmailVerified, IsIdVerified, IsActive
- CreatedAt, UpdatedAt, LastLogin

---

#### **Products.cs**
**Purpose:** Product inventory model

**Properties:**
- Id, ProductId, Name, Description
- Category, Price, Quantity
- Location, SerialNumber
- ConditionStatus, IsAvailable
- ImageUrl, QrCodeUrl
- CreatedBy, CreatedAt, UpdatedAt

---

#### **BorrowRequest.cs**
**Purpose:** Borrow request model

**Properties:**
- Id, UserId, ProductId, ProductName
- Purpose, RequestDate
- BorrowDate, ReturnDate
- Status ("Pending", "Approved", "Rejected", etc.)
- AdminNotes
- CreatedAt, UpdatedAt

---

#### **Admin.cs**
**Purpose:** Admin account model

**Properties:**
- Id, Username, PasswordHash

---

#### **AdminKey.cs**
**Purpose:** Admin access key model

**Properties:**
- Id, key_hash, is_active, created_at

---

#### **VerificationCode.cs**
**Purpose:** Email verification code model

**Properties:**
- Id, Email, Code
- Type, ExpiresAt, IsUsed, CreatedAt

---

#### **PasswordResetToken.cs**
**Purpose:** Password reset token model

**Properties:**
- Id, UserId, Token
- ExpiresAt, IsUsed, CreatedAt

---

#### **AuditLog.cs**
**Purpose:** System activity log model

**Properties:**
- Id, UserId, Action
- TableName, RecordId
- OldValues, NewValues
- IpAddress, CreatedAt

---

## 🔧 SERVICES

### **EmailService.cs**
**Location:** `ProductINV/EmailService.cs`

**Purpose:** Email sending service

**Features:**
- Send verification codes
- Send password reset emails
- Send borrow request notifications
- SMTP configuration (Gmail)

**Methods:**
- `SendVerificationCodeAsync()` - Send OTP codes
- Uses SMTP client with Gmail credentials

---

### **UserServices.cs**
**Location:** `ProductINV/Services/UserServices.cs`

**Purpose:** User-related business logic

**Note:** May existing service file pero hindi actively ginagamit sa current implementation

---

## 🎨 STATIC FILES (wwwroot/)

### **CSS Files**
**Location:** `wwwroot/css/`

- `site.css` - Main site styles
- `login.css` - Login page specific styles

### **JavaScript Files**
**Location:** `wwwroot/js/`

- `site.js` - Main site scripts
- `login.js` - Login page scripts

### **Images**
**Location:** `wwwroot/`

- Logo files (cwtp_tr.png, Logo.jpg)
- User images (charles.jpg, cla.jpg, etc.)

### **Libraries**
**Location:** `wwwroot/lib/`

- **Bootstrap** - CSS framework
- **jQuery** - JavaScript library
- **jQuery Validation** - Form validation
- **jQuery Validation Unobtrusive** - ASP.NET validation

---

## 🔐 SECURITY FEATURES

### **Password Security**
- All passwords are hashed using **BCrypt**
- Password hashing: `BCrypt.Net.BCrypt.HashPassword()`
- Password verification: `BCrypt.Net.BCrypt.Verify()`

### **Session Management**
- Session timeout: 30 minutes
- HttpOnly cookies (XSS protection)
- Session-based authentication
- Role-based access control

### **Email Verification**
- OTP (6-digit code) sent via email
- Code expiration (10 minutes)
- One-time use codes

### **Admin Security**
- Admin key required for admin login
- Separate admin table
- Admin key validation

---

## 🎯 CODE PATTERNS

### **Razor Pages Pattern**
```csharp
public class PageNameModel : PageModel
{
    private readonly AppDbContext _db;
    
    public PageNameModel(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task OnGet()
    {
        // Load data
    }
    
    public async Task<IActionResult> OnPostActionName()
    {
        // Handle POST request
        return Page();
    }
}
```

### **Handler Naming Convention**
- `OnGet()` - GET request handler
- `OnPost[ActionName]()` - POST request handler
- `OnGet[ActionName]()` - GET with query parameters

### **Dependency Injection**
- Constructor injection para sa services
- Scoped services para sa database context
- Session access via `HttpContext.Session`

### **Error Handling**
- Try-catch blocks sa handlers
- Error messages sa `ErrorMessage` property
- Success messages sa `SuccessMessage` property
- User-friendly error display

---

## 📱 UI/UX FEATURES

### **Theme System**
- Light/Dark mode toggle
- LocalStorage para sa theme persistence
- CSS variables para sa theming
- Smooth transitions

### **Responsive Design**
- Mobile-first approach
- Bootstrap grid system
- Flexible layouts
- Touch-friendly buttons

### **Animations**
- Gradient background animations
- Fade-in effects
- Hover transitions
- Loading states

### **Modals**
- Help Center modal
- Terms & Conditions modal
- Email Verification modal
- Forgot Password modal

---

## 🔄 DATA FLOW

### **User Registration Flow**
1. User fills registration form
2. Terms & Conditions validation
3. Email/Username uniqueness check
4. Password hashing
5. User creation sa database
6. Redirect to login

### **User Login Flow**
1. User enters email/password
2. Credential validation
3. OTP generation
4. Email sent with OTP
5. User enters OTP
6. OTP verification
7. Session creation
8. Redirect to UserView

### **Admin Login Flow**
1. Admin enters username/password/admin key
2. Credential validation
3. Admin key validation
4. Session creation
5. Redirect to AdminDashboard

### **Borrow Request Flow**
1. User selects product
2. Fills borrow form
3. Request submission
4. Email notification to admin
5. Admin approves/rejects
6. Status update
7. Email notification to user

---

## 🗄️ DATABASE ACCESS PATTERNS

### **Entity Framework (Recommended)**
```csharp
// Using AppDbContext
var products = await _db.Products
    .Where(p => p.IsAvailable)
    .ToListAsync();
```

### **Raw SQL (Legacy)**
```csharp
// Using MySqlConnection (ManageProduct.cshtml.cs)
using (MySqlConnection conn = new MySqlConnection(connStr))
{
    conn.Open();
    // SQL queries
}
```

**Note:** New code should use Entity Framework. Raw SQL is only sa legacy ManageProduct page.

---

## 📝 NAMING CONVENTIONS

### **Files**
- Page files: `[PageName].cshtml` / `[PageName].cshtml.cs`
- Model files: `[ModelName].cs`
- Service files: `[ServiceName].cs`

### **Classes**
- Page Models: `[PageName]Model : PageModel`
- Models: `[ModelName]`
- Services: `[ServiceName]`

### **Properties**
- Public properties: PascalCase
- Private fields: camelCase with underscore prefix
- BindProperty: PascalCase

### **Methods**
- Handlers: `OnGet()`, `OnPost[Action]()`
- Private methods: PascalCase
- Async methods: `[MethodName]Async()`

---

## 🚀 DEPLOYMENT CHECKLIST

### **Configuration Files**
- [ ] `appsettings.json` - Database connection string
- [ ] `launchSettings.json` - Application URLs
- [ ] Email credentials configuration

### **Database Setup**
- [ ] Create database
- [ ] Run migrations (if using EF Migrations)
- [ ] Seed initial data (admin accounts, etc.)

### **Environment Variables**
- [ ] Database connection string
- [ ] Email SMTP credentials
- [ ] Application URLs

### **Security**
- [ ] Change default passwords
- [ ] Configure HTTPS
- [ ] Set up admin keys
- [ ] Review session timeout

---

## 🐛 DEBUGGING GUIDE

### **Common Issues**

#### **Session Not Working**
- Check `app.UseSession()` sa Program.cs
- Verify session middleware order
- Check cookie settings

#### **Database Connection Failed**
- Verify connection string sa appsettings.json
- Check MySQL server status
- Verify database exists
- Check user permissions

#### **Email Not Sending**
- Verify SMTP credentials
- Check Gmail app password
- Verify network connectivity
- Check spam folder

#### **Page Not Found**
- Verify `@page` directive sa cshtml file
- Check route configuration
- Verify file naming convention

---

## 📚 DEPENDENCIES

### **NuGet Packages**
- `BCrypt.Net-Next` (4.0.3) - Password hashing
- `MailKit` (4.14.1) - Email sending
- `MySql.Data` (9.4.0) - MySQL connector
- `MySql.EntityFrameworkCore` (9.0.6) - EF MySQL support
- `Pomelo.EntityFrameworkCore.MySql` (9.0.0) - EF MySQL provider

### **Frontend Libraries**
- Bootstrap 5.x - CSS framework
- jQuery 3.x - JavaScript library
- jQuery Validation - Form validation

---

## 🔍 CODE NAVIGATION TIPS

### **Finding Code**
1. **Page Logic** → `Pages/[PageName].cshtml.cs`
2. **Page View** → `Pages/[PageName].cshtml`
3. **Data Models** → `Pages/Model/[ModelName].cs`
4. **Database Context** → `Pages/Data/AppDbContext.cs`
5. **Services** → `Services/[ServiceName].cs` or root level
6. **Configuration** → `Program.cs` or `appsettings.json`

### **Understanding Flow**
1. Start sa `Program.cs` para sa configuration
2. Check `AppDbContext.cs` para sa database structure
3. Review page models para sa business logic
4. Check views para sa UI implementation

---

## 📖 BEST PRACTICES

### **Code Organization**
- ✅ Keep page models focused on single responsibility
- ✅ Use dependency injection
- ✅ Separate concerns (Models, Services, Pages)
- ✅ Use async/await para sa database operations
- ✅ Handle errors gracefully

### **Security**
- ✅ Always hash passwords
- ✅ Validate user input
- ✅ Use parameterized queries
- ✅ Check user permissions
- ✅ Use HTTPS in production

### **Performance**
- ✅ Use async methods
- ✅ Load only needed data
- ✅ Use pagination for large lists
- ✅ Cache static data when appropriate

### **Maintainability**
- ✅ Write clear comments
- ✅ Use meaningful names
- ✅ Follow naming conventions
- ✅ Keep methods small and focused
- ✅ Document complex logic

---

## 🎓 LEARNING RESOURCES

### **ASP.NET Core Razor Pages**
- Official Microsoft Documentation
- Razor Pages Tutorial
- Entity Framework Core Guide

### **MySQL**
- MySQL Documentation
- Entity Framework MySQL Provider

### **Security**
- OWASP Top 10
- ASP.NET Core Security Best Practices

---

## 📞 SUPPORT

Para sa questions o issues:
1. Check error messages sa console
2. Review logs
3. Check database connection
4. Verify configuration files
5. Review code structure guide

---

**Last Updated:** November 2024  
**Framework:** ASP.NET Core 8.0  
**Database:** MySQL 8.0+  
**Architecture:** Razor Pages (MVC Pattern)

