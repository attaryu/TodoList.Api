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

## Build Docker Image
Make sure to specify the `APP_ENV` environment variable when building the Docker image to ensure it uses the correct configuration. For example, if you want to build the image for production, you can run:
```bash
APP_ENV=Production docker build -t todolist-api .
```
That's it! You can now run the Docker container with the built image. Make sure to set the appropriate environment variables when running the container to ensure it connects to the correct database and services.

## Build and Run with Docker Compose
Same as above, you have to specify the `APP_ENV` environment variable when using Docker Compose to build and run the application. Here's how you can do it:
```bash
APP_ENV=Production docker compose up --build -d
```
