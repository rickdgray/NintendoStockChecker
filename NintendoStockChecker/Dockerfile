FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NintendoStockChecker/NintendoStockChecker.csproj", "NintendoStockChecker/"]
RUN dotnet restore "NintendoStockChecker/NintendoStockChecker.csproj"
COPY . .
WORKDIR "/src/NintendoStockChecker"
RUN dotnet build "NintendoStockChecker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NintendoStockChecker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NintendoStockChecker.dll"]