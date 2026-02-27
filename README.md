# UniThesis - Thesis Management System

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.7-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![MongoDB](https://img.shields.io/badge/MongoDB-6.0+-47A248?logo=mongodb)](https://www.mongodb.com/)
[![Firebase](https://img.shields.io/badge/Firebase-Auth-FFCA28?logo=firebase)](https://firebase.google.com/)
[![SignalR](https://img.shields.io/badge/SignalR-Real--time-512BD4)](https://learn.microsoft.com/aspnet/signalr)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind-3.4-06B6D4?logo=tailwindcss)](https://tailwindcss.com/)

A comprehensive **Thesis Management System** built for universities, enabling end-to-end management of the thesis lifecycle — from topic proposal and group formation to evaluation, defense scheduling, and reporting. Built with **Clean Architecture**, **Domain-Driven Design**, and **CQRS** patterns.

---

## Features

### Admin
- Dashboard with system overview and statistics
- User management (Students, Mentors, Evaluators)
- Department & Major configuration
- Semester & Phase management (create, activate, close)
- System reports generation (PDF / Excel)
- Settings & system configuration
- Support ticket management

### Mentor
- Propose topics to the Topic Pool
- View and manage assigned student groups
- Schedule meetings with students
- Provide feedback on submissions

### Student
- Create groups (up to 5 members, elect Leader)
- Browse and register topics from the Topic Pool
- Submit projects for evaluation
- View meeting & defense schedules

### Evaluator
- Review assigned project submissions
- Approve / Request Modification / Reject projects
- Check similarity between submissions
- View evaluation history and schedule

### Cross-Cutting
- Real-time chat messaging (SignalR)
- Real-time notifications
- PDF report generation (QuestPDF)
- Excel export (ClosedXML)
- Email notifications (MailKit)
- File upload/download (Azure Blob Storage)

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | React 19, TypeScript 5.7, Vite 6, Tailwind CSS 3, Framer Motion, React Router 7 |
| **Backend** | .NET 8, ASP.NET Core, Minimal API |
| **CQRS / Mediator** | MediatR 12.4, FluentValidation 11.11 |
| **Primary Database** | SQL Server + Entity Framework Core 8 |
| **Document Database** | MongoDB (Chat, Notifications, Audit Logs) |
| **Authentication** | Firebase Admin SDK + JWT Bearer Tokens |
| **Real-time** | ASP.NET Core SignalR (NotificationHub, ChatHub) |
| **Background Jobs** | Hangfire (7 scheduled jobs) |
| **Caching** | In-Memory + Redis |
| **Email** | MailKit / MimeKit (SMTP with SSL) |
| **File Storage** | Azure Blob Storage, Google Cloud Storage |
| **Reporting** | QuestPDF (PDF), ClosedXML (Excel) |
| **Logging** | Serilog + Azure Application Insights |
| **Health Checks** | SQL Server, MongoDB, Redis |

---

## Project Structure

```
UniThesis/
├── UniThesis.sln
│
├── UniThesis.Domain/                 # Core Domain Layer
│   ├── Aggregates/                   #   9 Aggregates (User, Project, Group, TopicPool,
│   │   ├── UserAggregate/            #     Evaluation, Defense, Meeting, Semester, Support)
│   │   ├── ProjectAggregate/         #   Each with: Root Entity, Child Entities,
│   │   ├── GroupAggregate/           #     Value Objects, Domain Events, Business Rules
│   │   ├── TopicPoolAggregate/
│   │   ├── EvaluationAggregate/
│   │   ├── DefenseAggregate/
│   │   ├── MeetingAggregate/
│   │   ├── SemesterAggregate/
│   │   └── SupportAggregate/
│   ├── Enums/                        #   35 domain enums
│   ├── Specifications/               #   Query specifications
│   ├── Services/                     #   Domain services
│   └── Common/                       #   Base classes, interfaces, primitives
│
├── UniThesis.Application/            # Application Layer (CQRS)
│   ├── Features/                     #   Feature slices (Commands/, Queries/, DTOs/)
│   │   └── StudentGroups/
│   ├── Common/
│   │   ├── Abstractions/             #   ICommand, IQuery interfaces
│   │   ├── Behaviors/                #   LoggingBehavior, ValidationBehavior
│   │   └── Interfaces/              #   Service contracts
│   └── DependencyInjection.cs
│
├── UniThesis.Persistence/            # Data Access Layer
│   ├── SqlServer/
│   │   ├── AppDbContext.cs           #   EF Core DbContext
│   │   ├── Configurations/           #   Entity type configurations
│   │   ├── Repositories/             #   Repository implementations
│   │   ├── Interceptors/             #   Auditable, SoftDelete, DomainEvent
│   │   ├── QueryServices/            #   Read-optimized query services
│   │   └── ValueConverters/          #   Custom value converters
│   ├── MongoDB/
│   │   ├── Documents/                #   7 document types
│   │   ├── Repositories/             #   MongoDB repository implementations
│   │   ├── Indexes/                  #   Index configurations
│   │   └── Serializers/              #   Custom BSON serializers
│   ├── Migrations/                   #   EF Core migrations
│   ├── Seeds/                        #   Development data seeder
│   └── DependencyInjection.cs
│
├── UniThesis.Infrastructure/         # Infrastructure Layer
│   ├── Authentication/               #   Firebase Auth integration
│   ├── Authorization/                #   Policy-based & resource-based auth
│   │   ├── Policies/                 #   Custom authorization policies
│   │   └── Requirements/            #   5 custom requirements
│   ├── BackgroundJobs/               #   Hangfire configuration
│   │   ├── Jobs/                     #   7 scheduled jobs
│   │   └── Scheduling/              #   Job scheduler & recurring config
│   ├── Caching/                      #   In-Memory + Redis caching
│   ├── EventHandlers/                #   Domain event handlers
│   ├── HealthChecks/                 #   SQL, MongoDB, Redis health checks
│   ├── Logging/                      #   Serilog configuration
│   ├── Middleware/                   #   4 custom middleware
│   ├── RealTime/
│   │   ├── Hubs/                     #   ChatHub, NotificationHub
│   │   ├── Models/                   #   Hub data models
│   │   └── Services/                #   Real-time services
│   ├── Services/
│   │   ├── Email/                    #   MailKit email + templates
│   │   ├── FileStorage/              #   Azure Blob + Google Cloud
│   │   ├── Notification/             #   Notification service
│   │   └── Reporting/               #   QuestPDF + ClosedXML
│   └── DependencyInjection.cs
│
├── UniThesis.API/                    # Presentation Layer
│   ├── Endpoints/                    #   13 Minimal API endpoint groups
│   ├── Extensions/                   #   Endpoint auto-registration
│   ├── Middlewares/                  #   Middleware pipeline
│   ├── Configurations/               #   Swagger / OpenAPI config
│   ├── Program.cs                    #   Application entry point
│   ├── appsettings.json
│   └── Properties/
│       └── launchSettings.json
│
└── unithesis.client/                 # Frontend
    ├── src/
    │   ├── pages/                    #   20 pages across 4 roles
    │   │   ├── admin/                #     7 admin pages
    │   │   ├── mentor/               #     6 mentor pages
    │   │   ├── student/              #     4 student pages
    │   │   ├── evaluator/            #     6 evaluator pages
    │   │   └── auth/                 #     Login page
    │   ├── components/
    │   │   ├── layout/               #     4 role-based layouts + sidebars
    │   │   ├── admin/                #     Admin-specific components
    │   │   └── mentor/               #     Mentor-specific components
    │   ├── contexts/                 #   React context providers
    │   ├── App.tsx                   #   Root component + routing
    │   └── main.tsx                  #   Entry point
    ├── package.json
    ├── tailwind.config.js
    ├── vite.config.ts
    └── tsconfig.json
```

---

## Prerequisites

| Software | Version | Required |
|----------|---------|----------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0+ | Yes |
| [Node.js](https://nodejs.org/) | 18+ | Yes |
| [SQL Server](https://www.microsoft.com/sql-server) | 2019+ | Yes |
| [MongoDB](https://www.mongodb.com/try/download) | 6.0+ | Yes |
| [Redis](https://redis.io/download) | 7.0+ | Optional (caching) |

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/DinhQuangKhanh/UniThesis.git
cd UniThesis
```

### 2. Configure Backend

Copy and update the configuration file:

```bash
cp UniThesis.API/appsettings.json UniThesis.API/appsettings.Development.json
```

Update `appsettings.Development.json` with your settings:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UniThesis;Trusted_Connection=True;TrustServerCertificate=True;",
    "HangfireConnection": "Server=localhost;Database=UniThesisHangfire;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "UniThesisLogs"
  },
  "JwtSettings": {
    "Secret": "<your-jwt-secret>",
    "Issuer": "UniThesis.API",
    "Audience": "UniThesis.Client",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  }
}
```

### 3. Run Database Migrations

```bash
dotnet ef database update --project UniThesis.Persistence --startup-project UniThesis.API
```

### 4. Start the Backend

```bash
dotnet run --project UniThesis.API
```

The API will be available at:
- HTTP: `http://localhost:5141`
- HTTPS: `https://localhost:7176`
- Swagger UI: `https://localhost:7176/swagger`
- Health Check: `https://localhost:7176/health`

### 5. Start the Frontend

```bash
cd unithesis.client
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`.

---

## API Endpoints

The API is organized into 13 endpoint groups using the Minimal API pattern:

| Group | Description |
|-------|-------------|
| **Authentications** | Login, token refresh, Firebase auth |
| **Users** | User CRUD, role assignment, profile management |
| **Departments** | Department & Major management |
| **Semesters** | Semester lifecycle, phase management |
| **TopicPools** | Topic proposal, registration, approval |
| **StudentGroups** | Group creation, member management |
| **Projects** | Project CRUD, submission, document upload |
| **Evaluations** | Evaluator assignment, review, results |
| **Meetings** | Meeting scheduling, approval, completion |
| **Chats** | Real-time messaging, conversations |
| **Notifications** | Real-time notification management |
| **Reports** | PDF/Excel report generation |
| **Supports** | Support ticket management |

Full API documentation available via **Swagger UI** at `/swagger`.

---

## Architecture

This project follows **Clean Architecture** with **Domain-Driven Design** principles:

```
                    ┌─────────────────────┐
                    │    UniThesis.API     │  Presentation
                    │  (Minimal API, MW)   │
                    └────────┬────────────┘
                             │
                    ┌────────▼────────────┐
                    │  UniThesis.App      │  Application
                    │  (CQRS, MediatR)    │
                    └────────┬────────────┘
                             │
                    ┌────────▼────────────┐
                    │  UniThesis.Domain   │  Domain (Core)
                    │  (Aggregates, DDD)  │
                    └──┬─────────────┬────┘
                       │             │
          ┌────────────▼──┐   ┌──────▼───────────┐
          │  Persistence  │   │  Infrastructure  │
          │  (EF + Mongo) │   │  (Auth, SignalR)  │
          └───────────────┘   └──────────────────┘
```

> For detailed architecture diagrams (CQRS flow, domain model, database design, auth flow, SignalR, Hangfire jobs, middleware pipeline, frontend architecture, and deployment), see **[ARCHITECTURE.md](ARCHITECTURE.md)**.

---

## Branching Strategy

| Branch | Purpose |
|--------|---------|
| `master` | Production-ready code |
| `dev` | Development integration |
| `feature/*` | Feature branches |
| `<developer-name>` | Developer working branches |

**Commit Convention:**

```
[UniThesis][Action][Layer]: Description

Actions: Init, Refactor, Perf, Fix, Feature
Layers:  Domain, Application, Persistence, Infrastructure, API, Client, Foundation
```

Example: `[UniThesis][Refactor][Client]: Update FrontEnd`

---

## License

This project is developed as part of a university capstone project.
