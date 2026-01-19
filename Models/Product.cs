using App.Data;

namespace App.Models;

public class Product
{
    private static readonly string path = Path.GetFullPath(@"../../../Database/Product.txt", AppContext.BaseDirectory);
    private static readonly string headers = "name,description,price,stock";

    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    public static string GetPath()
    {
        return path;
    }

    public static string GetHeaders()
    {
        return headers;
    }
}