using App.Model;
using App.Repository;

namespace App.Service;

public class ProductService
{
    private readonly ProductRepository _repo;

    public ProductService(ProductRepository repo)
    {
        _repo = repo;
    }

    public (bool Success, string Message) CreateProduct(
        string name,
        string description,
        decimal price,
        int stock
    )
    {
        // Validaciones...
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
        {
            return (false, "El nombre debe tener al menos 3 carácteres.");
        }

        if (price < 0)
        {
            return (false, "El precio no puede ser negativo.");
        }

        if (_repo.Exists(name))
        {
            return (false, "El producto ya existe en el sistema.");
        }

        // Si todo esta bien, se crea el producto
        Product newProduct = new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock
        };

        bool ok = _repo.Create(newProduct);

        return ok ?
            (true, "Producto guardado correctamente") :
            (false, "Error al guardar el producto");
    }
}