# Этап сборки: Используем образ .NET SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Изменено: Указываем путь к csproj относительно текущего контекста (./ExpenseTrackerApi)
COPY ["ExpenseTrackerApi.csproj", "ExpenseTrackerApi/"]
RUN dotnet restore "ExpenseTrackerApi/ExpenseTrackerApi.csproj"
COPY . .
WORKDIR "/src/ExpenseTrackerApi"
RUN dotnet build "ExpenseTrackerApi.csproj" -c Release -o /app/build

# Этап публикации: Создаём готовое к развёртыванию приложение
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /src
# Изменено: Указываем путь к csproj относительно текущего контекста (./ExpenseTrackerApi)
COPY ["ExpenseTrackerApi.csproj", "ExpenseTrackerApi/"]
RUN dotnet restore "ExpenseTrackerApi/ExpenseTrackerApi.csproj"
COPY . .
WORKDIR "/src/ExpenseTrackerApi"
RUN dotnet publish "ExpenseTrackerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап запуска: Используем минимальный образ .NET Runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTrackerApi.dll"]