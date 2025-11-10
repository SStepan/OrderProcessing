FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["OrderProcessing.sln", "./"]
COPY ["src/OrderProcessing.Api/OrderProcessing.Api.csproj", "src/OrderProcessing.Api/"]
COPY ["src/OrderProcessing.Application/OrderProcessing.Application.csproj", "src/OrderProcessing.Application/"]
COPY ["src/OrderProcessing.Core/OrderProcessing.Core.csproj", "src/OrderProcessing.Core/"]
COPY ["src/OrderProcessing.Infrastructure/OrderProcessing.Infrastructure.csproj", "src/OrderProcessing.Infrastructure/"]
COPY ["src/OrderProcessing.Contracts/OrderProcessing.Contracts.csproj", "src/OrderProcessing.Contracts/"]

RUN dotnet restore "src/OrderProcessing.Api/OrderProcessing.Api.csproj"

COPY . .

WORKDIR "/src/src/OrderProcessing.Api"
RUN dotnet build "OrderProcessing.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderProcessing.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "OrderProcessing.Api.dll"]