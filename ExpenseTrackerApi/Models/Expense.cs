namespace ExpenseTrackerApi.Models
{
    public class Expense
    {
        public int Id { get; set; } // Первичный ключ, будет автоинкрементироваться
        public string Description { get; set; } // Описание расхода
        public decimal Amount { get; set; } // Сумма расхода
        public DateTime Date { get; set; } // Дата расхода
        public string Category { get; set; } // Категория расхода (например, "Еда", "Транспорт")
    }
}