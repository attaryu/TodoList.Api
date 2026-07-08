FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/TodoList.ApiV2/TodoList.ApiV2.csproj", "apps/TodoList.ApiV2/"]
COPY ["src/TodoList.Contracts/TodoList.Contracts.csproj", "apps/TodoList.Contracts/"]

RUN dotnet restore "apps/TodoList.ApiV2/TodoList.ApiV2.csproj"

COPY ["src/TodoList.ApiV2/", "apps/TodoList.ApiV2/"]
COPY ["src/TodoList.Contracts/", "apps/TodoList.Contracts/"]

WORKDIR "/src/apps/TodoList.ApiV2"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine3.23
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TodoList.ApiV2.dll"]
