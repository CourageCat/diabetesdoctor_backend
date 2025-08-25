FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MediaService.Web/MediaService.Web.csproj", "src/MediaService.Web/"]
COPY ["src/MediaService.Application/MediaService.Application.csproj", "src/MediaService.Application/"]
COPY ["src/MediaService.Contract/MediaService.Contract.csproj", "src/MediaService.Contract/"]
COPY ["src/MediaService.Domain/MediaService.Domain.csproj", "src/MediaService.Domain/"]
COPY ["src/MediaService.Persistence/MediaService.Persistence.csproj", "src/MediaService.Persistence/"]
COPY ["src/MediaService.Infrastructure/MediaService.Infrastructure.csproj", "src/MediaService.Infrastructure/"]
COPY ["src/MediaService.Presentation/MediaService.Presentation.csproj", "src/MediaService.Presentation/"]
COPY ["src/MediaService.ServiceDefaults/MediaService.ServiceDefaults.csproj", "src/MediaService.ServiceDefaults/"]
RUN dotnet restore "src/MediaService.Web/MediaService.Web.csproj"
COPY . .
WORKDIR "/src/src/MediaService.Web"
RUN dotnet build "./MediaService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MediaService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaService.Web.dll"]
