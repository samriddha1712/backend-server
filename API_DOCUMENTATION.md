# AI Smart Sheet API - Complete Endpoint Documentation

## Base URL
```
Development: http://localhost:5000/api
Production: https://your-domain.com/api
```

## Authentication

All protected endpoints require a JWT Bearer token in the Authorization header:

```http
Authorization: Bearer {your-jwt-token}
```

---

## 📋 Complete API Reference

### 🔐 Authentication Endpoints

#### 1. Register New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe",
  "role": "developer"  // developer, manager, or admin
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@example.com",
    "fullName": "John Doe",
    "role": "developer",
    "isActive": true,
    "createdAt": "2025-10-02T10:00:00Z",
    "updatedAt": "2025-10-02T10:00:00Z"
  }
}
```

#### 2. Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "user@example.com",
      "fullName": "John Doe",
      "role": "developer",
      "isActive": true,
      "createdAt": "2025-10-02T10:00:00Z",
      "updatedAt": "2025-10-02T10:00:00Z"
    },
    "expiresAt": "2025-10-02T18:00:00Z"
  }
}
```

#### 3. Verify Token
```http
GET /api/auth/verify
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Token is valid",
  "data": {
    "email": "user@example.com",
    "name": "John Doe",
    "role": "developer",
    "authenticated": true
  }
}
```

---

### 👥 User Management Endpoints

#### 1. Get All Users (Paginated)
```http
GET /api/users?page=1&pageSize=20&search=john&role=developer
Authorization: Bearer {admin-token}
```

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 20)
- `search` (optional): Search by name or email
- `role` (optional): Filter by role (admin, manager, developer)

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "data": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440000",
        "email": "user@example.com",
        "fullName": "John Doe",
        "role": "developer",
        "isActive": true,
        "createdAt": "2025-10-02T10:00:00Z",
        "updatedAt": "2025-10-02T10:00:00Z"
      }
    ],
    "page": 1,
    "pageSize": 20,
    "totalCount": 45,
    "totalPages": 3
  }
}
```

#### 2. Get User by ID
```http
GET /api/users/{userId}
Authorization: Bearer {token}
```

#### 3. Create New User
```http
POST /api/users
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "email": "newuser@example.com",
  "fullName": "Jane Smith",
  "password": "SecurePassword123!",
  "role": "developer"
}
```

#### 4. Update User
```http
PUT /api/users/{userId}
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "fullName": "Jane Smith Updated",
  "role": "manager",
  "isActive": true
}
```

#### 5. Delete User
```http
DELETE /api/users/{userId}
Authorization: Bearer {admin-token}
```

#### 6. Deactivate User
```http
POST /api/users/{userId}/deactivate
Authorization: Bearer {admin-token}
```

#### 7. Activate User
```http
POST /api/users/{userId}/activate
Authorization: Bearer {admin-token}
```

---

### 📁 Project Management Endpoints

#### 1. Get All Projects
```http
GET /api/projects?page=1&pageSize=20&search=webapp&status=active
Authorization: Bearer {token}
```

**Query Parameters:**
- `page` (optional): Page number
- `pageSize` (optional): Items per page
- `search` (optional): Search by project name or client
- `status` (optional): Filter by status (active, inactive, completed)

#### 2. Get User's Projects
```http
GET /api/projects/user/{userId}
Authorization: Bearer {token}
```

#### 3. Get Manager's Projects
```http
GET /api/projects/manager/{managerId}
Authorization: Bearer {manager-token}
```

#### 4. Get Project by ID
```http
GET /api/projects/{projectId}
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440000",
    "name": "E-Commerce Website",
    "description": "Building a new e-commerce platform",
    "client": "Acme Corp",
    "status": "active",
    "hourlyRate": 150.00,
    "createdBy": "550e8400-e29b-41d4-a716-446655440000",
    "createdAt": "2025-09-01T10:00:00Z",
    "updatedAt": "2025-10-02T10:00:00Z",
    "creator": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "admin@example.com",
      "fullName": "Admin User",
      "role": "admin"
    }
  }
}
```

#### 5. Create Project
```http
POST /api/projects
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "name": "New Project",
  "description": "Project description",
  "client": "Client Name",
  "status": "active",
  "hourlyRate": 150.00
}
```

#### 6. Update Project
```http
PUT /api/projects/{projectId}
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "name": "Updated Project Name",
  "status": "completed"
}
```

#### 7. Delete Project
```http
DELETE /api/projects/{projectId}
Authorization: Bearer {admin-token}
```

#### 8. Assign User to Project
```http
POST /api/projects/{projectId}/assign-user
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "projectId": "660e8400-e29b-41d4-a716-446655440000"
}
```

#### 9. Remove User from Project
```http
POST /api/projects/{projectId}/remove-user
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "projectId": "660e8400-e29b-41d4-a716-446655440000"
}
```

#### 10. Get Project Members
```http
GET /api/projects/{projectId}/members
Authorization: Bearer {manager-token}
```

---

### ⏱️ Time Entry Endpoints

#### 1. Get Time Entries (Filtered)
```http
GET /api/timeentries?userId={userId}&projectId={projectId}&startDate=2025-10-01&endDate=2025-10-31&status=approved&page=1&pageSize=20
Authorization: Bearer {token}
```

**Query Parameters:**
- `userId` (optional): Filter by user
- `projectId` (optional): Filter by project
- `startDate` (optional): Start date (ISO format)
- `endDate` (optional): End date (ISO format)
- `status` (optional): Filter by status
- `page` (optional): Page number
- `pageSize` (optional): Items per page

#### 2. Get Time Entry by ID
```http
GET /api/timeentries/{timeEntryId}
Authorization: Bearer {token}
```

#### 3. Create Time Entry
```http
POST /api/timeentries
Authorization: Bearer {token}
Content-Type: application/json

{
  "projectId": "660e8400-e29b-41d4-a716-446655440000",
  "description": "Worked on user authentication",
  "category": "Development",
  "hours": 8.0,
  "date": "2025-10-02"
}
```

#### 4. Create Bulk Time Entries
```http
POST /api/timeentries/bulk
Authorization: Bearer {token}
Content-Type: application/json

{
  "projectId": "660e8400-e29b-41d4-a716-446655440000",
  "description": "Daily standup meetings",
  "hours": 0.5,
  "dates": ["2025-10-01", "2025-10-02", "2025-10-03", "2025-10-04", "2025-10-05"]
}
```

#### 5. Update Time Entry
```http
PUT /api/timeentries/{timeEntryId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "description": "Updated description",
  "hours": 7.5,
  "category": "Development"
}
```

#### 6. Delete Time Entry
```http
DELETE /api/timeentries/{timeEntryId}
Authorization: Bearer {token}
```

#### 7. Submit Time Entry for Approval
```http
POST /api/timeentries/{timeEntryId}/submit
Authorization: Bearer {token}
```

#### 8. Submit Multiple Time Entries
```http
POST /api/timeentries/submit-multiple
Authorization: Bearer {token}
Content-Type: application/json

{
  "timeEntryIds": [
    "770e8400-e29b-41d4-a716-446655440000",
    "880e8400-e29b-41d4-a716-446655440000"
  ]
}
```

---

### ✅ Approval Endpoints

#### 1. Get Pending Approvals
```http
GET /api/approvals/pending?managerId={managerId}&projectId={projectId}&page=1&pageSize=20
Authorization: Bearer {manager-token}
```

#### 2. Manager Approve Time Entry
```http
POST /api/approvals/manager-approve
Authorization: Bearer {manager-token}
Content-Type: application/json

{
  "timeEntryId": "770e8400-e29b-41d4-a716-446655440000",
  "comments": "Approved - looks good"
}
```

#### 3. Admin Approve Time Entry
```http
POST /api/approvals/admin-approve
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "timeEntryId": "770e8400-e29b-41d4-a716-446655440000",
  "comments": "Final approval granted"
}
```

#### 4. Reject Time Entry
```http
POST /api/approvals/reject
Authorization: Bearer {manager-token}
Content-Type: application/json

{
  "timeEntryId": "770e8400-e29b-41d4-a716-446655440000",
  "comments": "Please provide more details"
}
```

#### 5. Bulk Approve Time Entries
```http
POST /api/approvals/bulk-approve
Authorization: Bearer {manager-token}
Content-Type: application/json

{
  "timeEntryIds": [
    "770e8400-e29b-41d4-a716-446655440000",
    "880e8400-e29b-41d4-a716-446655440000"
  ],
  "comments": "Batch approved"
}
```

#### 6. Get Approval History
```http
GET /api/approvals/history/{timeEntryId}
Authorization: Bearer {token}
```

---

### 📊 Dashboard Endpoints

#### 1. Get User Dashboard Stats
```http
GET /api/dashboard/user-stats
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "totalHoursToday": 8.0,
    "totalHoursWeek": 40.0,
    "totalHoursMonth": 160.0,
    "activeProjects": 3,
    "pendingApprovals": 5
  }
}
```

#### 2. Get Manager Dashboard Stats
```http
GET /api/dashboard/manager-stats
Authorization: Bearer {manager-token}
```

#### 3. Get Admin Dashboard Stats
```http
GET /api/dashboard/admin-stats
Authorization: Bearer {admin-token}
```

#### 4. Get User Project Stats
```http
GET /api/dashboard/user-project-stats?startDate=2025-10-01&endDate=2025-10-31
Authorization: Bearer {token}
```

#### 5. Get Manager Project Stats
```http
GET /api/dashboard/manager-project-stats?startDate=2025-10-01&endDate=2025-10-31
Authorization: Bearer {manager-token}
```

#### 6. Get All Project Stats
```http
GET /api/dashboard/all-project-stats?startDate=2025-10-01&endDate=2025-10-31
Authorization: Bearer {admin-token}
```

---

## 🔒 Authorization Roles

| Role | Access Level |
|------|-------------|
| **Admin** | Full access to all endpoints |
| **Manager** | Can manage assigned projects, approve time entries |
| **Developer** | Can manage own time entries, view assigned projects |

## 📝 Common Response Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (missing or invalid token) |
| 403 | Forbidden (insufficient permissions) |
| 404 | Not Found |
| 500 | Internal Server Error |

## 🎯 Error Response Format

All errors follow this format:

```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    "Detailed error message 1",
    "Detailed error message 2"
  ]
}
```

---

## 🧪 Testing with cURL

### Login Example
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'
```

### Get Projects Example
```bash
curl -X GET http://localhost:5000/api/projects \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Create Time Entry Example
```bash
curl -X POST http://localhost:5000/api/timeentries \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "projectId":"660e8400-e29b-41d4-a716-446655440000",
    "description":"Development work",
    "hours":8.0,
    "date":"2025-10-02"
  }'
```

---

## 📚 Additional Resources

- **Swagger UI**: Access at `http://localhost:5000` when running locally
- **Postman Collection**: Import the OpenAPI spec from Swagger
- **TypeScript SDK**: Generate from OpenAPI spec using openapi-generator

---

*Last Updated: October 2, 2025*
