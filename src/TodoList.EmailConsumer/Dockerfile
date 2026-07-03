FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/TodoList.EmailConsumer/TodoList.EmailConsumer.csproj", "src/TodoList.EmailConsumer/"]
COPY ["src/TodoList.Contracts/TodoList.Contracts.csproj", "src/TodoList.Contracts/"]

RUN dotnet restore "src/TodoList.EmailConsumer/TodoList.EmailConsumer.csproj"

COPY ["src/TodoList.EmailConsumer/", "src/TodoList.EmailConsumer/"]
COPY ["src/TodoList.Contracts/", "src/TodoList.Contracts/"]

WORKDIR "/src/src/TodoList.EmailConsumer"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine3.23
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TodoList.EmailConsumer.dll"]