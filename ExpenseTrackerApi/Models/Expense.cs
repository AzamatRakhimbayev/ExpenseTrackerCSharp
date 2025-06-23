namespace ExpenseTrackerApi.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public required string Description { get; set; } // Добавлено required
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public required string Category { get; set; } // Добавлено required
    }
}