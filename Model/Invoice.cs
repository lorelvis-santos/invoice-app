namespace App.Model;

public class Invoice : BaseModel
{
    public static readonly string FullPath = Path.Combine(AppContext.BaseDirectory, "../../..", "Database", "Invoice.txt");
    public static readonly string Headers = "id,date,subtotal,taxes,total";

    public DateTime Date { get; set; } = DateTime.Now;
    public decimal Subtotal { get; set; }
    public decimal Taxes { get; set; }
    public decimal Total { get; set; }
}