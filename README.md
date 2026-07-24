# Trainee Management API

The **Trainee Management System** is a backend application built using **ASP.NET Core Web API** for managing trainees, mentors, learning tasks, task assignments, submissions, reviews, and asynchronous file processing.

The application uses **MySQL** as the primary database, **Redis** for distributed caching, **RabbitMQ** for asynchronous message processing, and **Docker Compose** to run the complete backend stack locally.

## Technology Used
* ASP.NET Core Web API
* C#
* MySql
* Redis
* RabbitMq
* Docker
* Swagger

## Project Structure

```text
TraineeManagement
│
├── TraineeManagement.Api
│   ├── Configurations
│   ├── Constants
│   ├── Controllers
│   ├── DTOs
│   ├── Exceptions
│   ├── Helpers
│   ├── Interfaces
│   ├── Middleware
│   └── Services
│
├── TraineeManagement.Shared
│   ├── Models
│   ├── Enums
│   ├── Contracts
│   ├── Data
│   ├── Configurations
│   └── Helpers
│
├── TraineeManagement.Worker
│   ├── Interfaces
│   └── Services
│
├── TrainingDirectory.Api
│   ├── Models
│   └── Controllers
│
├── docker-compose.yml
├── .env
└── README.md
```

## Clone Repository

Clone the repository using Git.

```bash
git clone https://github.com/ishteqali/TraineeManagement.Api.git

```

---

## Restore Dependencies

Restore all NuGet packages.

```bash
dotnet restore
```

---

## Build the Solution

```bash
dotnet build
```

The project should build successfully without any errors.

---

## Project Configuration

The application uses environment variables for sensitive configuration such as database credentials, Redis, RabbitMQ, JWT settings and Docker configuration.

Create a file named

```text
.env
```

inside the project root.

Example:

```properties
# -----------------------------
# Database
# -----------------------------
DbSettings__Host=localhost
DbSettings__Port=3306
DbSettings__Database=trainee_management_db
DbSettings__User=root
DbSettings__Password=your_password

# -----------------------------
# JWT
# -----------------------------
Jwt__Issuer=TraineeManagementApi
Jwt__Audience=TraineeManagementClient
Jwt__ExpiryMinutes=60
Jwt__Key=YourSuperSecretKey

# -----------------------------
# Redis
# -----------------------------
Redis__ConnectionString=localhost:6379
Redis__InstanceName=TraineeManagement:

# -----------------------------
# RabbitMQ
# -----------------------------
RabbitMq__Host=localhost
RabbitMq__Port=5672
RabbitMq__Username=guest
RabbitMq__Password=guest
RabbitMq__VirtualHost=/

# -----------------------------
# Training Directory API
# -----------------------------
TrainingDirectory__BaseUrl=http://localhost:5002
```

> **Note**
>
> When running with **Docker Compose**, the service names replace `localhost`.
>
> Example:
>
> ```properties
> DbSettings__Host=mysql
> Redis__ConnectionString=redis:6379
> RabbitMq__Host=rabbitmq
> TrainingDirectory__BaseUrl=http://trainingdirectory-api:8080
> ```

---

## Running the Application without Docker

Apply EF Core migrations.

```bash
dotnet ef database update -p Shared -s TraineeManagement.Api
```

Run the API.

```bash
dotnet run --project TraineeManagement.Api
```

The application will start and Swagger UI will be available at:

```
http://localhost:5119/swagger
```

## Running the Application using Docker Compose

The project supports Docker Compose for running the complete backend environment. All required services are started together, allowing the application to run without installing MySQL, Redis or RabbitMQ locally.

### Services

Docker Compose starts the following containers:

| Service | Description |
|----------|-------------|
| TraineeManagement.Api | Main ASP.NET Core Web API |
| TraineeManagement.Worker | Background Worker Service |
| TrainingDirectory.Api | Internal service for inter-service communication |
| MySQL | Primary relational database |
| Redis | Distributed cache |
| RabbitMQ | Message broker for asynchronous processing |


### Build Docker Images

Run the following command from the project root.

```bash
docker compose build
```

---

### Start All Services

```bash
docker compose up -d
```

This command starts all required containers in the background.

---

### Stop All Services

```bash
docker compose down
```

---

### Rebuild after Code Changes

Whenever application code is modified, rebuild the images before starting the containers.

```bash
docker compose up --build -d
```

---

### View Running Containers

```bash
docker ps
```

---

### View Container Logs

#### API

```bash
docker compose logs traineemanagement-api
```

#### Worker

```bash
docker compose logs traineemanagement-worker
```

#### Training Directory API

```bash
docker compose logs trainingdirectory-api
```

#### RabbitMQ

```bash
docker compose logs rabbitmq
```

#### MySQL

```bash
docker compose logs mysql
```

#### Redis

```bash
docker compose logs redis
```

---

### Docker Container Ports

| Service | Container Port | Host Port |
|----------|---------------|-----------|
| TraineeManagement.Api | 8080 | 5001 |
| TrainingDirectory.Api | 8080 | 5002 |
| MySQL | 3306 | 3306 |
| Redis | 6379 | 6379 |
| RabbitMQ | 5672 | 5672 |
| RabbitMQ Management | 15672 | 15672 |

---

### Access URLs

| Service | URL |
|----------|-----|
| Swagger UI | http://localhost:5001/swagger |
| Training Directory API | http://localhost:5002/swagger |
| RabbitMQ Management | http://localhost:15672 |

---

### Docker Volumes

The project uses Docker volumes for persistent storage.

- MySQL Database
- Redis Data
- RabbitMQ Data
- Uploaded Files

Data remains available even after restarting the containers.

---

### Health Check Endpoints

Application health can be verified using the following endpoints.

#### Liveness

```
GET /health/live
```

#### Readiness

```
GET /health/ready
```

The readiness endpoint verifies connectivity with:

- MySQL
- Redis
- RabbitMQ
- TrainingDirectory.Api

---

### Notes

- Containers communicate using Docker service names instead of `localhost`.
- MySQL host inside Docker is `mysql`.
- Redis host inside Docker is `redis`.
- RabbitMQ host inside Docker is `rabbitmq`.
- Training Directory API host inside Docker is `trainingdirectory-api`.

## Database Setup

The application uses **MySQL** as the primary relational database and **Entity Framework Core** for database access.

The database schema is created using EF Core migrations.

---

### Create Database

Create a MySQL database.

```sql
CREATE DATABASE trainee_management_db;
```

---

### Configure Database

Update your database configuration inside the `.env` file.

```properties
DbSettings__Host=localhost
DbSettings__Port=3306
DbSettings__Database=trainee_management_db
DbSettings__User=root
DbSettings__Password=your_password
```

When running with Docker Compose:

```properties
DbSettings__Host=mysql
DbSettings__Port=3306
DbSettings__Database=trainee_management_db
DbSettings__User=root
DbSettings__Password=your_password
```

---

### Apply Database Migrations

Run the following command to create all tables.

```bash
dotnet ef database update \
  --project TraineeManagement.Shared \
  --startup-project TraineeManagement.Api
```

---

### Create a New Migration

After making changes to your entity models, create a new migration.

```bash
dotnet ef migrations add MigrationName \
  --project TraineeManagement.Shared \
  --startup-project TraineeManagement.Api
```

Example:

```bash
dotnet ef migrations add AddSubmissionProcessing
```

---

### Update Database After New Migration

```bash
dotnet ef database update \
  --project TraineeManagement.Shared \
  --startup-project TraineeManagement.Api
```

---

### Remove the Last Migration

If the migration has not been applied to the database:

```bash
dotnet ef migrations remove \
  --project TraineeManagement.Shared \
  --startup-project TraineeManagement.Api
```

---

### List Available Migrations

```bash
dotnet ef migrations list \
  --project TraineeManagement.Shared \
  --startup-project TraineeManagement.Api
```

---

### Verify Database

After applying migrations, the database should contain tables similar to:

- Trainees
- Mentors
- LearningTasks
- TaskAssignments
- Submissions
- SubmissionFiles
- Reviews
- SubmissionProcessingJobs
- Roles
- Users

---

### Notes

- Always apply migrations before running the application.
- Do not edit migration files manually unless necessary.
- Keep the migration history under source control.
- Use the Docker service name (`mysql`) instead of `localhost` when the application runs inside Docker containers.

## Login Credentials for testing
```bash
{
    "username": "admin",
    "password": "admin@123"
}
```

## JWT usage instructions
1. Login using `POST /api/auth/login` endpoint with login credentials
2. After successful login, response body will contain jwt token and copy it
3. In Swagger UI, there is authorize button on top of the page click it and paste token and then click authorize
4. Now we can use all the secure api after login using this token till token get expire

## API List

### Health
* GET - /api/health - getting system running status
* GET - /health/live - liveness check
* GET - /health/ready - Readiness check

### Auth
* POST - /api/auth/login - user login endpoint

### Trainee
* GET - /api/trainees?pageNumber=1&pageSize=10&search={searchTerm}&status={status} - get all trainee matching the keyword with pagination
* GET - /api/trianees/{id} - get specific trainee by id
* POST - /api/trainees - add new trainee
* PUT - /api/trainees - update trainee by id
* DELETE - /api/trainees - delete trainee by id

### Mentor
* GET - /api/mentors?pageNumber=1&pageSize=10&search={searchTerm}&status={status} - get all mentor matching the keyword with pagination
* GET - /api/mentors/{id} - get specific mentor by id
* POST - /api/mentors - add new mentor
* PUT - /api/mentors - update mentor by id
* DELETE - /api/mentor - delete mentor by id

### Learning Task
* GET - /api/learning-tasks?pageNumber=1&pageSize=10&search={searchTerm}&status={status} - get all learning task matching the keyword with pagination
* GET - /api/learning-tasks/{id} - get specific learning task by id
* POST - /api/learning-tasks - add new learning task
* PUT - /api/learning-tasks - update learning task by id
* DELETE - /api/learning-tasks - delete learning task by id

### Task Assigment 
* POST - /api/task-assignments - add new task assignment
* GET - /api/task-assignments - get all task assignment 
* GET - /api/task-assignments/{id} - get specific task assignment by id
* PUT - /api/task-assignments/{id}/status - update task assignment status

### Submission
* POST - /api/submissions -  add new submission
* GET - /api/submissions - get all submission
* GET - /api/submissions/{id} - get specific submission by id

### Review
* POST - /api/reviews - add new review
* GET - /api/reviews - get all review 
* GET - /api/reviews/{id} - get specific review by id   

### Submission Files
* POST - /api/submissions/{submissionId}/files - add submission file for provided submission id
* GET - /api/submission-files/{id} - get submission file meta data for specific submission file by id
* DELETE - /api/submission-files/{id} - delete specific submission file by id
* GET - /api/submission-files/download/{id} - download submission file by id  

### Processing Jobs
* GET - /api/processing-jobs/{id} - get processing job information

---

## Response Status Codes

| Status Code | Meaning |
|--------------|---------|
| 200 | Request completed successfully |
| 201 | Resource created successfully |
| 202 | Request accepted for background processing |
| 204 | Request completed with no content |
| 400 | Invalid request |
| 401 | Authentication required |
| 403 | Permission denied |
| 404 | Resource not found |
| 409 | Conflict |
| 500 | Internal server error |

---

## Sample Request JSON

for POST & PUT (/api/trainees)
```bash
{
    "firstName": "Ishteqali",
    "lastName": "Khan",
    "email": "ishteqalikhan@gmail.com",
    "techStack": "C#",
    "status": "Active"
}
```

## Sample Response JSON

for GET (/api/trainees/{id})
```bash
{
    "firstName": "Ishteqali",
    "lastName": "Khan",
    "email": "ishteqalikhan@gmail.com",
    "techStack": "ASP.NET",
    "status": "Active"
}
```

## Features Completed

### Phase 1:
#### Day 1 (3rd July, 2026)
    -Swagger UI for testing Api
    -In-memory data storage using List<Trainee>
    -Health Check endpoint (GET) /api/health
    -Get all Trainees (GET) /api/trainees
    -Get Trainee by Id (GET/{id}) /api/trainees/{id}
    -Add new Trainee (POST) /api/trainees

### Day 2 (6th July, 2026)
    -Update Trainee details (PUT) /api/trainees
    -Delete Trainee (DELETE) /api/trainees
    -Request and response DTOs
    -Input Validation
    -Service Layer Implementation

### Day 3 (7th July, 2026)
    -Added EntityFrameworkCore
    -Added Async functions
    -Added a Search function

### Phase 2:
#### Day 1 (8th July, 2026)
    -Replaced InMemoryDatabase with MySQL database using EF Core
    -Made connection to MySQL database successfully 
    -Created Migration for Trainee and MySQL database schema created for Trainee
    -All CRUD ( Create, Read, Update, and Delete) API working with MySQL
    -After restarting server record still exists in database

#### Day 2 (9th July, 2026)
    -User table is created and only hash password is stored 
    -Admin user is created by data seeding (username: admin, password: admin@123)
    -POST /api/auth/login endpoint created with JWT token

#### Day 3 (10th July, 2026)
    -Secured api endpoints with JWT Role-Based Access Control
    -Added status filter along with search query and pagination
    -Configured CORS for future React Frontend
    -Implemented structured logging and saving of logs at Logs/api-log-{date}.txt
    -Used .env for database connection

#### Day 4 (13th July, 2026)
    -Done Mentor Module Completely with Models, DTOs, Service, Controller and api is protected using JWT
    -Done Learning Task Module Completely with Models, DTOs, Service, Controller and api is protected using JWT
    -MySQL schema for Mentor and Learning Task created with all migrations

#### Day 5 (14th July, 2026)
    -Completed Task Assignment, Submission and Review Module with all required endpoints and api is protected using JWT
    -Added Global Exception Handling Middleware for unexpected errors
    -Validated System against OWASP API security standards

### Phase 3:
#### Day 1 (15th July, 2026)
    -Created File Abstraction for finding, reading, saving and deleting file with seperating logic for file handling in 'LocalFileStorageService.cs' and for validation and controller communication in 'SubmissionFileService.cs'
    -Created all api for upload, download, and delete
    -added validation for file upload for malicious upload concern

#### Day 2 (16th July, 2026)
    -Installed Docker for redis
    -Implemented caching on getting trainee, task assignment and submission
    -Added cache invalidation for updated and deleted data
    -Added Logging of cache failure 

#### Day 3 (17th July, 2026)
    -Added and configured Rabbitmq
    -Created a Versioned Contract for producer and consumer 
    -Message is published by producer 
    -Created a basic worker project (TraineeManagement.Api)

#### Day 4 (20th July, 2026)
    -Implemented SubmissionProcess in worker project
    -Simulating processing using checksum validation
    -Added job tracking api for tracking jobs (GET /api/processing-jobs/{id})
    -Created Consumer idempotent and added retry and failed message handling (adding dead letter queue)

#### Day 5 (21th July, 2026)
    -Created new project for simulating inter-service communication (TrainingDirectory.Api)
    -Created an api in this new project for testing (GET /api/trainees/{id})
    -Added resilience controls for timeout and added fall back when service is unavailable
    -Added this api in main application for testing this (GET /api/training-directory/trainees/{id})

#### Day 6 (22th July, 2026)
    -Added structured logging for every service
    -Added health checks apis (GET /health/live and GET /health/ready)
    -Putting everything in docker and using docker compose to run whole as a one 
    -Tested every services running after using docker compose

#### Day 7 (23th July, 2026)
    -Testing whole appication every services
    -Completed documentation
    -Created Architecture diagram


## Security Features

The application follows secure coding practices to protect user data and uploaded files.

Implemented:

### Authentication & Authorization

- JWT Authentication
- Role-Based Authorization
- Protected API Endpoints
- Secure Token Validation

---

### Input Validation

- Model Validation using Data Annotations
- Request Validation
- Custom Validation Messages
- Global Exception Handling

---

### File Security

- Allowed File Extension Validation
- Maximum File Size Validation
- SHA-256 Checksum Generation
- SHA-256 Checksum Validation
- Secure Local File Storage
- Duplicate File Detection

---

### API Security

- Standard HTTP Status Codes
- Proper Error Responses
- Centralized Exception Handling Middleware
- Structured Logging using Serilog

---

### Messaging Security

- Durable RabbitMQ Queues
- Persistent Messages
- Correlation ID for Request Tracking
- Idempotent Message Processing

---

### Distributed Cache

- Cache-Aside Pattern
- Cache Invalidation
- Configurable Cache Expiration
- No Sensitive Data Cached

---

### Docker Security

- Environment Variables for Configuration
- Isolated Containers
- Persistent Volumes
- Service-to-Service Communication using Docker Network

---

### Health Monitoring

- MySQL Health Check
- Redis Health Check
- RabbitMQ Health Check
- TrainingDirectory API Health Check
- Liveness Endpoint
- Readiness Endpoint

## Improvements Planned

* Integrating SQL database (Completed)
* Adding Authentications for api endpoints (Completed)
* Adding React UI 

## Security Checklist
* User Authentication using JWT token
* Hashing and storing of password in database
* Logging of all events and password and token doesn't log
* Global Exception Handling
* No hardcoding of secrets
* CORS restricted to expected origin 
* Protected APIs require token 

## Next Improvement Areas
* Adding Frontend UI 