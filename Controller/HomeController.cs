using App.View;
using App.Repository;
using App.Service;

namespace App.Controller;

public class HomeController : BaseController
{
    private readonly ProductController _productController;
    private readonly InvoiceController _invoiceController;

    public HomeController(HomeMenu homeMenu, ProductController productController, InvoiceController invoiceController) : base(homeMenu)
    {
        _productController = productController;
        _invoiceController = invoiceController;
    }

    protected override bool HandleChoice(int choice)
    {
        switch (choice)
        {
            case -1:
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("\tGestor de Facturas");

                Console.WriteLine();
                Console.WriteLine("\t¿Estás seguro que deseas salir de la aplicación? (y/n)");

                Console.WriteLine();
                Console.Write("\tRespuesta >> ");

                string answer = Console.ReadLine()?.ToLower() ?? "n";

                if (answer == "y" || answer == "yes")
                    return false;

                break;

            case 0:
                bool invoicesLoop = true;

                while (invoicesLoop)
                {
                    invoicesLoop = _invoiceController.Execute();
                }

                break;
                
            case 1:
                bool productsLoop = true;

                while (productsLoop)
                {
                    productsLoop = _productController.Execute();
                }

                break;

            default:
                Console.WriteLine();
                Console.WriteLine("\tError: has ingresado un número inválido.");
                Console.WriteLine();
                Console.Write("\tPresiona una tecla para reintentarlo...");
                Console.ReadKey();

                break;
        }

        return true;
    }
}