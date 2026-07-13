# Trainee Management API

The Trainee Management API is a backend web service that creates, updates, deletes,and manages trainee profiles for an organization. All Trainee Data is stored in InMemoryDatabase. It is also contain search query parameter in GET request for searching all Trainee records that matches that query. It provides input validaton for all request and shows only required data in response.

## Technology Used
* ASP.NET Core Web API
* C#
* Swagger

## How to Run

Follow these step to build and run this .NET project (Trainee Management API)

### 1. Prerequisite Check
Install .NET sdk for windows or Ubuntu from official sources

### 2. Clone and Open this project directory

### 3. Restore dependencies
```bash
dotnet restore
```

### 4. Build and Run the .NET project
```bash
dotnet run
```
Open browser and go to the url: `http://localhost:5119/swagger/index.html` where application is running 

## Database Setup (MySQL)
This project uses MySQL database for data storage

### 1. Run MySQL server on default port 3306

### 2. Create database for this application and named it `trainee_management_db`

### 3. Create `.env` file and add the values like MySQL username, password, etc. in below format
```bash
DbSettings__Host=localhost
DbSettings__Port=3306
DbSettings__Database=trainee_management_db
DbSettings__User=username
DbSettings__Password=password
```

### 4. Add neccessary dotnet packages
```bash
dotnet add package MySql.EntityFrameworkCore
```

### 5. Run this command on project root for creating database schema
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## API List

* GET - /api/trainees - get all trainee
* GET - /api/trainees?pageNumber=1&pageSize=10&search={searchTerm}&status={status} - get all trainee matching the keyword
* GET - /api/trianees/{id} - get specific trainee by id
* POST - /api/trainees - add new trainee
* PUT - /api/trainees - update trainee by id
* DELETE - /api/trainees - delete trainee by id

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

## Known Limitations

* No Authentication (fixed)

## Improvements Planned

* Integrating SQL database (Completed)
* Adding Authentications for api endpoints (Completed)
