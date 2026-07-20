FROM mcr.microsoft.com/dotnet/sdk:10.0 AS appbuild
WORKDIR /src

COPY ["src/TodoList.Api/TodoList.Api.csproj", "apps/TodoList.Api/"]
COPY ["src/TodoList.Contracts/TodoList.Contracts.csproj", "apps/TodoList.Contracts/"]

RUN dotnet restore "apps/TodoList.Api/TodoList.Api.csproj"

COPY ["src/TodoList.Api/", "apps/TodoList.Api/"]
COPY ["src/TodoList.Contracts/", "apps/TodoList.Contracts/"]

WORKDIR /src/apps/TodoList.Api
RUN dotnet publish -c Release -o /app/publish

FROM appbuild AS dbmigration
WORKDIR /src/apps/TodoList.Api
ENV PATH $PATH:/root/.dotnet/tools

RUN dotnet tool install --global dotnet-ef
RUN dotnet ef migrations bundle --self-contained -r linux-musl-x64 -o /app/publish/migrations.bundle

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine3.23 AS runner

RUN apk add --no-cache \
    krb5-libs \
    icu-libs

WORKDIR /app

COPY --from=appbuild /app/publish .
COPY --from=dbmigration /app/publish/migrations.bundle .

RUN chmod +x migrations.bundle

EXPOSE 8080

ENTRYPOINT ["dotnet", "TodoList.Api.dll"]