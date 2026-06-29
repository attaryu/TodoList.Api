# A simple todo-list API
Built with .NET 10 for learning purposes.

## Getting Started
### 1. Provide env file
```bash
cp .env.example .env.Development
```
Don't forget to update the `.env.Development` file!

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Setup Docker for Database Services
```bash
docker compose up -d
```

### 4. Apply Database Migrations
```bash
dotnet ef database update
```

### 5. Run the application
```bash
dotnet run
```
It works! You can access at `http://localhost:5014` (or `https://localhost:7168` for HTTPS).The API Documentation is available at `http://localhost:5014/swagger`.

---

## Run with Docker Compose
To run the application with Docker Compose, make sure to specify the env file. Here's how you can do it:

```bash
docker compose --env-file .env.Development up -d
```
---

> Note: If you want to run the application in production mode, you can use the `.env.Production` file instead of `.env.Development`. Just make sure to update the `.env.Production` file with the correct production settings.

## Build Docker Image
To build the Docker image for production, you can use the following command:

```bash
docker compose --env-file .env.Production --profile app build -t todolist-api .
```