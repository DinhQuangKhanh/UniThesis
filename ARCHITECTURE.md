# UniThesis - Architecture Documentation

Detailed architecture diagrams for the UniThesis Thesis Management System.

---

## Table of Contents

1. [Clean Architecture Layers](#1-clean-architecture-layers)
2. [High-Level System Architecture](#2-high-level-system-architecture)
3. [CQRS + MediatR Pipeline](#3-cqrs--mediatr-pipeline)
4. [Domain Model (Aggregates)](#4-domain-model-aggregates)
5. [Database Architecture (Polyglot Persistence)](#5-database-architecture-polyglot-persistence)
6. [Authentication & Authorization](#6-authentication--authorization)
7. [Real-time Communication (SignalR)](#7-real-time-communication-signalr)
8. [Background Jobs (Hangfire)](#8-background-jobs-hangfire)
9. [Request Pipeline (Middleware)](#9-request-pipeline-middleware)
10. [Frontend Architecture](#10-frontend-architecture)
11. [Deployment Architecture](#11-deployment-architecture)

---

## 1. Clean Architecture Layers

The project follows Clean Architecture with strict dependency inversion. Outer layers depend on inner layers, never the reverse.

```
┌──────────────────────────────────────────────────────────────────────┐
│                                                                      │
│   ┌──────────────────────────────────────────────────────────────┐   │
│   │                                                              │   │
│   │   ┌──────────────────────────────────────────────────────┐   │   │
│   │   │                                                      │   │   │
│   │   │   ┌──────────────────────────────────────────────┐   │   │   │
│   │   │   │                                              │   │   │   │
│   │   │   │             UniThesis.Domain                  │   │   │   │
│   │   │   │                                              │   │   │   │
│   │   │   │   - Aggregates (9)     - Domain Events       │   │   │   │
│   │   │   │   - Entities           - Business Rules      │   │   │   │
│   │   │   │   - Value Objects      - Specifications      │   │   │   │
│   │   │   │   - Enums (35)         - Domain Services     │   │   │   │
│   │   │   │   - Repository Interfaces (Contracts)        │   │   │   │
│   │   │   │                                              │   │   │   │
│   │   │   └──────────────────────────────────────────────┘   │   │   │
│   │   │                                                      │   │   │
│   │   │                UniThesis.Application                  │   │   │
│   │   │                                                      │   │   │
│   │   │   - Commands & Queries (CQRS)   - DTOs               │   │   │
│   │   │   - Pipeline Behaviors          - Validators         │   │   │
│   │   │   - Service Interfaces          - Event Handlers     │   │   │
│   │   │                                                      │   │   │
│   │   └──────────────────────────────────────────────────────┘   │   │
│   │                                                              │   │
│   │   UniThesis.Persistence          UniThesis.Infrastructure    │   │
│   │                                                              │   │
│   │   - AppDbContext (EF Core)       - Firebase Authentication   │   │
│   │   - MongoDbContext               - Authorization Handlers    │   │
│   │   - Repository Implementations   - SignalR Hubs              │   │
│   │   - Migrations & Seeds           - Hangfire Background Jobs  │   │
│   │   - Interceptors                 - Email / File Storage      │   │
│   │   - Query Services               - Caching (Redis)           │   │
│   │                                  - Health Checks             │   │
│   └──────────────────────────────────────────────────────────────┘   │
│                                                                      │
│                        UniThesis.API                                  │
│                                                                      │
│   - Minimal API Endpoints (13 groups)   - Middleware Pipeline        │
│   - Swagger / OpenAPI Configuration     - CORS Configuration        │
│   - Program.cs (Composition Root)       - Error Handling             │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘

           Dependency Direction:  API ──► Application ──► Domain
                                  Persistence ──────────► Domain
                                  Infrastructure ───────► Domain
```

**Key Principle:** The Domain layer has ZERO external dependencies. It defines interfaces (contracts) that outer layers implement.

---

## 2. High-Level System Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              CLIENT (Browser)                               │
│                                                                             │
│         React 19  +  TypeScript 5.7  +  Vite 6  +  Tailwind CSS 3         │
│         React Router 7  +  Framer Motion  +  React PDF                     │
│                                                                             │
└──────────────┬────────────────────────────────────┬─────────────────────────┘
               │                                    │
               │ HTTP/REST (JSON)                   │ WebSocket
               │ Port: 5141 / 7176                  │ (SignalR)
               │                                    │
┌──────────────▼────────────────────────────────────▼─────────────────────────┐
│                           UniThesis.API (.NET 8)                            │
│                                                                             │
│  ┌─────────────┐  ┌──────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │  Endpoints   │  │ Swagger  │  │ Middleware   │  │ SignalR Hubs        │  │
│  │  (13 groups) │  │ /swagger │  │  Pipeline    │  │ /hubs/chat          │  │
│  │              │  │          │  │              │  │ /hubs/notifications  │  │
│  └──────┬───────┘  └──────────┘  └──────────────┘  └─────────────────────┘  │
│         │                                                                   │
├─────────▼───────────────────────────────────────────────────────────────────┤
│                       UniThesis.Application                                 │
│                                                                             │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────────┐  │
│  │    MediatR        │  │  FluentValidation │  │  Pipeline Behaviors     │  │
│  │  Commands/Queries │  │  Request Validators│  │  Logging + Validation  │  │
│  └────────┬─────────┘  └──────────────────┘  └──────────────────────────┘  │
│           │                                                                 │
├───────────▼─────────────────────────────────────────────────────────────────┤
│                         UniThesis.Domain                                    │
│                                                                             │
│      9 Aggregates  │  50+ Domain Events  │  25+ Business Rules             │
│      35 Enums      │  Specifications     │  Domain Services                │
│                                                                             │
├─────────────────────────────────┬───────────────────────────────────────────┤
│      UniThesis.Persistence      │         UniThesis.Infrastructure          │
└───────┬──────────┬──────────────┘──────┬────────┬────────┬────────┬────────┘
        │          │                     │        │        │        │
        ▼          ▼                     ▼        ▼        ▼        ▼
┌──────────┐ ┌──────────┐      ┌──────────┐ ┌────────┐ ┌──────┐ ┌────────┐
│SQL Server│ │ MongoDB  │      │ Firebase │ │Hangfire│ │Redis │ │  SMTP  │
│ (EF Core │ │          │      │   Auth   │ │  Jobs  │ │Cache │ │ Email  │
│  8.0.23) │ │ Driver   │      │  Admin   │ │ (SQL)  │ │      │ │MailKit │
│          │ │  3.6.0   │      │  SDK     │ │ 1.8.22 │ │      │ │        │
└──────────┘ └──────────┘      └──────────┘ └────────┘ └──────┘ └────────┘
                                     │
                                     ▼
                              ┌────────────┐
                              │Azure Blob  │
                              │  Storage   │
                              │+ GCS       │
                              └────────────┘
```

---

## 3. CQRS + MediatR Pipeline

Commands (write operations) and Queries (read operations) follow separate paths through the MediatR pipeline.

```
                         HTTP Request (POST/PUT/DELETE)
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │   Minimal API        │
                         │   Endpoint           │
                         │                      │
                         │   var result = await  │
                         │   mediator.Send(cmd); │
                         └──────────┬────────────┘
                                    │
                      ┌─────────────▼─────────────┐
                      │     MediatR Pipeline       │
                      │                            │
                      │  ┌──────────────────────┐  │
                      │  │ ValidationBehavior   │  │    Runs FluentValidation
                      │  │ (IPipelineBehavior)  │  │    validators. Throws
                      │  │                      │  │    ValidationException
                      │  └──────────┬───────────┘  │    if invalid.
                      │             │              │
                      │  ┌──────────▼───────────┐  │
                      │  │  LoggingBehavior     │  │    Logs request name,
                      │  │  (IPipelineBehavior) │  │    user info, and
                      │  │                      │  │    execution time.
                      │  └──────────┬───────────┘  │
                      │             │              │
                      └─────────────┼──────────────┘
                                    │
                     ┌──────────────┴──────────────┐
                     │                             │
            ┌────────▼────────┐          ┌─────────▼────────┐
            │  Command Path   │          │   Query Path     │
            │                 │          │                  │
            │ ICommandHandler │          │  IQueryHandler   │
            │                 │          │                  │
            │ - Validate rules│          │ - Build spec     │
            │ - Modify domain │          │ - Execute query  │
            │ - Raise events  │          │ - Map to DTO     │
            │ - Save via UoW  │          │ - Return result  │
            └────────┬────────┘          └─────────┬────────┘
                     │                             │
            ┌────────▼────────┐          ┌─────────▼────────┐
            │   Repository    │          │  Query Service   │
            │  + Unit of Work │          │  (Read-optimized)│
            └────────┬────────┘          └─────────┬────────┘
                     │                             │
                     └──────────────┬──────────────┘
                                    │
                           ┌────────▼────────┐
                           │    Database     │
                           │  (SQL Server)   │
                           └─────────────────┘

    Domain Events Flow:
    ┌───────────┐    DomainEventInterceptor     ┌─────────────────┐
    │ Aggregate  │──── (EF Core SaveChanges) ───►│  MediatR Publish │
    │ .Raise()   │                               │  (INotification) │
    └───────────┘                               └────────┬────────┘
                                                         │
                                              ┌──────────▼──────────┐
                                              │  Event Handlers     │
                                              │  - Send Email       │
                                              │  - Push Notification│
                                              │  - Update MongoDB   │
                                              │  - Log Activity     │
                                              └─────────────────────┘
```

---

## 4. Domain Model (Aggregates)

9 Aggregate Roots with their entities, value objects, and relationships.

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              DOMAIN MODEL                                       │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌─────────────────┐         ┌──────────────────────┐                          │
│  │  UserAggregate   │         │  SemesterAggregate    │                          │
│  │─────────────────│         │──────────────────────│                          │
│  │ User (Root)      │         │ Semester (Root)       │                          │
│  │ ├── UserRole     │         │ ├── SemesterPhase     │                          │
│  │ └── Email (VO)   │         │ ├── AcademicYear (VO) │                          │
│  │                  │         │ ├── DateRange (VO)    │                          │
│  │ Rules:           │         │ └── SemesterCode (VO) │                          │
│  │  EmailMustBeFpt  │         │                      │                          │
│  └────────┬─────────┘         │ Rules:               │                          │
│           │                   │  PhasesMustNotOverlap │                          │
│           │ creates           │  DatesMustBeValid     │                          │
│           │                   └──────────────────────┘                          │
│  ┌────────▼─────────┐                                                           │
│  │  GroupAggregate   │◄─────────────── registers ──────────────────┐            │
│  │─────────────────│                                              │            │
│  │ Group (Root)      │         ┌──────────────────────┐            │            │
│  │ ├── GroupMember   │         │ TopicPoolAggregate    │            │            │
│  │ └── GroupCode(VO) │         │──────────────────────│            │            │
│  │                  │         │ TopicPool (Root)      │            │            │
│  │ Rules:           │         │ ├── TopicRegistration ├────────────┘            │
│  │  MaxMembers      │         │ ├── ExpirationInfo(VO)│                          │
│  │  MustHaveLeader  │         │ └── TopicCode (VO)   │                          │
│  │  NoMultipleGroups│         │                      │                          │
│  └────────┬─────────┘         │ Rules:               │                          │
│           │                   │  MustBeAvailable      │                          │
│           │ owns              │  MaxActiveTopics      │                          │
│           │                   └──────────────────────┘                          │
│  ┌────────▼─────────────────────────────────────────────────────────┐           │
│  │  ProjectAggregate                                                │           │
│  │─────────────────────────────────────────────────────────────────│           │
│  │ Project (Root)                                                   │           │
│  │ ├── Document              ├── ProjectMentor                      │           │
│  │ ├── ProjectCode (VO)      ├── ProjectName (VO)                   │           │
│  │ └── TechnologyStack (VO)                                        │           │
│  │                                                                  │           │
│  │ Events: Created, Submitted, Approved, Completed, DocumentUploaded│           │
│  │ Rules:  OnlySubmitWhenDraft, MaxMentors                          │           │
│  └──────┬──────────────────────┬─────────────────────┬──────────────┘           │
│         │                      │                     │                          │
│         │ evaluated by         │ defended at          │ has meetings            │
│         │                      │                     │                          │
│  ┌──────▼───────────┐  ┌──────▼───────────┐  ┌──────▼───────────┐             │
│  │ EvaluationAgg.   │  │  DefenseAgg.     │  │  MeetingAgg.     │             │
│  │─────────────────│  │─────────────────│  │─────────────────│             │
│  │ EvaluationSub-   │  │ DefenseSchedule  │  │ MeetingSchedule  │             │
│  │   mission (Root) │  │   (Root)         │  │   (Root)         │             │
│  │ ├── ProjectEval- │  │ ├── DefenseCncl  │  │ └── MeetingLoc-  │             │
│  │ │   uatorAssign  │  │ ├── CouncilMbr   │  │     ation (VO)   │             │
│  │ ├── ProjectSnap- │  │ └── DefenseLoc-  │  │                  │             │
│  │ │   shot (VO)    │  │     ation (VO)   │  │ Events:          │             │
│  │ └── Submission-  │  │                  │  │  Requested       │             │
│  │     Number (VO)  │  │ Rules:           │  │  Approved        │             │
│  │                  │  │  MustHaveChairman│  │  Completed       │             │
│  │ Rules:           │  │  MustHaveCouncil │  │  Cancelled       │             │
│  │  Exactly3Evaluat │  │                  │  │  Rejected        │             │
│  │  CannotEvalOwn   │  └──────────────────┘  └──────────────────┘             │
│  │  MaxResubmission │                                                          │
│  └──────────────────┘                                                          │
│                                                                                 │
│  ┌──────────────────┐                                                          │
│  │  SupportAgg.     │     (Standalone: Department, Major, SystemConfiguration, │
│  │─────────────────│                  Report, ProjectArchive)                  │
│  │ SupportTicket    │                                                          │
│  │   (Root)         │                                                          │
│  │ └── TicketCode   │                                                          │
│  │     (VO)         │                                                          │
│  └──────────────────┘                                                          │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 5. Database Architecture (Polyglot Persistence)

SQL Server handles transactional data with ACID guarantees. MongoDB handles high-throughput logs and real-time messaging data.

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        SQL SERVER (Entity Framework Core)                    │
│                     Transactional Data with ACID Guarantees                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────┐ ┌──────────────┐ ┌──────────────┐ ┌────────────────────┐  │
│  │    Users     │ │   Groups     │ │  Projects    │ │    TopicPools      │  │
│  │─────────────│ │──────────────│ │──────────────│ │────────────────────│  │
│  │ Id          │ │ Id           │ │ Id           │ │ Id                 │  │
│  │ Email       │ │ GroupCode    │ │ ProjectCode  │ │ TopicCode          │  │
│  │ FullName    │ │ Status       │ │ ProjectName  │ │ Title              │  │
│  │ Status      │ │ CreatedAt    │ │ Status       │ │ Status             │  │
│  │             │ │              │ │ TechStack    │ │ ExpirationDate     │  │
│  │ ┌─────────┐│ │ ┌──────────┐ │ │ ┌──────────┐ │ │ ┌────────────────┐ │  │
│  │ │UserRoles││ │ │GroupMmbrs│ │ │ │Documents │ │ │ │TopicRegistrtns│ │  │
│  │ └─────────┘│ │ └──────────┘ │ │ │ProjectMtr│ │ │ └────────────────┘ │  │
│  └─────────────┘ └──────────────┘ │ └──────────┘ │ └────────────────────┘  │
│                                   └──────────────┘                          │
│  ┌───────────────┐ ┌───────────────┐ ┌──────────────┐ ┌──────────────────┐ │
│  │  Semesters     │ │  Evaluations  │ │  Defenses    │ │   Meetings       │ │
│  │───────────────│ │───────────────│ │──────────────│ │──────────────────│ │
│  │ Id            │ │ Id            │ │ Id           │ │ Id               │ │
│  │ SemesterCode  │ │ Status        │ │ ScheduleDate │ │ ScheduleDate     │ │
│  │ Status        │ │ SubmissionNum │ │ Status       │ │ Status           │ │
│  │               │ │               │ │              │ │ Location         │ │
│  │ ┌───────────┐ │ │ ┌───────────┐ │ │ ┌──────────┐│ │                  │ │
│  │ │SemPhases  │ │ │ │ProjEvalAs│ │ │ │DefCncl   ││ └──────────────────┘ │
│  │ └───────────┘ │ │ └───────────┘ │ │ │CnclMmbrs ││                     │
│  └───────────────┘ └───────────────┘ │ └──────────┘│ ┌──────────────────┐ │
│                                      └──────────────┘ │ SupportTickets   │ │
│  Standalone: Department, Major, SystemConfiguration,   │──────────────────│ │
│              Report, ProjectArchive                    │ Id, Code, Status │ │
│                                                        └──────────────────┘ │
│  Interceptors:                                                              │
│    - AuditableEntityInterceptor (CreatedAt, UpdatedAt, CreatedBy)           │
│    - SoftDeleteInterceptor (IsDeleted flag, no hard deletes)                │
│    - DomainEventInterceptor (publishes events after SaveChanges)            │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                       MONGODB (Document Store)                              │
│              High-Throughput Logs, Chat & Real-time Data                    │
│              Database: "UniThesisLogs"                                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────────┐  ┌──────────────────────┐                        │
│  │  Conversations       │  │  Messages            │                        │
│  │──────────────────────│  │──────────────────────│                        │
│  │ _id, Participants,   │  │ _id, ConversationId, │   Real-time Chat       │
│  │ Type, CreatedAt      │  │ SenderId, Content,   │   Messaging Data       │
│  │                      │  │ Type, SentAt         │                        │
│  └──────────────────────┘  └──────────────────────┘                        │
│                                                                             │
│  ┌──────────────────────┐  ┌──────────────────────┐                        │
│  │  Notifications       │  │  EvaluationLogs      │                        │
│  │──────────────────────│  │──────────────────────│                        │
│  │ _id, UserId, Title,  │  │ _id, EvaluationId,   │   Event Tracking       │
│  │ Message, Category,   │  │ Action, Timestamp,   │   & Audit Trail        │
│  │ IsRead, CreatedAt    │  │ PerformedBy, Details │                        │
│  └──────────────────────┘  └──────────────────────┘                        │
│                                                                             │
│  ┌──────────────────────┐  ┌──────────────────────┐  ┌──────────────────┐  │
│  │  ProjectModHistory   │  │  UserActivityLogs    │  │ SystemAuditLogs  │  │
│  │──────────────────────│  │──────────────────────│  │──────────────────│  │
│  │ _id, ProjectId,      │  │ _id, UserId, Action, │  │ _id, Action,     │  │
│  │ FieldChanged,        │  │ Timestamp, IpAddress,│  │ EntityType,      │  │
│  │ OldValue, NewValue,  │  │ UserAgent, Details   │  │ Timestamp,       │  │
│  │ ModifiedBy, At       │  │                      │  │ Details          │  │
│  └──────────────────────┘  └──────────────────────┘  └──────────────────┘  │
│                                                                             │
│  Why MongoDB?                                                               │
│    - High write throughput for logs (no joins needed)                       │
│    - Flexible schema for varying log structures                            │
│    - TTL indexes for automatic log cleanup                                 │
│    - Optimized for append-heavy, read-light workloads                      │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 6. Authentication & Authorization

```
                            ┌──────────────────┐
                            │   Client App     │
                            │   (React)        │
                            └────────┬─────────┘
                                     │
                          1. Login with credentials
                                     │
                            ┌────────▼─────────┐
                            │  Firebase Auth   │
                            │  (Google Cloud)  │
                            └────────┬─────────┘
                                     │
                          2. Firebase ID Token
                                     │
                            ┌────────▼─────────┐
                            │  UniThesis API   │
                            │  /auth/login     │
                            └────────┬─────────┘
                                     │
                   3. Validate Firebase token via Admin SDK
                      Generate JWT (60 min) + Refresh Token (7 days)
                                     │
                            ┌────────▼─────────┐
                            │  JWT Bearer      │
                            │  Token Response  │
                            │  { token,        │
                            │    refreshToken } │
                            └────────┬─────────┘
                                     │
                     ┌───────────────┴────────────────┐
                     │    Subsequent API Requests      │
                     │    Authorization: Bearer <jwt>  │
                     └───────────────┬────────────────┘
                                     │
                            ┌────────▼──────────────────────────────────┐
                            │        AUTHORIZATION PIPELINE             │
                            │                                           │
                            │  ┌─────────────────────────────────────┐  │
                            │  │ 1. PermissionAuthorizationHandler   │  │
                            │  │    Check role-based permissions     │  │
                            │  ├─────────────────────────────────────┤  │
                            │  │ 2. ProjectOwnerAuthorizationHandler │  │
                            │  │    Verify user owns the project     │  │
                            │  ├─────────────────────────────────────┤  │
                            │  │ 3. GroupMemberAuthorizationHandler  │  │
                            │  │    Verify user is in the group      │  │
                            │  ├─────────────────────────────────────┤  │
                            │  │ 4. MentorOfProjectAuthHandler      │  │
                            │  │    Verify user mentors the project  │  │
                            │  ├─────────────────────────────────────┤  │
                            │  │ 5. ResourceAuthorizationHandler    │  │
                            │  │    Generic resource-based check     │  │
                            │  └─────────────────────────────────────┘  │
                            │                                           │
                            │  Requirements:                            │
                            │   - PermissionRequirement                 │
                            │   - GroupMemberRequirement                │
                            │   - MentorOfProjectRequirement            │
                            │   - ProjectOwnerRequirement               │
                            │   - SameDepartmentRequirement             │
                            └───────────────────────────────────────────┘

    Roles: Admin  |  Mentor  |  Student  |  Evaluator
```

---

## 7. Real-time Communication (SignalR)

```
┌──────────────────────────────────────────────────────────────────────────┐
│                        SIGNALR ARCHITECTURE                              │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   Client A                         Client B                              │
│   (Student)                        (Mentor)                              │
│   ┌──────────┐                     ┌──────────┐                          │
│   │ React    │                     │ React    │                          │
│   │ SignalR  │                     │ SignalR  │                          │
│   │ Client   │                     │ Client   │                          │
│   └────┬─────┘                     └────┬─────┘                          │
│        │                                │                                │
│        │ ws://host/hubs/chat            │ ws://host/hubs/chat            │
│        │ ?access_token=<jwt>            │ ?access_token=<jwt>            │
│        │                                │                                │
│   ┌────▼────────────────────────────────▼────┐                           │
│   │            SignalR Hub Server             │                           │
│   │                                           │                           │
│   │  ┌─────────────────────────────────────┐  │                           │
│   │  │         ChatHub                     │  │                           │
│   │  │         /hubs/chat                  │  │                           │
│   │  │                                     │  │                           │
│   │  │  - SendMessage(convId, content)     │  │                           │
│   │  │  - JoinConversation(convId)         │  │                           │
│   │  │  - LeaveConversation(convId)        │  │                           │
│   │  │  - OnConnected / OnDisconnected     │  │                           │
│   │  │                                     │  │                           │
│   │  │  Groups:                            │  │                           │
│   │  │    "conversation_{id}" per chat     │  │                           │
│   │  │    "user_{id}" per user             │  │                           │
│   │  └─────────────────────────────────────┘  │                           │
│   │                                           │                           │
│   │  ┌─────────────────────────────────────┐  │                           │
│   │  │      NotificationHub               │  │                           │
│   │  │      /hubs/notifications            │  │                           │
│   │  │                                     │  │                           │
│   │  │  - OnConnected (join user group)    │  │                           │
│   │  │  - ReceiveNotification (client)     │  │                           │
│   │  │                                     │  │                           │
│   │  │  Groups:                            │  │                           │
│   │  │    "user_{id}" per user             │  │                           │
│   │  │    "project_{id}" per project       │  │                           │
│   │  └─────────────────────────────────────┘  │                           │
│   │                                           │                           │
│   └───────────────────────────────────────────┘                           │
│                     │                                                     │
│                     ▼                                                     │
│             ┌───────────────┐                                             │
│             │   MongoDB     │  Messages & Notifications                   │
│             │   Storage     │  persisted for history                      │
│             └───────────────┘                                             │
│                                                                          │
│   Auth: JWT token passed via query string (?access_token=...)            │
│   Transport: WebSocket (primary), Server-Sent Events, Long Polling       │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## 8. Background Jobs (Hangfire)

```
┌──────────────────────────────────────────────────────────────────────────┐
│                     HANGFIRE BACKGROUND JOBS                             │
│                                                                          │
│   Dashboard: /hangfire                                                   │
│   Storage:   SQL Server (HangfireConnection)                            │
│   Service:   HangfireJobService : IBackgroundJobService                 │
│                                                                          │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   ┌────────────────────────────────────────────────────────────────┐     │
│   │  RECURRING JOBS (Scheduled by RecurringJobsConfiguration)     │     │
│   ├────────────────────────────────────────────────────────────────┤     │
│   │                                                                │     │
│   │  1. TopicExpirationJob                                        │     │
│   │     Schedule: Daily                                            │     │
│   │     Action:   Check topic pool expiration dates,               │     │
│   │               auto-close expired topics                        │     │
│   │                                                                │     │
│   │  2. EvaluationReminderJob                                     │     │
│   │     Schedule: Daily                                            │     │
│   │     Action:   Send email reminders to evaluators               │     │
│   │               with pending reviews                             │     │
│   │                                                                │     │
│   │  3. SemesterPhaseTransitionJob                                 │     │
│   │     Schedule: Daily                                            │     │
│   │     Action:   Auto-transition semester phases based            │     │
│   │               on configured date ranges                        │     │
│   │                                                                │     │
│   │  4. DefenseScheduleReminderJob                                │     │
│   │     Schedule: Daily                                            │     │
│   │     Action:   Notify council members and students              │     │
│   │               of upcoming defense sessions                     │     │
│   │                                                                │     │
│   │  5. MeetingReminderJob                                        │     │
│   │     Schedule: Hourly                                           │     │
│   │     Action:   Send reminders for upcoming meetings             │     │
│   │               to mentors and students                          │     │
│   │                                                                │     │
│   │  6. DataCleanupJob                                            │     │
│   │     Schedule: Weekly                                           │     │
│   │     Action:   Clean up expired tokens, old logs,               │     │
│   │               temporary files                                  │     │
│   │                                                                │     │
│   │  7. ReportGenerationJob                                       │     │
│   │     Schedule: On-demand / Weekly                               │     │
│   │     Action:   Generate semester progress reports               │     │
│   │               (PDF via QuestPDF, Excel via ClosedXML)          │     │
│   │                                                                │     │
│   └────────────────────────────────────────────────────────────────┘     │
│                                                                          │
│   Job Flow:                                                              │
│                                                                          │
│   ┌──────────┐    ┌──────────────┐    ┌────────────┐    ┌────────────┐  │
│   │ Scheduler │───►│ Hangfire SQL  │───►│ Job Runner │───►│ Services   │  │
│   │ (Cron)    │    │   Storage    │    │ (Worker)   │    │ (Email,    │  │
│   │           │    │              │    │            │    │  SignalR,  │  │
│   └──────────┘    └──────────────┘    └────────────┘    │  DB)       │  │
│                                                         └────────────┘  │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## 9. Request Pipeline (Middleware)

The order of middleware is critical. Each request flows through this pipeline from top to bottom.

```
                        Incoming HTTP Request
                                │
                                ▼
                ┌───────────────────────────────┐
                │     HTTPS Redirection         │    Redirect HTTP to HTTPS
                └───────────────┬───────────────┘
                                │
                                ▼
                ┌───────────────────────────────┐
                │     CORS Middleware           │    Validate AllowedOrigins
                │     (Cors.AllowedOrigins)     │    Allow credentials
                └───────────────┬───────────────┘
                                │
                                ▼
                ┌───────────────────────────────┐
                │     Authentication            │    Validate JWT Bearer token
                │     (JWT Bearer)              │    Set HttpContext.User
                └───────────────┬───────────────┘
                                │
                                ▼
                ┌───────────────────────────────┐
                │     Authorization             │    Check policies & requirements
                └───────────────┬───────────────┘
                                │
                                ▼
                ┌───────────────────────────────┐
                │     CorrelationIdMiddleware   │    Add X-Correlation-Id header
                │                               │    for request tracing
                └───────────────┬───────────────┘
                                │
                                ▼
                ┌───────────────────────────────┐
                │     RequestLoggingMiddleware  │    Log request details
                │     (Serilog)                 │    with structured logging
                └───────────────┬───────────────┘
                                │
                                ▼
                ┌───────────────────────────────┐
                │   ExceptionHandlingMiddleware │    Catch unhandled exceptions
                │                               │    Return ProblemDetails JSON
                └───────────────┬───────────────┘
                                │
                                ▼
                ┌───────────────────────────────┐
                │  PerformanceMonitoringMW      │    Track response times
                │  (Application Insights)       │    Flag slow requests
                └───────────────┬───────────────┘
                                │
                     ┌──────────┴──────────┐
                     │                     │
                     ▼                     ▼
        ┌────────────────────┐  ┌─────────────────────┐
        │  /health           │  │  /hubs/*             │
        │  Health Checks     │  │  SignalR Hubs        │
        │  (SQL, Mongo,      │  │  - /hubs/chat        │
        │   Redis)           │  │  - /hubs/notif.      │
        └────────────────────┘  └─────────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Minimal API Endpoints         │
        │  (13 groups auto-registered)   │
        │                                │
        │  /api/auth/*                   │
        │  /api/users/*                  │
        │  /api/projects/*               │
        │  /api/groups/*                 │
        │  /api/topics/*                 │
        │  /api/evaluations/*            │
        │  /api/meetings/*               │
        │  /api/semesters/*              │
        │  /api/departments/*            │
        │  /api/notifications/*          │
        │  /api/chats/*                  │
        │  /api/reports/*                │
        │  /api/supports/*               │
        └────────────────────────────────┘
                     │
                     ▼
              HTTP Response
```

---

## 10. Frontend Architecture

```
┌──────────────────────────────────────────────────────────────────────────┐
│                     FRONTEND ARCHITECTURE                                │
│                     React 19 + TypeScript 5.7 + Vite 6                  │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   Entry Point: main.tsx ──► App.tsx (Router)                            │
│                                                                          │
│   ┌──────────────────────────────────────────────────────────────────┐   │
│   │                     ROUTING (React Router 7)                     │   │
│   ├──────────────────────────────────────────────────────────────────┤   │
│   │                                                                  │   │
│   │   /login ──────────────────────────► LoginPage                  │   │
│   │   /maintenance ────────────────────► MaintenancePage            │   │
│   │                                                                  │   │
│   │   /admin/* ──► AdminLayout                                      │   │
│   │   │  /admin             ──► DashboardPage                       │   │
│   │   │  /admin/users       ──► UsersPage                           │   │
│   │   │  /admin/projects    ──► ProjectsPage                        │   │
│   │   │  /admin/semesters   ──► SemestersPage                       │   │
│   │   │  /admin/reports     ──► ReportsPage                         │   │
│   │   │  /admin/settings    ──► SettingsPage                        │   │
│   │   │  /admin/support     ──► SupportPage                         │   │
│   │                                                                  │   │
│   │   /mentor/* ──► MentorLayout                                    │   │
│   │   │  /mentor            ──► MentorDashboardPage                 │   │
│   │   │  /mentor/groups     ──► MentorGroupsPage                   │   │
│   │   │  /mentor/groups/:id ──► MentorTopicDetailPage              │   │
│   │   │  /mentor/topics     ──► MentorTopicsPage                   │   │
│   │   │  /mentor/topics/:id ──► MentorFeedbackPage                 │   │
│   │   │  /mentor/schedule   ──► MentorSchedulePage                 │   │
│   │                                                                  │   │
│   │   /student/* ──► StudentLayout                                  │   │
│   │   │  /student           ──► StudentDashboardPage               │   │
│   │   │  /student/my-topic  ──► StudentMyTopicPage                 │   │
│   │   │  /student/topics    ──► StudentTopicsPage                  │   │
│   │   │  /student/schedule  ──► StudentSchedulePage                │   │
│   │                                                                  │   │
│   │   /evaluator/* ──► EvaluatorLayout                              │   │
│   │   │  /evaluator             ──► EvaluatorDashboardPage         │   │
│   │   │  /evaluator/projects    ──► EvaluatorProjectsPage          │   │
│   │   │  /evaluator/schedule    ──► EvaluatorSchedulePage          │   │
│   │   │  /evaluator/history     ──► EvaluatorHistoryPage           │   │
│   │   │  /evaluator/review/:id  ──► EvaluatorReviewPage            │   │
│   │   │  /evaluator/similarity  ──► EvaluatorSimilarityPage        │   │
│   │                                                                  │   │
│   └──────────────────────────────────────────────────────────────────┘   │
│                                                                          │
│   ┌──────────────────────────────────────────────────────────────────┐   │
│   │                     COMPONENT HIERARCHY                          │   │
│   ├──────────────────────────────────────────────────────────────────┤   │
│   │                                                                  │   │
│   │   App.tsx                                                        │   │
│   │   ├── ProtectedRoute (auth guard)                               │   │
│   │   ├── Layouts (role-specific)                                   │   │
│   │   │   ├── AdminLayout    ──► Header + Sidebar + Content         │   │
│   │   │   ├── MentorLayout   ──► Header + MentorSidebar + Content   │   │
│   │   │   ├── StudentLayout  ──► Header + StudentSidebar + Content  │   │
│   │   │   └── EvaluatorLayout──► Header + EvaluatorSidebar + Content│   │
│   │   ├── Header                                                    │   │
│   │   │   └── NotificationDropdown (SignalR real-time)              │   │
│   │   └── Modals                                                    │   │
│   │       ├── CreateSemesterModal                                   │   │
│   │       └── RegisterTopicModal                                    │   │
│   │                                                                  │   │
│   └──────────────────────────────────────────────────────────────────┘   │
│                                                                          │
│   ┌──────────────────────────────────────────────────────────────────┐   │
│   │                     TECH DETAILS                                 │   │
│   ├──────────────────────────────────────────────────────────────────┤   │
│   │   Build:       Vite 6 (dev server + production bundler)         │   │
│   │   Styling:     Tailwind CSS 3 + PostCSS                         │   │
│   │   Animation:   Framer Motion                                    │   │
│   │   PDF Viewer:  react-pdf + pdfjs-dist                           │   │
│   │   Theming:     CSS custom properties (--color-primary)          │   │
│   │                stored in localStorage                           │   │
│   │   Linting:     ESLint 9 + typescript-eslint                     │   │
│   └──────────────────────────────────────────────────────────────────┘   │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## 11. Deployment Architecture

```
┌──────────────────────────────────────────────────────────────────────────┐
│                  CURRENT: LOCAL DEVELOPMENT SETUP                        │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   Developer Machine                                                      │
│   ┌─────────────────────────────────────────────────────────────────┐   │
│   │                                                                 │   │
│   │  ┌──────────────┐    ┌──────────────┐    ┌──────────────────┐  │   │
│   │  │   Vite Dev   │    │  .NET 8 API  │    │  SQL Server      │  │   │
│   │  │   Server     │    │  Kestrel     │    │  (LocalDB /      │  │   │
│   │  │              │    │              │    │   Express)        │  │   │
│   │  │  :5173       │───►│  :5141 HTTP  │───►│  :1433           │  │   │
│   │  │  (React)     │    │  :7176 HTTPS │    │                  │  │   │
│   │  └──────────────┘    │              │    └──────────────────┘  │   │
│   │                      │  /swagger    │                          │   │
│   │                      │  /health     │    ┌──────────────────┐  │   │
│   │                      │  /hubs/*     │───►│  MongoDB         │  │   │
│   │                      │  /hangfire   │    │  Community       │  │   │
│   │                      └──────────────┘    │  :27017          │  │   │
│   │                                          └──────────────────┘  │   │
│   │                                                                 │   │
│   │                      External Services (Cloud):                │   │
│   │                      ┌──────────────────┐                      │   │
│   │                      │  Firebase Auth   │                      │   │
│   │                      │  Azure Blob      │                      │   │
│   │                      │  Google Cloud    │                      │   │
│   │                      │  SMTP Server     │                      │   │
│   │                      └──────────────────┘                      │   │
│   │                                                                 │   │
│   └─────────────────────────────────────────────────────────────────┘   │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘


┌──────────────────────────────────────────────────────────────────────────┐
│                  TARGET: CLOUD DEPLOYMENT                                │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   ┌──────────────┐         ┌──────────────────────────────────────────┐  │
│   │   CDN        │         │        Cloud Platform (Azure / GCP)     │  │
│   │  (Static     │         │                                          │  │
│   │   Assets)    │         │  ┌──────────────┐  ┌──────────────────┐  │  │
│   │              │         │  │  App Service  │  │  Azure SQL /     │  │  │
│   │  React SPA   │────────►│  │  / Cloud Run  │  │  Cloud SQL       │  │  │
│   │  Build       │         │  │              │──►│                  │  │  │
│   └──────────────┘         │  │  .NET 8 API  │  └──────────────────┘  │  │
│                            │  │              │                        │  │
│                            │  │              │  ┌──────────────────┐  │  │
│                            │  │              │──►│  MongoDB Atlas   │  │  │
│                            │  └──────┬───────┘  └──────────────────┘  │  │
│                            │         │                                │  │
│                            │         │          ┌──────────────────┐  │  │
│                            │         ├─────────►│  Azure Redis     │  │  │
│                            │         │          └──────────────────┘  │  │
│                            │         │                                │  │
│                            │         │          ┌──────────────────┐  │  │
│                            │         ├─────────►│  Azure Blob      │  │  │
│                            │         │          │  Storage         │  │  │
│                            │         │          └──────────────────┘  │  │
│                            │         │                                │  │
│                            │         │          ┌──────────────────┐  │  │
│                            │         └─────────►│  Application     │  │  │
│                            │                    │  Insights        │  │  │
│                            │                    └──────────────────┘  │  │
│                            │                                          │  │
│                            │  ┌──────────────────────────────────┐    │  │
│                            │  │  Firebase Auth (managed by GCP)  │    │  │
│                            │  └──────────────────────────────────┘    │  │
│                            │                                          │  │
│                            └──────────────────────────────────────────┘  │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## Summary

| Aspect | Details |
|--------|---------|
| **Architecture** | Clean Architecture + DDD |
| **Backend** | .NET 8, ASP.NET Core, Minimal API |
| **Frontend** | React 19, TypeScript, Vite, Tailwind |
| **Databases** | SQL Server (OLTP) + MongoDB (Logs/Chat) |
| **Auth** | Firebase + JWT + 5 Authorization Handlers |
| **Real-time** | SignalR (2 Hubs) |
| **Background** | Hangfire (7 Scheduled Jobs) |
| **Aggregates** | 9 DDD Aggregates, 50+ Domain Events |
| **API** | 13 Endpoint Groups |
| **Frontend Pages** | 20 Pages across 4 Role Layouts |
