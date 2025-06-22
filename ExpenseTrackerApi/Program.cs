using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.Models;

namespace ExpenseTrackerApi // Добавьте это!
{
    public class Program // Добавьте это!
    {
        public static void Main(string[] args) // Добавьте это!
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Добавляем CORS сервисы
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:3000") // Замените на порт вашего фронтенда, если он другой
                                        .AllowAnyHeader()
                                        .AllowAnyMethod());
            });

            builder.Services.AddDbContext<ExpenseTrackerDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigin"); // Применяем политику CORS

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            // Expense API Endpoints
            app.MapGet("/expenses", async (ExpenseTrackerDbContext db) =>
            {
                return await db.Expenses.ToListAsync();
            });

            app.MapGet("/expenses/{id}", async (int id, ExpenseTrackerDbContext db) =>
            {
                return await db.Expenses.FindAsync(id)
                            is Expense expense
                                ? Results.Ok(expense)
                                : Results.NotFound();
            });

            app.MapPost("/expenses", async (Expense expense, ExpenseTrackerDbContext db) =>
            {
                db.Expenses.Add(expense);
                await db.SaveChangesAsync();
                return Results.Created($"/expenses/{expense.Id}", expense);
            });

            app.MapPut("/expenses/{id}", async (int id, Expense expense, ExpenseTrackerDbContext db) =>
            {
                var existingExpense = await db.Expenses.FindAsync(id);

                if (existingExpense is null) return Results.NotFound();

                existingExpense.Description = expense.Description;
                existingExpense.Amount = expense.Amount;
                existingExpense.Date = expense.Date;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            app.MapDelete("/expenses/{id}", async (int id, ExpenseTrackerDbContext db) =>
            {
                if (await db.Expenses.FindAsync(id) is Expense expense)
                {
                    db.Expenses.Remove(expense);
                    await db.SaveChangesAsync();
                    return Results.Ok(expense);
                }
                return Results.NotFound();
            });

            app.Run();
        }
    }

    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}