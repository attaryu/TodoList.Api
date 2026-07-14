FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/TodoList.EmailConsumer/TodoList.EmailConsumer.csproj", "apps/TodoList.EmailConsumer/"]
COPY ["src/TodoList.Contracts/TodoList.Contracts.csproj", "apps/TodoList.Contracts/"]

RUN dotnet restore "apps/TodoList.EmailConsumer/TodoList.EmailConsumer.csproj"

COPY ["src/TodoList.EmailConsumer/", "apps/TodoList.EmailConsumer/"]
COPY ["src/TodoList.Contracts/", "apps/TodoList.Contracts/"]

WORKDIR /src/apps/TodoList.EmailConsumer
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine3.23

RUN apk add --no-cache \
    krb5-libs \
    icu-libs

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TodoList.EmailConsumer.dll"]