# AI Smart Sheet - .NET Backend API

ASP.NET Core 8.0 Web API backend for the AI Smart Sheet timesheet application. This API provides comprehensive endpoints for user management, project tracking, time entry logging, and approval workflows.

## 🏗️ Architecture

- **Framework**: ASP.NET Core 8.0 Web API
- **Database**: PostgreSQL (via Supabase) with Entity Framework Core
- **Authentication**: JWT Bearer tokens with BCrypt password hashing
- **Authorization**: Role-based (Admin, Manager, Developer)
- **Logging**: Serilog with console and file sinks
- **Documentation**: Swagger/OpenAPI
- **Validation**: FluentValidation
- **ORM**: Entity Framework Core with Npgsql

## 📁 Project Structure

```
backend-dotnet/
├── Controllers/          # API endpoint controllers
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── ProjectsController.cs
│   ├── TimeEntriesController.cs
│   ├── ApprovalsController.cs
│   └── DashboardController.cs
├── Models/              # Database entity models
│   ├── User.cs
│   ├── Project.cs
│   ├── TimeEntry.cs
│   ├── Approval.cs
│   ├── UserProject.cs
│   ├── ProjectManager.cs
│   └── AuditLog.cs
├── DTOs/                # Data Transfer Objects
│   └── CommonDTOs.cs
├── Services/            # Business logic services
│   ├── IServices.cs
│   ├── AuthService.cs
│   ├── UserService.cs
│   ├── ProjectService.cs
│   └── StubServices.cs  # To be implemented
├── Data/                # Database context
│   └── AppDbContext.cs
├── Middleware/          # Custom middleware
│   ├── ExceptionHandlingMiddleware.cs
│   └── AuditLoggingMiddleware.cs
├── Program.cs           # Application startup
└── appsettings.json     # Configuration
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL database (Supabase account recommended)
- Your Next.js frontend running on `http://localhost:3000`

### Installation

1. **Navigate to the backend directory**
   ```powershell
   cd backend-dotnet
   ```

2. **Restore dependencies**
   ```powershell
   dotnet restore
   ```

3. **Configure database connection**
   
   Edit `appsettings.json` and update the connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=YOUR_SUPABASE_HOST;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
     },
     "JwtSettings": {
       "SecretKey": "YOUR_SECRET_KEY_MINIMUM_32_CHARACTERS",
       "ExpirationMinutes": 480
     }
   }
   ```

4. **Apply database migrations** (optional - using existing Supabase schema)
   ```powershell
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

   > Note: Since you already have a Supabase database with schema, you can skip migrations and the API will connect to your existing tables.

5. **Run the application**
   ```powershell
   dotnet run
   ```

   The API will start on `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP)

6. **Access Swagger Documentation**
   
   Open your browser to: `http://localhost:5000` or `https://localhost:5001`

## 🔐 Authentication

### Register a new user

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe",
  "role": "developer"
}
```

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "uuid-here",
      "email": "user@example.com",
      "fullName": "John Doe",
      "role": "developer",
      "isActive": true
    },
    "expiresAt": "2025-10-03T12:00:00Z"
  }
}
```

### Using the JWT Token

Include the token in the Authorization header for all protected endpoints:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📋 API Endpoints

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | User login | No |
| GET | `/api/auth/verify` | Verify JWT token | Yes |

### User Management Endpoints

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/users` | Get all users (paginated) | Yes | Admin |
| GET | `/api/users/{id}` | Get user by ID | Yes | All |
| POST | `/api/users` | Create new user | Yes | Admin |
| PUT | `/api/users/{id}` | Update user | Yes | Admin |
| DELETE | `/api/users/{id}` | Delete user | Yes | Admin |
| POST | `/api/users/{id}/deactivate` | Deactivate user | Yes | Admin |
| POST | `/api/users/{id}/activate` | Activate user | Yes | Admin |

### Project Management Endpoints

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/projects` | Get all projects | Yes | All |
| GET | `/api/projects/{id}` | Get project by ID | Yes | All |
| GET | `/api/projects/user/{userId}` | Get user's projects | Yes | All |
| GET | `/api/projects/manager/{managerId}` | Get manager's projects | Yes | Manager+ |
| POST | `/api/projects` | Create new project | Yes | Admin |
| PUT | `/api/projects/{id}` | Update project | Yes | Admin |
| DELETE | `/api/projects/{id}` | Delete project | Yes | Admin |
| POST | `/api/projects/{id}/assign-user` | Assign user to project | Yes | Admin |
| POST | `/api/projects/{id}/remove-user` | Remove user from project | Yes | Admin |
| POST | `/api/projects/{id}/assign-manager` | Assign manager | Yes | Admin |
| GET | `/api/projects/{id}/members` | Get project members | Yes | Manager+ |

### Time Entry Endpoints

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/timeentries` | Get time entries (filtered) | Yes | All |
| GET | `/api/timeentries/{id}` | Get time entry by ID | Yes | All |
| POST | `/api/timeentries` | Create time entry | Yes | All |
| POST | `/api/timeentries/bulk` | Create bulk time entries | Yes | All |
| PUT | `/api/timeentries/{id}` | Update time entry | Yes | Owner |
| DELETE | `/api/timeentries/{id}` | Delete time entry | Yes | Owner |
| POST | `/api/timeentries/{id}/submit` | Submit for approval | Yes | Owner |
| POST | `/api/timeentries/submit-multiple` | Submit multiple entries | Yes | Owner |

### Approval Endpoints

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/approvals/pending` | Get pending approvals | Yes | Manager+ |
| POST | `/api/approvals/manager-approve` | Manager approve entry | Yes | Manager |
| POST | `/api/approvals/admin-approve` | Admin approve entry | Yes | Admin |
| POST | `/api/approvals/reject` | Reject time entry | Yes | Manager+ |
| POST | `/api/approvals/bulk-approve` | Bulk approve entries | Yes | Manager+ |
| GET | `/api/approvals/history/{timeEntryId}` | Get approval history | Yes | All |

### Dashboard Endpoints

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/dashboard/user-stats` | Get user dashboard stats | Yes | All |
| GET | `/api/dashboard/manager-stats` | Get manager dashboard stats | Yes | Manager |
| GET | `/api/dashboard/admin-stats` | Get admin dashboard stats | Yes | Admin |
| GET | `/api/dashboard/user-project-stats` | Get user project stats | Yes | All |
| GET | `/api/dashboard/manager-project-stats` | Get manager project stats | Yes | Manager |
| GET | `/api/dashboard/all-project-stats` | Get all project stats | Yes | Admin |

## 🛠️ Development

### Running in Development Mode

```powershell
dotnet watch run
```

This enables hot reload - changes to code will automatically restart the application.

### Running Tests

```powershell
dotnet test
```

### Building for Production

```powershell
dotnet publish -c Release -o ./publish
```

## 🔧 Configuration

### Environment Variables

You can use environment variables instead of `appsettings.json`:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=...;Database=..."
$env:JwtSettings__SecretKey="your-secret-key"
```

### CORS Configuration

Update `appsettings.json` to add allowed origins:

```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://yourdomain.com"
    ]
  }
}
```

## 📊 Database Schema

The API works with your existing Supabase PostgreSQL schema:

- **users** - User profiles and authentication
- **projects** - Project information
- **user_projects** - User-project assignments
- **time_entries** - Time tracking records
- **approvals** - Two-stage approval workflow
- **project_managers** - Manager-project assignments
- **audit_logs** - Action tracking and audit trail

## 🔒 Security Features

- ✅ JWT Bearer token authentication
- ✅ BCrypt password hashing
- ✅ Role-based authorization (Admin, Manager, Developer)
- ✅ CORS protection
- ✅ SQL injection protection (EF Core parameterization)
- ✅ Audit logging for all actions
- ✅ HTTPS enforcement in production

## 📝 Logging

Logs are written to:
- **Console**: Real-time log output
- **File**: `logs/aismartsheet-YYYY-MM-DD.txt`

Log levels:
- **Debug**: Development only
- **Information**: General information
- **Warning**: Warning messages
- **Error**: Error messages with stack traces

## 🚀 Deployment

### Deploy to Azure App Service

1. Create an Azure App Service (Linux, .NET 8.0)
2. Configure connection strings in Azure Portal
3. Deploy using:
   ```powershell
   dotnet publish -c Release
   # Upload publish folder to Azure
   ```

### Deploy with Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AISmartSheet.API.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AISmartSheet.API.dll"]
```

## 🤝 Integration with Next.js Frontend

Update your Next.js `.env.local`:

```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api
# or for production
NEXT_PUBLIC_API_URL=https://your-api-domain.com/api
```

Example API call from Next.js:

```typescript
// lib/api/client.ts
const API_URL = process.env.NEXT_PUBLIC_API_URL;

export async function login(email: string, password: string) {
  const response = await fetch(`${API_URL}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  return response.json();
}
```

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [JWT Authentication](https://jwt.io)
- [Swagger/OpenAPI](https://swagger.io)

## ⚠️ TODO - Implementation Needed

The following service methods are stubs and need full implementation:

### TimeEntryService
- All methods for time entry CRUD operations
- Bulk operations
- Status transitions

### ApprovalService
- Two-stage approval workflow (Manager → Admin)
- Bulk approval operations
- Approval history tracking

### DashboardService
- User statistics calculation
- Manager dashboard aggregations
- Admin analytics

Refer to `Services/StubServices.cs` for method signatures.

## 📞 Support

For issues or questions:
1. Check the Swagger documentation at `/` endpoint
2. Review logs in `logs/` directory
3. Check database connection and permissions

---

**Built with ❤️ for AI Smart Sheet**
