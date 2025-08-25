FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

EXPOSE 8080
EXPOSE 8081
    
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ChatService.Web/ChatService.Web.csproj", "src/ChatService.Web/"]
COPY ["src/ChatService.Application/ChatService.Application.csproj", "src/ChatService.Application/"]
COPY ["src/ChatService.Contract/ChatService.Contract.csproj", "src/ChatService.Contract/"]
COPY ["src/ChatService.Domain/ChatService.Domain.csproj", "src/ChatService.Domain/"]
COPY ["src/ChatService.Persistence/ChatService.Persistence.csproj", "src/ChatService.Persistence/"]
COPY ["src/ChatService.Infrastructure/ChatService.Infrastructure.csproj", "src/ChatService.Infrastructure/"]
COPY ["src/ChatService.Presentation/ChatService.Presentation.csproj", "src/ChatService.Presentation/"]
RUN dotnet restore "src/ChatService.Web/ChatService.Web.csproj"
COPY . .
WORKDIR "/src/src/ChatService.Web"
RUN dotnet build "./ChatService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChatService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatService.Web.dll"]
