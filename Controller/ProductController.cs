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

        return base.Execute();
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
            Insert();
            return true;
        }

        // A partir de aqui se implementarían funcionalidades como poder editar y eliminar...

        return true;
    }

    private bool Insert()
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