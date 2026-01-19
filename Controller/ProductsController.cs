using App.View;
using App.Repository;
using App.Model;
using App.Extension;

namespace App.Controller;

public class ProductsController : BaseController
{
    private readonly ProductsMenu _view;
    private readonly ProductsRepository _repo;

    public ProductsController(ProductsMenu view, ProductsRepository repo) : base(view)
    {
        _view = view;
        _repo = repo;
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
        switch (choice)
        {
            case -1: 
                return false;
        }

        return true;
    }
}