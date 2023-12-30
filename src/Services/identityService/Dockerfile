#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5005

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IdentityService.API/IdentityService.API.csproj", "IdentityService.API/"]
COPY ["Mikroservices.Application/Mikroservices.Application.csproj", "Mikroservices.Application/Mikroservices.Application/"]
COPY ["Mikroservices.Domain/Mikroservices.Domain.csproj", "Mikroservices.Domain/"]
COPY ["Mikroservices.Infrastructure/Mikroservices.Infrastructure.csproj", "Mikroservices.Infrastructure/"]
COPY ["Mikroservices.Persistence/Mikroservices.Persistence.csproj", "Mikroservices.Persistence/"]
RUN dotnet restore "IdentityService.API/IdentityService.API.csproj"
COPY . .
WORKDIR "/src/IdentityService.API"
RUN dotnet build "IdentityService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.API.dll"]