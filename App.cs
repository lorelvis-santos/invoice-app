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
InvoiceItemsMenu invoiceItemsMenu = new();
InvoiceDraftItemsMenu invoiceDraftItemsMenu = new();
InvoiceDraftItemOptionsMenu invoiceDraftItemOptionsMenu = new();
InvoiceRepository invoiceRepo = new();
InvoiceItemRepository invoiceItemRepo = new();
InvoiceService invoiceService = new(invoiceRepo, invoiceItemRepo, productController, productService);
InvoiceController invoiceController = new(
    invoiceMenu, 
    createInvoiceMenu,
    invoiceItemsMenu,
    invoiceDraftItemsMenu,
    invoiceDraftItemOptionsMenu,
    productController,
    invoiceRepo, 
    invoiceService
);

HomeMenu homeMenu = new();
HomeController homeController = new(homeMenu, productController, invoiceController);

bool homeLoop = true;

while (homeLoop)
{
    homeLoop = homeController.Execute();
}