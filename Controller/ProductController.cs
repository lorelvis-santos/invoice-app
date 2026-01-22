using App.View;
using App.Repository;
using App.Model;
using App.Extension;
using App.Service;

namespace App.Controller;

public class ProductController : BaseController
{
    private readonly ProductMenu _view;
    private readonly ProductRepository _repo;
    private readonly ProductService _service;

    public ProductController(ProductMenu view, ProductRepository repo, ProductService service) : base(view)
    {
        _view = view;
        _repo = repo;
        _service = service;
    }

    public new bool Execute()
    {
        List<Product> products = _repo.GetAll();

        List<string> data = [.. products.Select(p => $"Nombre: {p.Name} | Precio: {p.Price} | Stock: {p.Stock}" ?? "N/A")];

        int rowsPerPage = 10;
        _view.Pages = data.ToPages(rowsPerPage);
        _view.RowsPerPage = rowsPerPage;
        _view.Tips = ["Pulsa [I] para agregar un nuevo producto"];
        _view.Subtitle = "¡Estás administrando los productos!";

        return base.Execute();
    }

    public Product? Select(bool showPrice = true, bool showStock = true)
    {
        // Obtenemos los productos
        List<Product> products = _repo.GetAll();
        if (products.Count == 0)
        {
            return null;
        }

        // Mostramos los productos con paginacion
        // Mostramos los productos con paginacion
        List<string> data = [.. products.Select(p => {
            string row = $"Nombre: {p.Name}";

            if (showPrice) 
            {
                row += $" | Precio: {p.Price}";
            }

            if (showStock) {
                row += $" | Stock: {p.Stock}";
            }
            
            return row;
        })];

        int rowsPerPage = 10;
        _view.Pages = data.ToPages(rowsPerPage);
        _view.RowsPerPage = rowsPerPage;
        _view.Tips = ["Presiona [ESC] para cancelar"];
        _view.Subtitle = "Selecciona un producto";

        // Mostramos la vista
        int choice = _view.Show();

        // Retornamos el objeto si la elección es válida
        if (choice == -1)
        {
            return null; // El usuario canceló
        }
        
        return products[choice];
    }

    public Product? Select(List<Product> products, bool showPrice = true, bool showStock = true)
    {
        // Obtenemos los productos
        if (products.Count <= 0)
        {
            return null;
        }

        // Mostramos los productos con paginacion
        List<string> data = [.. products.Select(p => {
            string row = $"Nombre: {p.Name}";

            if (showPrice) 
            {
                row += $" | Precio: {p.Price}";
            }

            if (showStock) {
                row += $" | Stock: {p.Stock}";
            }
            
            return row;
        })];

        int rowsPerPage = 10;
        _view.Pages = data.ToPages(rowsPerPage);
        _view.RowsPerPage = rowsPerPage;
        _view.Tips = ["Presiona [ESC] para cancelar"];
        _view.Subtitle = "Selecciona un producto";

        // Mostramos la vista
        int choice = _view.Show();

        // Retornamos el objeto si la elección es válida
        if (choice == -1)
        {
            return null; // El usuario canceló
        }
        
        return products[choice];
    }

    protected override bool HandleChoice(int choice)
    {
        if (choice == -1)
        {
            return false;
        }

        // Caso especial: Insercion
        if (choice == -100)
        {
            CreateProduct();
            return true;
        }

        // A partir de aqui se implementarían funcionalidades como poder editar y eliminar...

        return true;
    }

    private bool CreateProduct()
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tPanel de administración");
        Console.WriteLine();
        Console.WriteLine("\tInsertar un nuevo producto"); 

        string name = PromptInput("Nombre");
        string description = PromptInput("Description");
        _ = decimal.TryParse(PromptInput("Precio"), out decimal price);
        _ = int.TryParse(PromptInput("Stock"), out int stock);

        var result = _service.CreateProduct(name, description, price, stock);

        Console.WriteLine($"\n\t{result.Message}");
        return result.Success;
    }

    public string PromptInput(string message)
    {
        string? input;

        do
        {
            Console.Write($"\n\t{message} >> ");
            input = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(input));

        return input.Trim();
    }
}