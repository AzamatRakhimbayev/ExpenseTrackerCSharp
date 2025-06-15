using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApi.Models; // Добавляем using для доступа к модели Expense

namespace ExpenseTrackerApi.Data
{
    public class ExpenseTrackerDbContext : DbContext
    {
        public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options)
            : base(options)
        {
        }

        // DbSet представляет таблицу в базе данных.
        // Каждый объект Expense будет строкой в таблице "Expenses".
        public DbSet<Expense> Expenses { get; set; }

        // Этот метод можно использовать для дополнительной настройки модели
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Например, можно указать, что строка Description не может быть null
            modelBuilder.Entity<Expense>().Property(e => e.Description).IsRequired();
            modelBuilder.Entity<Expense>().Property(e => e.Amount).HasColumnType("decimal(18,2)"); // Точность для денег
        }
    }
}