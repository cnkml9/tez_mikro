#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/PaymnetService/PaymnetService.Api/PaymnetService.Api.csproj", "src/Services/PaymnetService/PaymnetService.Api/"]
COPY ["src/BuildingBlock/EventBus/EventBus.Base/EventBus.Base.csproj", "src/BuildingBlock/EventBus/EventBus.Base/"]
COPY ["src/BuildingBlock/EventBus/EventBus.Factory/EventBus.Factory.csproj", "src/BuildingBlock/EventBus/EventBus.Factory/"]
COPY ["src/BuildingBlock/EventBus/EventBus.AzureServiceBus/EventBus.AzureServiceBus.csproj", "src/BuildingBlock/EventBus/EventBus.AzureServiceBus/"]
COPY ["src/BuildingBlock/EventBus/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "src/BuildingBlock/EventBus/EventBus.RabbitMQ/"]
RUN dotnet restore "src/Services/PaymnetService/PaymnetService.Api/PaymnetService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/PaymnetService/PaymnetService.Api"
RUN dotnet build "PaymnetService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymnetService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymnetService.Api.dll"]