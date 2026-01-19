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
}