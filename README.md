<!-- Project Badges -->
<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8.0" />
  <img src="https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C# 12" />
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/Entity%20Framework-8.0-512BD4?style=for-the-badge&logo=nuget&logoColor=white" alt="Entity Framework Core" />
  <img src="https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" alt="JWT Auth" />
  <img src="https://img.shields.io/badge/Firebase-FCM-FFCA28?style=for-the-badge&logo=firebase&logoColor=black" alt="Firebase FCM" />
  <img src="https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" alt="Swagger" />
</p>

<!-- Project Title -->
<h1 align="center">🔗 LinkWear — Wasl API</h1>

<p align="center">
  <strong>B2B Supply Chain Backend for the Fashion & Textile Industry</strong>
</p>

<p align="center">
  Wasl (وصل — "Link") is a production-ready RESTful API that connects retail <strong>store owners</strong> with <strong>fabric and clothing suppliers</strong> through a structured digital marketplace. Built with ASP.NET Core 8.0 and Clean Architecture, it provides supplier discovery, product catalog management, order lifecycle handling, price negotiation, shipment tracking, and real-time push notifications — all tailored to the Middle Eastern textile supply chain.
</p>

---

## 📑 Table of Contents

- [Overview](#-overview)
- [Key Highlights](#-key-highlights)
- [Features](#-features)
- [Architecture](#-architecture)
- [Technologies Used](#-technologies-used)
- [Installation & Setup](#-installation--setup)
- [Configuration](#-configuration)
- [API Endpoints](#-api-endpoints)
- [Contributing](#-contributing)

---

## 🔭 Overview

### What is Wasl?

Wasl is a comprehensive B2B backend platform designed to digitize wholesale fashion procurement. It replaces phone calls, manual catalogs, and informal agreements with a structured API that handles the full supplier-to-store workflow.

### Business Domain

The platform operates in the **wholesale fashion & textile domain**, enabling:
- **Store Owners** to discover suppliers, browse products, place orders, and track shipments
- **Suppliers** to manage catalogs, receive orders, submit price offers, and update delivery status
- **Admins** to oversee users, approve suppliers, and monitor all system orders

### Problem Solved

- **Fragmented Discovery** — Store owners can now search and compare approved suppliers digitally
- **Manual Order Processes** — End-to-end order lifecycle from placement to delivery is fully automated
- **No Tracking Visibility** — Granular shipment tracking with full history per order
- **Communication Gaps** — Push notifications keep all parties informed in real-time

---

## ✨ Key Highlights

- 🔐 **Enterprise-Grade Security** — JWT authentication with access/refresh token rotation, email confirmation, and password reset flows
- 🏗️ **Clean Architecture** — Strict separation across Domain, Application, Infrastructure, and Presentation layers
- 📦 **Full Order Lifecycle** — Pending → Price Offer → Payment → Shipped → Delivered with cancellation support
- 🚚 **Shipment Tracking** — 9-stage tracking pipeline (Placed → Under Review → Awaiting Payment → Paid → Picked Up → In Transit → Delivered → Failed → Returned)
- 🔔 **Firebase Push Notifications** — Real-time FCM notifications for order events, tracking, approvals, and payments
- 📧 **Email Integration** — MailKit-based transactional emails for confirmations and password resets
- ✅ **Input Validation** — FluentValidation for robust request validation
- 🗺️ **Object Mapping** — AutoMapper for clean DTO transformations
- 📊 **Specification Pattern** — Flexible, reusable query specifications for data filtering
- 🔄 **Unit of Work** — Transactional consistency across repository operations
- 📝 **API Documentation** — Swagger/OpenAPI with JWT authorization support

---

## 🚀 Features

### Authentication & Authorization

| Feature | Description |
|---------|-------------|
| User Registration | Registration with email confirmation and role selection (StoreOwner / Supplier) |
| Login / Logout | Secure JWT-based authentication |
| Refresh Tokens | Silent token renewal with configurable expiration |
| Email Confirmation | Token-based email verification flow |
| Password Reset | Forgot / reset password via email tokens |

### Supplier Discovery

| Feature | Description |
|---------|-------------|
| Browse Suppliers | List all approved suppliers with optional search |
| Supplier Details | View supplier profile, business info, and city |
| Supplier Products | Browse a supplier's full product catalog |
| Activity Filtering | Filter by category: Fabrics, Ready-Made, Men's Wear, Traditional, Accessories |

### Product Management (Supplier)

| Feature | Description |
|---------|-------------|
| CRUD Operations | Create, read, update, and delete products |
| Inventory Control | Track available quantity and minimum order thresholds |
| Pricing | Set and update product pricing |

### Order Lifecycle

| Feature | Description |
|---------|-------------|
| Create Order | Store owners place orders selecting supplier and products |
| Price Offers | Suppliers submit price offers on pending orders |
| Payment Confirmation | Store owners confirm payment to advance order status |
| Cancel Order | Store owners can cancel pending orders |
| Order Details | Both parties can view full order breakdown |
| Status Filtering | Query orders by status (Pending, Paid, Shipped, Delivered, Cancelled) |

### Shipment Tracking

| Feature | Description |
|---------|-------------|
| Tracking History | Full timeline per order with status descriptions |
| 9-Stage Pipeline | OrderPlaced → UnderReview → AwaitingPayment → Paid → PickedUp → InTransit → Delivered → FailedDelivery → Returned |
| Status Updates | Suppliers update tracking with delivery details |

### Notifications

| Feature | Description |
|---------|-------------|
| FCM Push Notifications | Real-time device notifications via Firebase |
| Notification Feed | Paginated notification retrieval |
| Unread Count | Get count of unread notifications |
| Mark as Read | Mark individual or all notifications as read |
| Event Types | OrderCreated, StatusUpdated, TrackingUpdated, SupplierApproved/Rejected, Payment events |

### Dashboards

| Feature | Description |
|---------|-------------|
| Store Owner Dashboard | Order statistics and activity overview |
| Supplier Dashboard | Product count, order counts, and recent orders |

### Administration

| Feature | Description |
|---------|-------------|
| User Management | View all users, soft-delete accounts |
| Order Monitoring | System-wide order oversight |
| Supplier Approval | Approval workflow for new supplier registrations |

---

## 🏛️ Architecture

Wasl follows **Clean Architecture** with strict dependency inversion:

```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTATION                           │
│                       (Wasl.Api)                            │
│    Controllers  ·  Middleware  ·  Extensions  ·  Errors     │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                     INFRASTRUCTURE                          │
│                  (Wasl.Infrastructure)                      │
│   Services · Repositories · Persistence · Specifications   │
│       UnitOfWork · Settings · EF Configurations             │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                      APPLICATION                            │
│                   (Wasl.Application)                        │
│    DTOs · Interfaces · Validation · Mapping · Response      │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                        DOMAIN                               │
│                     (Wasl.Domain)                           │
│  Entities: ApplicationUser, Product, Order, OrderItem,     │
│  ShipmentTrackingHistory, Notifications, RefreshToken,     │
│  FcmToken  ·  Enums  ·  Exceptions  ·  BaseEntity         │
└─────────────────────────────────────────────────────────────┘
```

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Core entities, enums, base classes. Zero external dependencies. |
| **Application** | DTOs, service interfaces, validation rules, mapping profiles. Defines contracts. |
| **Infrastructure** | Service implementations, EF Core, Identity, JWT, Email, FCM, repositories. |
| **Presentation** | REST controllers, middleware, Swagger config, program entry point. |

---

## 🛠️ Technologies Used

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Core runtime |
| ASP.NET Core | 8.0 | Web API framework |
| Entity Framework Core | 8.0 | ORM with Code-First migrations |
| SQL Server | — | Relational database |
| ASP.NET Core Identity | 8.0 | User/role management |
| JWT Bearer | 8.0 | Token-based authentication |
| Firebase Admin SDK | 3.4.0 | FCM push notifications |
| AutoMapper | 13.0.1 | Object mapping |
| FluentValidation | 12.1.1 | Request validation |
| MailKit | 4.15.0 | SMTP email service |
| Swashbuckle | 6.6.2 | Swagger / OpenAPI docs |

---

## 🚀 Installation & Setup

### Prerequisites

| Requirement | Version |
|-------------|---------|
| .NET SDK | 8.0+ |
| SQL Server | 2019+ or LocalDB |
| Firebase Project | Service account credentials |

### 1. Clone & Restore

```bash
git clone https://github.com/Ahmed-Abdulrahim/LinkWear.git
cd LinkWear
dotnet restore
```

### 2. Configure Database & Settings

Update `Wasl.Api/appsettings.json` with your connection string and credentials (see [Configuration](#-configuration)).

### 3. Set Firebase Credentials

```bash
export FIREBASE_CREDENTIALS='{"type":"service_account", ...}'
```

### 4. Run

```bash
dotnet run --project Wasl.Api
```

Migrations are applied automatically on startup. Default seed accounts are created on first run.

### Default Seed Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@wasl.com` | `P@ssw0rd` |
| StoreOwner | `storeowner@wasl.com` | `P@ssw0rd` |
| Supplier | `supplier@wasl.com` | `P@ssw0rd` |

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "conn1": "Server=YOUR_SERVER;Database=WaslDb;User Id=sa;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "Wasl.Api",
    "Audience": "Wasl.Client",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "SenderEmail": "noreply@wasl.com",
    "SenderName": "LinkWear",
    "UseSsl": true,
    "BaseUrl": "https://your-deployed-url.com"
  }
}
```

### Environment Variables

| Variable | Description |
|----------|-------------|
| `FIREBASE_CREDENTIALS` | Firebase service account JSON string |

---

## 📡 API Endpoints

### Authentication

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `POST` | `/api/Authentication/register` | Register a new user | ❌ |
| `POST` | `/api/Authentication/Login` | Authenticate and get tokens | ❌ |
| `GET` | `/api/Authentication/confirm-email` | Confirm email with token | ❌ |
| `POST` | `/api/Authentication/refresh-token` | Refresh access token | ❌ |
| `POST` | `/api/Authentication/Forget-password` | Request password reset | ❌ |
| `POST` | `/api/Authentication/reset-password` | Reset password with token | ❌ |

### Account

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/Account/Get-Profile` | Get current user profile | ✅ |
| `PUT` | `/api/Account/Update-Profile` | Update profile | ✅ |
| `POST` | `/api/Account/Change-Password` | Change password | ✅ |
| `POST` | `/api/Account/Logout` | Logout | ✅ |

### Products

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/Product` | Get all products | ✅ |
| `GET` | `/api/Product/{id}` | Get product by ID | ✅ |
| `POST` | `/api/Product/addProduct` | Add product (Supplier) | ✅ |
| `PUT` | `/api/Product` | Update product (Supplier) | ✅ |
| `DELETE` | `/api/Product/{id}` | Delete product (Supplier) | ✅ |

### Orders (Store Owner)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `POST` | `/api/Order` | Create a new order | ✅ |
| `GET` | `/api/Order` | Get orders (filter by status) | ✅ |
| `GET` | `/api/Order/{id}` | Get order details | ✅ |
| `PUT` | `/api/Order/cancelOrder/{id}` | Cancel a pending order | ✅ |
| `PUT` | `/api/Order/{id}/confirm-payment` | Confirm payment | ✅ |
| `GET` | `/api/Order/dashboard` | Store owner dashboard stats | ✅ |

### Supplier Management

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/Supplier/dashboard` | Supplier dashboard stats | ✅ |
| `GET` | `/api/Supplier/orders` | Get supplier's orders | ✅ |
| `GET` | `/api/Supplier/orders/pending` | Get pending orders | ✅ |
| `PUT` | `/api/Supplier/orders/{id}/price-offer` | Submit price offer | ✅ |
| `PUT` | `/api/Supplier/orders/{id}/status` | Update order status | ✅ |
| `PUT` | `/api/Supplier/orders/{id}/tracking` | Update shipment tracking | ✅ |

### Supplier Discovery

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/Suppliers` | Browse approved suppliers | ✅ |
| `GET` | `/api/Suppliers/{id}` | Get supplier details | ✅ |
| `GET` | `/api/Suppliers/{id}/products` | Get supplier's products | ✅ |

### Notifications

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `POST` | `/api/notifications/token` | Register FCM device token | ✅ |
| `GET` | `/api/notifications` | Get notifications (paginated) | ✅ |
| `GET` | `/api/notifications/unread-count` | Get unread count | ✅ |
| `PUT` | `/api/notifications/{id}/read` | Mark as read | ✅ |
| `PUT` | `/api/notifications/read-all` | Mark all as read | ✅ |

### Admin

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/Admin/getusers` | Get all users | ✅ |
| `DELETE` | `/api/Admin/{userId}` | Soft-delete a user | ✅ |
| `GET` | `/api/Admin/orders` | Get all system orders | ✅ |

> 📝 Full interactive documentation available at `/swagger` when running.

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

```bash
# Fork & clone
git checkout -b feature/YourFeature
git commit -m "feat: add YourFeature"
git push origin feature/YourFeature
# Open a Pull Request
```

---

<p align="center">
  Built with ❤️ by <a href="https://github.com/Ahmed-Abdulrahim"><strong>Ahmed Abdulrahim</strong></a>
</p>
