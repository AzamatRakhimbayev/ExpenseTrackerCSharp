using Microsoft.EntityFrameworkCore; // Эту строку нужно добавить
using ExpenseTrackerApi.Data;      // Эту строку нужно добавить
using ExpenseTrackerApi.Models; // Для доступа к модели Expense

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// >>>>>>>>>>>>>> ДОБАВЬТЕ ЭТИ СТРОКИ ЗДЕСЬ <<<<<<<<<<<<<<<
builder.Services.AddDbContext<ExpenseTrackerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// >>>>>>>>>>>>>> ДОБАВЬТЕ ЭТИ СТРОКИ ЗДЕСЬ <<<<<<<<<<<<<<<


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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
// Конечные точки API для управления расходами

// Получить все расходы
app.MapGet("/expenses", async (ExpenseTrackerDbContext db) =>
{
    return await db.Expenses.ToListAsync();
})
.WithName("GetAllExpenses")
.WithOpenApi();

// Получить расход по ID
app.MapGet("/expenses/{id}", async (int id, ExpenseTrackerDbContext db) =>
{
    return await db.Expenses.FindAsync(id)
        is Expense expense
        ? Results.Ok(expense)
        : Results.NotFound();
})
.WithName("GetExpenseById")
.WithOpenApi();

// Добавить новый расход
app.MapPost("/expenses", async (Expense expense, ExpenseTrackerDbContext db) =>
{
    db.Expenses.Add(expense);
    await db.SaveChangesAsync();
    return Results.Created($"/expenses/{expense.Id}", expense);
})
.WithName("CreateExpense")
.WithOpenApi();

// Обновить существующий расход
app.MapPut("/expenses/{id}", async (int id, Expense updatedExpense, ExpenseTrackerDbContext db) =>
{
    var expense = await db.Expenses.FindAsync(id);

    if (expense is null) return Results.NotFound();

    expense.Description = updatedExpense.Description;
    expense.Amount = updatedExpense.Amount;
    expense.Date = updatedExpense.Date;
    expense.Category = updatedExpense.Category;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateExpense")
.WithOpenApi();

// Удалить расход
app.MapDelete("/expenses/{id}", async (int id, ExpenseTrackerDbContext db) =>
{
    if (await db.Expenses.FindAsync(id) is Expense expense)
    {
        db.Expenses.Remove(expense);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
})
.WithName("DeleteExpense")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}