namespace App.Model;

public class InvoiceItem : BaseModel
{
    public static readonly string FullPath = Path.Combine(AppContext.BaseDirectory, "Database", "InvoiceItem.txt");
    public static readonly string Headers = "id,invoiceId,productId,productName,quantity,unitPrice,rowTotal";

    public string InvoiceId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal RowTotal => Quantity * UnitPrice;
}