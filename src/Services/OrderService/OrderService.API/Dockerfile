#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/OrderService/OrderService.API/OrderService.API.csproj", "src/Services/OrderService/OrderService.API/"]
COPY ["src/BuildingBlock/EventBus/EventBus.Base/EventBus.Base.csproj", "src/BuildingBlock/EventBus/EventBus.Base/"]
COPY ["src/Services/OrderService/OrderService.Appliation/OrderService.Appliation.csproj", "src/Services/OrderService/OrderService.Appliation/"]
COPY ["src/BuildingBlock/EventBus/EventBus.Factory/EventBus.Factory.csproj", "src/BuildingBlock/EventBus/EventBus.Factory/"]
COPY ["src/BuildingBlock/EventBus/EventBus.AzureServiceBus/EventBus.AzureServiceBus.csproj", "src/BuildingBlock/EventBus/EventBus.AzureServiceBus/"]
COPY ["src/BuildingBlock/EventBus/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "src/BuildingBlock/EventBus/EventBus.RabbitMQ/"]
COPY ["src/Services/OrderService/OrderService.Domain/OrderService.Domain.csproj", "src/Services/OrderService/OrderService.Domain/"]
COPY ["src/Services/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "src/Services/OrderService/OrderService.Infrastructure/"]
RUN dotnet restore "src/Services/OrderService/OrderService.API/OrderService.API.csproj"
COPY . .
WORKDIR "/src/src/Services/OrderService/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]