# Todo List API

A simple todo-list API built with **.NET 10**, created for learning purposes.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) and Docker Compose

## Getting Started

### 1. Set up environment variables

Copy the example env file and fill in your own values:

```bash
cp .env.example .env.Development
```

> ⚠️ Update the values in `.env.Development` if needed. But the default values already work for local development.

### 2. Run the project

Choose one of the two options below.

#### Option A — Docker Compose (recommended, full features)

Runs the API, database, and email service together.

```bash
docker compose --profile app up --build -d
```

Once it's up, open the API docs at [http://localhost:80/swagger](http://localhost:80/swagger).

If you want to see email notifications, go to [Ethereal Email](https://ethereal.email/login) and login with the credentials in `.env.Development` file.

#### Option B — Manual (no email features)

Use this if you just want to run the API and database without the email consumer.

1. **Start the database services**

   ```bash
   docker compose up -d
   ```

2. **Restore dependencies and run the API**

   ```bash
   # restore
   dotnet restore src/TodoList.Api/TodoList.Api.csproj
   # run
   dotnet run --project src/TodoList.Api/TodoList.Api.csproj
   ```

3. **(Optional) Run the email service**

   In a separate terminal:

   ```bash
   # restore
   dotnet restore src/TodoList.EmailConsumer/TodoList.EmailConsumer.csproj
   # run
   dotnet run --project src/TodoList.EmailConsumer/TodoList.EmailConsumer.csproj
   ```

## Building for Production

Build the Docker image using your production env file:

```bash
docker compose --env-file .env.Production --profile app build
```

> ⚠️ Make sure `.env.Production` contains the correct production settings before building.

## Stopping the Project
 
- **If you ran everything with Docker Compose (Option A):**
    ```bash
    docker compose --profile app down
    ```
 
- **If you only started the database services (Option B):**
    ```bash
    docker compose down
    ```
 
- To stop the manually-run API and/or email consumer, just press `Ctrl + C` in their respective terminals.
> 💡 Add the `-v` flag (e.g. `docker compose down -v`) if you also want to remove the database volumes and start fresh next time.