#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CatagolService.API/CatagolService.API.csproj", "CatagolService.API/"]
COPY ["Mikroservice.Application/Mikroservice.Application.csproj", "Mikroservice.Application/"]
COPY ["Mikroservice.Domain/Mikroservice.Domain.csproj", "Mikroservice.Domain/"]
COPY ["Mikroservice.Infrastructure/Mikroservice.Infrastructure.csproj", "Mikroservice.Infrastructure/"]
COPY ["Mikroservice.Persistence/Mikroservice.Persistence.csproj", "Mikroservice.Persistence/"]
RUN dotnet restore "CatagolService.API/CatagolService.API.csproj"
COPY . .
WORKDIR "/src/CatagolService.API"
RUN dotnet build "CatagolService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatagolService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatagolService.API.dll"]