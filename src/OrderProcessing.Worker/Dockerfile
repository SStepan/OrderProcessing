FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["OrderProcessing.sln", "./"]
COPY ["src/OrderProcessing.Worker/OrderProcessing.Worker.csproj", "src/OrderProcessing.Worker/"]
COPY ["src/OrderProcessing.Application/OrderProcessing.Application.csproj", "src/OrderProcessing.Application/"]
COPY ["src/OrderProcessing.Core/OrderProcessing.Core.csproj", "src/OrderProcessing.Core/"]
COPY ["src/OrderProcessing.Infrastructure/OrderProcessing.Infrastructure.csproj", "src/OrderProcessing.Infrastructure/"]
COPY ["src/OrderProcessing.Contracts/OrderProcessing.Contracts.csproj", "src/OrderProcessing.Contracts/"]

RUN dotnet restore "src/OrderProcessing.Worker/OrderProcessing.Worker.csproj"

COPY . .

WORKDIR "/src/src/OrderProcessing.Worker"
RUN dotnet build "OrderProcessing.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderProcessing.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "OrderProcessing.Worker.dll"]