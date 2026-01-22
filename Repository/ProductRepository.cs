using App.Data;
using App.Model;
using System.Globalization;

namespace App.Repository;

public class ProductRepository: BaseRepository<Product>
{    
    public ProductRepository() : base(Product.FullPath, Product.Headers) { }

    protected override string MapToText(Product item)
    {
        return $"{item.Id},{item.Name},{item.Description},{item.Price.ToString(CultureInfo.InvariantCulture)},{item.Stock}";
    }

    protected override Product MapFromText(string line)
    {
        string[] parts = line.Split(',');

        return new Product
        {
            Id = parts[0],
            Name = parts[1],
            Description = parts[2],
            Price = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
            Stock = int.Parse(parts[4])
        };
    }

    public bool UpdateStock(string id, int newStock)
    {
        Product? product = GetById(id);

        if (product == null)
        {
            return false;
        }

        if (newStock < 0)
        {
            return false;
        }

        int rowIndex = Database.FindRowIndex(Path, "id", id);
        product.Stock = newStock;

        Database.UpdateRow(Path, rowIndex, MapToText(product));

        return true;
    }

    public bool IncrementStock(string id, int toIncrement)
    {
        Product? product = GetById(id);

        if (product == null)
        {
            return false;
        }

        return UpdateStock(id, product.Stock + toIncrement);
    }

    public bool DecrementStock(string id, int toDecrement)
    {
        Product? product = GetById(id);

        if (product == null)
        {
            return false;
        }

        return UpdateStock(id, product.Stock - toDecrement);
    }

    public bool Exists(string name)
    {
        var products = GetAll();

        foreach (var product in products)
        {
            if (string.Equals(product.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}