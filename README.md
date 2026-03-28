# Trip Planner Microservices (TravelExpense)

This repository contains a small microservices sample for trip & expense management. It includes four service projects under `src/services`:

- expense-service (ExpenseService.Api)
- notification-service (NotificationService.Api)
- trip-service (TripService.Api)
- user-service (UserService.Api)

Each service is a minimal ASP.NET Core Web API and contains a simple `DbContext` pointing to SQL Server.

## Quick start (build)

From the repository root:

```bash
# build the entire solution
dotnet build TripPlanner.sln

# build a single service (example)
dotnet build src/services/expense-service/ExpenseService.Api.csproj
```

## Run a service locally

Each service is a standard ASP.NET Core Web API. Example to run the Expense service:

```bash
cd src/services/expense-service
dotnet run --project ExpenseService.Api.csproj
```

Open `https://localhost:5001` (or the URL printed by the app) to hit the API.

## Connection strings

Each service contains an `appsettings.json` with a sample `ConnectionStrings:DefaultConnection` using SQL Server (example):

```
Server=localhost,1433;Database=ExpenseDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;
```

Notes:
- Replace the sample credentials with your local or hosted SQL Server settings.
- For local development with Docker, use the appropriate service name as the SQL Server host instead of `localhost`.

## EF Core & Migrations

Each project has EF Core packages installed. To add and apply migrations for a service (example: Expense):

```bash
# create a migration
cd src/services/expense-service
dotnet ef migrations add InitialCreate --project ExpenseService.Api.csproj

# apply the migration (ensure DB is reachable)
dotnet ef database update --project ExpenseService.Api.csproj
```

If `dotnet ef` is not available, install the EF tools globally:

```bash
dotnet tool install --global dotnet-ef
```

## Docker

A `deploy/docker-compose.yaml` already exists in this repo. I left Docker setup for you to add since you mentioned you want to learn it yourself. Typical next steps:

- Add a `mcr.microsoft.com/mssql/server` service in the compose file for SQL Server
- Map environment variables for SA password and database ports
- Start the compose stack and point `appsettings.json` connection strings to the SQL Server service hostname

## .gitignore

A top-level `.gitignore` was added to ignore build artifacts, IDE files, node modules, logs, and environment files.

## Projects in the solution

The following projects were added to `TripPlanner.sln`:

- `src/services/expense-service/ExpenseService.Api.csproj`
- `src/services/notification-service/NotificationService.Api.csproj`
- `src/services/trip-service/TripService.Api.csproj`
- `src/services/user-service/UserService.Api.csproj`

## Next suggestions (optional)

- I can add Docker Compose SQL Server and show how to wire connection strings.
- I can create migrations for each project and a small script to initialize the DBs.
- I can commit the current workspace changes (README, .gitignore, new projects) into a git commit.

If you want any of these, tell me which one and I'll proceed.
