using App.Controller;
using App.Model;
using App.Repository;

ProductRepository repository = new();

List<Product> products = new List<Product>
{
    new Product { Name = "Laptop", Description = "HP Victus", Price = 800.00m, Stock = 10 },
    new Product { Name = "Mouse", Description = "Logitech", Price = 25.50m, Stock = 50 }
};

Console.WriteLine(repository.SaveAll(products));

List<Product> savedProducts = repository.GetAll();

foreach (Product product in savedProducts)
{
    Console.WriteLine(product.Name);
}

Console.WriteLine(Product.FullPath);

Console.ReadKey();

bool homeLoop = true;

while (homeLoop)
{
    homeLoop = HomeController.Execute();
}