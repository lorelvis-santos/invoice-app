namespace App.Model;

public class Product : BaseModel
{
    public static readonly string FullPath = Path.Combine(AppContext.BaseDirectory, "Database", "Product.txt");
    public static readonly string Headers = "id,name,description,price,stock";

    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}