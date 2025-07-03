// Program.cs

using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApi.Models; // Убедитесь, что это пространство имен соответствует вашему ExpenseTrackerDbContext и моделям
using Microsoft.AspNetCore.Cors.Infrastructure;
using ExpenseTrackerApi.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Настройка сервисов для CORS ---
// Добавление сервисов CORS в контейнер зависимостей приложения
builder.Services.AddCors(options =>
{
    // Определение CORS-политики с именем "AllowFrontendOrigin"
    options.AddPolicy("AllowFrontendOrigin",
        policy =>
        {
            // Указываем разрешенные источники (домены/порты), с которых могут приходить запросы.
            // Для вашего React-приложения, которое запускается Vite в режиме разработки, это обычно http://localhost:5173
            // В продакшене здесь будет домен вашего фронтенда, например, "https://your-frontend-domain.com"
            policy.WithOrigins("http://localhost:5173") // <-- УБЕДИТЕСЬ, ЧТО ЭТОТ ПОРТ СООТВЕТСТВУЕТ ВАШЕМУ ФРОНТЕНДУ!
                  .AllowAnyHeader()    // Разрешаем любые HTTP-заголовки в запросах от фронтенда
                  .AllowAnyMethod();   // Разрешаем любые HTTP-методы (GET, POST, PUT, DELETE) от фронтенда
        });
});
// ------------------------------------


// --- 2. Настройка сервисов для контроллеров, Swagger/OpenAPI и Entity Framework Core ---
// Добавление сервисов для работы с контроллерами (вашими API-эндпоинтами)
builder.Services.AddControllers();

// Добавление сервисов для генерации документации API (Swagger/OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка подключения к базе данных PostgreSQL с использованием Entity Framework Core
// Строка подключения берется из конфигурации приложения (appsettings.json или переменных окружения)
// В Docker Compose мы передаем её через переменные окружения
builder.Services.AddDbContext<ExpenseTrackerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// -------------------------------------------------------------------------------------


// --- 3. Построение приложения и настройка HTTP-конвейера запросов ---
var app = builder.Build();

// Конфигурация HTTP request pipeline (последовательность обработки запросов)
// Swagger UI доступен только в режиме разработки для удобства тестирования API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Перенаправление HTTP-запросов на HTTPS (рекомендуется для продакшена)
app.UseHttpsRedirection();

// --- 4. Применение CORS-политики ---
// Применение созданной ранее CORS-политики к HTTP-конвейеру обработки запросов
// Это должно быть ДО UseAuthorization() и UseEndpoints()
app.UseCors("AllowFrontendOrigin"); // <-- Применяем политику по её имени "AllowFrontendOrigin"
// ----------------------------------


// --- 5. Настройка авторизации и маршрутизации контроллеров ---
// Включение системы авторизации (если она есть, хотя у нас пока нет явной авторизации)
app.UseAuthorization();

// Маппинг контроллеров, чтобы запросы направлялись в соответствующие методы ваших контроллеров
app.MapControllers();
// -----------------------------------------------------------


// --- 6. Запуск приложения ---
app.Run();