# 🛍️ ShopAPI — Clean Architecture ASP.NET Core Project

> A fully structured ASP.NET Core Web API built with Clean Architecture principles, Dapper ORM, and JWT Authentication
---
## 🧠 Project Architecture
```
ShopApi
│
├── 📂 Domain
│   ├── Entities (UserEntity, ProductEntity, OrderEntity, etc.)
│   ├── AdminSettings.cs
│   └── Domain.csproj
│
├── 📂 Application
│   ├── Dto/
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   └── Services/
│   ├── MappingProfiles/
│   ├── Services/
│   └── Application.csproj
│
├── 📂 Infrastructure
│   ├── Repositories/
│   ├── Seed/AdminInitializer.cs
│   ├── NpgsqlConnectionFactory.cs
│   └── Infrastructure.csproj
│
├── 📂 Presentation
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProductController.cs
│   │   ├── OrderController.cs
│   │   └── UserController.cs
│   ├── appsettings.json
│   ├── Program.cs
│   └── Presentation.csproj
│
└── 📜 ShopApi.sln
```
---

## ⚙️ Tech Stack

| Layer | Technologies |
|-------|---------------|
| **Core Framework** | ASP.NET Core 9.0 |
| **ORM** | Dapper |
| **Mapping** | AutoMapper |
| **Database** | PostgreSQL |
| **Authentication** | JWT (Access + Refresh Tokens) |
| **Security** | BCrypt password hashing |
| **Architecture** | Clean Architecture (Domain → Application → Infrastructure → Presentation) |
| **Docs** | Swagger / Swashbuckle |

---

## 🔐 Authentication Flow

- **User Login/Register:** Generates both `AccessToken` and `RefreshToken`.
- **AccessToken:** Used for quick API authorization (valid for 1 hour).
- **RefreshToken:** Stored securely and used to renew tokens without re-login.
- **AdminInitializer:** Automatically seeds an admin user from `appsettings.json` on startup.

---

## 🧩 Main Features

✅ **User Management**
- Register / Login with hashed passwords
- JWT access + refresh tokens
- Role-based authorization (User / Admin)

✅ **Product Management**
- Create / Update / Delete products
- Fetch product list or by ID

✅ **Orders**
- Create orders with multiple products
- Retrieve all user orders with product details

✅ **Admin Features**
- Admin seeded automatically from configuration
- Access protected endpoints with `[Authorize(Roles = "Admin")]`

---
🧑‍💻 Admin Seeding Example

In your appsettings.json:
```
"AdminSettings": {
"Username": "admin",
"Password": "StrongAdminPassword123!",
"Email": "admin@shopapi.com",
"Role": "Admin"
}
```

```bash
    # Clone the repository
    git clone https://github.com/yourusername/ShopApi.git
    
    # Navigate into the project
    cd ShopApi/Presentation
    
    # Run the API
    dotnet run
```

## 🧩 Example appsettings.json (for JWT + DB)
```
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=shopdb;Username=postgres;Password=yourpassword"
},
"JwtSettings": {
  "Key": "supersecretkeyhere12345",
  "Issuer": "ShopApi",
  "Audience": "ShopApiUsers"
}
```

🧠 **Key Principles Followed**
- Clean Separation of Concerns
- Dependency Injection Everywhere
- Repository + Service Patterns
- Asynchronous Programming (async/await)
- Layered Architecture: no circular dependencies
- Minimal Controllers — all logic in Services

⸻

🚀 **Future Improvements**
- Add Redis cache for product queries
- Implement Email verification
- Add integration tests with xUnit + Testcontainers
- Build Blazor or React client

⸻

👨‍💻 Author

Gor Muradkhanyan

📍 Armenia

💼 Junior .NET Developer

💬 “Code should be simple, readable, and logical — like a good story.”
