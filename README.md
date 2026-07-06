# Trainee Management API

## Technology Used
* ASP.NET Core Web API
* C#
* Swagger

## How to Run

Follow these step to build and run this .NET project (Trainee Management API)

### 1. Prerequisite Check
Install .NET sdk for windows or Ubuntu from official sources

### 2. Clone and Open this project directory

### 3. restore dependencies
```bash
dotnet restore
```

### 4. Build and Run the .NET project
```bash
dotnet run
```
Open browser and go to the url: `http://localhost:5119/swagger/index.html` where application is running 


## API List

* GET - /api/trainees - get all trainee
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

## Known Limitations

* No Database integrations
* Deleted Trainee's id cannot be reuse
