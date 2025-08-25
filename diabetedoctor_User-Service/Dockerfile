FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/UserService.Web/UserService.Web.csproj", "src/UserService.Web/"]
COPY ["src/UserService.Contract/UserService.Contract.csproj", "src/UserService.Contract/"]
COPY ["src/UserService.Presentation/UserService.Presentation.csproj", "src/UserService.Presentation/"]
COPY ["src/UserService.Application/UserService.Application.csproj", "src/UserService.Application/"]
COPY ["src/UserService.Domain/UserService.Domain.csproj", "src/UserService.Domain/"]
COPY ["src/UserService.Persistence/UserService.Persistence.csproj", "src/UserService.Persistence/"]
COPY ["src/UserService.Infrastructure/UserService.Infrastructure.csproj", "src/UserService.Infrastructure/"]
COPY ["src/UserService.ServiceDefaults/UserService.ServiceDefaults.csproj", "src/UserService.ServiceDefaults/"]
RUN dotnet restore "src/UserService.Web/UserService.Web.csproj"
COPY . .
WORKDIR "/src/src/UserService.Web"
RUN dotnet build "./UserService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.Web.dll"]
