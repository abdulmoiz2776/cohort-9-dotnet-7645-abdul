# cohort-9-dotnet-7645-abdul
Cohort 9 — .NET Fullstack (.NET+ReactJS) assignment for Abdul Moiz

Sure — for GitHub submission, you don't need an unnecessarily huge README. This is a **short but proper** version that covers setup, architecture, features, secrets, database, testing, and running.

# Task Management System

A full-stack Task Management System built with **ASP.NET Core Web API** and **React.js**, following **Clean Architecture** principles.

## Features

### Authentication & Authorization

* User Registration
* Email Verification
* Login / Logout
* JWT Authentication
* Refresh Tokens & Session Management
* Forgot/Reset Password
* Change Password
* Profile Management
* Role-Based Authorization
* Admin & Regular User Roles

### Task Management

* Create, Read, Update & Delete Tasks
* Assign Tasks to Users
* Task Status & Priority
* Task Categories
* Due Dates
* Search & Filtering
* User-specific and Admin task management
* Dashboard task statistics

### Other Features

* Global Exception Handling
* Serilog Logging
* Entity Framework Core
* SQL Server
* Swagger / OpenAPI
* Health Checks
* xUnit Unit Testing
* SonarQube Code Quality Analysis

---

## Technology Stack

**Backend:** ASP.NET Core, C#, .NET 8, Entity Framework Core, SQL Server, Identity, JWT, MediatR, FluentValidation, Serilog

**Frontend:** React.js, Vite, Axios, React Router, Tailwind CSS

**Testing:** xUnit, Moq

**Tools:** Git, GitHub, SonarQube

---

## Architecture

The backend follows Clean Architecture:

```text
TaskManagement
├── TaskManagement.API
├── TaskManagement.Application
├── TaskManagement.Domain
├── TaskManagement.Infrastructure
└── TaskManagement.Tests
```

---

## Prerequisites

Install:

* .NET 8 SDK
* Node.js & npm
* SQL Server / SQL Server Express
* Git

Verify:

```bash
dotnet --version
node --version
npm --version
```

---

## Backend Setup

Clone the repository:

```bash
git clone <YOUR-GITHUB-REPOSITORY-URL>
cd <REPOSITORY>/backend/TaskManagement.API
```

Restore and build:

```bash
dotnet restore
dotnet build
```

---

## Database Setup

The application uses **SQL Server** and Entity Framework Core.

Configure your database connection using User Secrets:

```bash
dotnet user-secrets init --project TaskManagement.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING" --project TaskManagement.API
```

Apply migrations:

```bash
dotnet ef database update --project TaskManagement.Infrastructure --startup-project TaskManagement.API
```

---

## User Secrets

Sensitive configuration is stored using **ASP.NET Core User Secrets** and is not included in Git.

Required configuration keys:

```text
Jwt:Secret
Jwt:Issuer
Jwt:Audience
Email:Password
ConnectionStrings:DefaultConnection
AdminUser:Email
AdminUser:Password
```

Each developer/evaluator must configure these values on their own machine.

Check configured secrets:

```bash
dotnet user-secrets list --project TaskManagement.API
```

**Never commit real passwords, JWT secrets, connection strings, or API credentials to GitHub.**

---

## Run Backend

```bash
dotnet run --project TaskManagement.API
```

Swagger:

```text
https://localhost:<API_PORT>/swagger
```

---

## Frontend Setup

Navigate to the React project:

```bash
cd frontend
npm install
npm run dev
```

Configure the backend API URL in the frontend environment file if required:

```env
VITE_API_BASE_URL=https://localhost:<API_PORT>/api
```

---

## Testing

Run unit tests:

```bash
dotnet test
```

Generate code coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Logging & Exception Handling

* **Serilog** is used for application and error logging.
* **Global Exception Handling** provides consistent API error responses.
* Sensitive information such as passwords and tokens should not be logged.

---

## SonarQube

SonarQube is used for:

* Code quality analysis
* Bugs
* Vulnerabilities
* Code smells
* Code duplication
* Test coverage

A SonarQube server must be configured separately for analysis.

---

## Running the Complete Application

### Backend

```bash
cd backend/TaskManagement.API
dotnet run --project TaskManagement.API
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Then open the frontend URL shown by Vite.

---

## Author

**Abdul Moiz**

BS Computer Science

