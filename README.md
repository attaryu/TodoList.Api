# A simple todo-list API

Built with .NET 10 for learning purposes.

## Getting Started

### 1. Provide env file
```bash
cp .env.example .env.Development
```

> Don't forget to update the `.env.Development` file!

### 2. Setup Database Services with Docker Compose

```bash
docker compose up -d
```

### 3. Restore Dependencies

```bash
dotnet restore src/TodoList.Api/TodoList.Api.csproj
dotnet restore src/TodoList.EmailConsumer/TodoList.EmailConsumer.csproj
```

### 4. Run The API
```bash
dotnet run --project src/TodoList.Api/TodoList.Api.csproj
```

It works! You can access at:
- API: [http://localhost:5014](http://localhost:5014)
- Swagger UI: [http://localhost:5014/swagger](http://localhost:5014/swagger)
- RabbitMQ Management UI: [http://localhost:15672](http://localhost:15672) (default username/password: guest/guest)

> Note, the app runs on port 5014 for HTTP. Check port 7168 if you run over HTTPS.

### 5. Run The Email Service (Optional)
If you want to try the email-related features, you can run the email consumer service in a separate terminal:

```bash
dotnet run --project src/TodoList.EmailConsumer/TodoList.EmailConsumer.csproj
```

---

## Build Docker Image
To build the Docker image for production, you can use the following command:

```bash
docker compose --env-file .env.Production --profile app build
```

> Note: Make sure to update the `.env.Production` file with the correct production settings.