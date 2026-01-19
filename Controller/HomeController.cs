using App.View;

namespace App.Controller;

public class HomeController : BaseController
{
    public HomeController(HomeMenu homeMenu) : base(homeMenu) { }

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
                Console.WriteLine("Viendo facturas...");
                Console.ReadKey();
                break;
                
            case 1:
                ProductsController productsController = new(new ProductsMenu(), new Repository.ProductsRepository());
                bool productsLoop = true;

                while (productsLoop)
                {
                    productsLoop = productsController.Execute();
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