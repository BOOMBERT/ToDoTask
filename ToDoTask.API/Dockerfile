FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ToDoTask.API/ToDoTask.API.csproj", "ToDoTask.API/"]
COPY ["ToDoTask.Application/ToDoTask.Application.csproj", "ToDoTask.Application/"]
COPY ["ToDoTask.Domain/ToDoTask.Domain.csproj", "ToDoTask.Domain/"]
COPY ["ToDoTask.Infrastructure/ToDoTask.Infrastructure.csproj", "ToDoTask.Infrastructure/"]
RUN dotnet restore "./ToDoTask.API/ToDoTask.API.csproj"
COPY . .
WORKDIR "/src/ToDoTask.API"
RUN dotnet build "./ToDoTask.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ToDoTask.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoTask.API.dll"]