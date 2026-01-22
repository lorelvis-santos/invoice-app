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

    public Product? GetById(string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return null;
        }

        return _repo.GetById(id);
    }

    public (bool Success, string Message) UpdateStock(string id, int newStock)
    {
        if (!Guid.TryParse(id, out _))
        {
            return (false, "Id inválida.");
        }

        if (newStock < 0)
        {
            return (false, "Stock inválido.");
        }

        if (!_repo.UpdateStock(id, newStock))
        {
            return (false, "No se pudo actualizar el nuevo stock.");
        }

        return (true, "Stock actualizado correctamente.");
    }

    public (bool Success, string Message) IncrementStock(string id, int toIncrement)
    {
        if (!Guid.TryParse(id, out _))
        {
            return (false, "Id inválida.");
        }

        if (toIncrement < 0)
        {
            return (false, "Stock inválido.");
        }

        if (!_repo.IncrementStock(id, toIncrement))
        {
            return (false, "No se pudo actualizar el nuevo stock.");
        }

        return (true, "Stock actualizado correctamente.");
    }

    public (bool Success, string Message) DecrementStock(string id, int toDecrement)
    {
        if (!Guid.TryParse(id, out _))
        {
            return (false, "Id inválida.");
        }

        if (toDecrement < 0)
        {
            return (false, "Stock inválido.");
        }

        if (!_repo.DecrementStock(id, toDecrement))
        {
            return (false, "No se pudo actualizar el nuevo stock.");
        }

        return (true, "Stock actualizado correctamente.");
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
            Price = Math.Round(price, 2),
            Stock = stock
        };

        bool ok = _repo.Create(newProduct);

        return ok ?
            (true, "Producto guardado correctamente") :
            (false, "Error al guardar el producto");
    }
}