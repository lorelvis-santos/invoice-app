using App.Controller;
using App.Repository;
using App.Service;
using App.View;

ProductMenu productMenu = new();
ProductRepository productRepo = new();
ProductService productService = new(productRepo);
ProductController productController = new(
    productMenu, 
    productRepo,
    productService
);

InvoiceMenu invoiceMenu = new();
CreateInvoiceMenu createInvoiceMenu = new();
InvoiceDetailsMenu invoiceDetailsMenu = new();
InvoiceRepository invoiceRepo = new();
InvoiceDetailRepository invoiceDetailRepo = new();
InvoiceService invoiceService = new(invoiceRepo, invoiceDetailRepo);
InvoiceController invoiceController = new(invoiceMenu, createInvoiceMenu, invoiceDetailsMenu, invoiceRepo, invoiceService, productController, productService);

HomeMenu homeMenu = new();
HomeController homeController = new(homeMenu, productController, invoiceController);

bool homeLoop = true;

while (homeLoop)
{
    homeLoop = homeController.Execute();
}