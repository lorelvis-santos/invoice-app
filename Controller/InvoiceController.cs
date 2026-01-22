using App.View;
using App.Repository;
using App.Model;
using App.Extension;
using App.Service;

namespace App.Controller;

public class InvoiceController : BaseController
{
    private readonly InvoiceMenu _view;
    private readonly CreateInvoiceMenu _createInvoiceView;
    private readonly InvoiceDetailsMenu _invoiceDetailsView;
    private readonly InvoiceRepository _repo;
    private readonly InvoiceService _service;
    private readonly ProductController _productController;
    private readonly ProductService _productService;

    private List<Invoice> _invoices = [];

    public InvoiceController(
        InvoiceMenu view, 
        CreateInvoiceMenu createInvoiceView,
        InvoiceDetailsMenu invoiceDetailsView,
        InvoiceRepository repo,
        InvoiceService service,
        ProductController productController,
        ProductService productService
    ) : base(view)
    {
        _view = view;
        _createInvoiceView = createInvoiceView;
        _invoiceDetailsView = invoiceDetailsView;
        _repo = repo;
        _service = service;
        _productController = productController;
        _productService = productService;
    }

    public new bool Execute()
    {
        _invoices = _repo.GetAll(reverse: true);

        List<string> data = [.. _invoices.Select(i => $"Id: {i.ShortId} | Fecha: {i.Date} | Total: ${i.Total}" ?? "N/A")];

        int rowsPerPage = 10;
        _view.Pages = data.ToPages(rowsPerPage);
        _view.RowsPerPage = rowsPerPage;
        _view.Tips = ["Pulsa [I] para agregar una nueva factura"];

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
            CreateInvoice();
            return true;
        }

        // Visualizar detalles de factura
        Invoice selectedInvoice = _invoices[choice];
        List<InvoiceDetail>? invoiceDetails = _service.GetInvoiceDetails(selectedInvoice.Id);

        if (invoiceDetails == null)
        {
            return false;
        }

        List<string> data = [.. invoiceDetails.Select(d => $"Nombre: {d.ProductName} | Unidad: {d.UnitPrice} | Cantidad: {d.Quantity} | Total: ${d.RowTotal}" ?? "N/A")];

        int rowsPerPage = 10;
        _invoiceDetailsView.Invoice = selectedInvoice;
        _invoiceDetailsView.Pages = data.ToPages(rowsPerPage);
        _invoiceDetailsView.RowsPerPage = rowsPerPage;
        _invoiceDetailsView.Show();

        return true;
    }

    private bool CreateInvoice()
    {
        List<InvoiceDetail> draftDetails = [];
        bool isCreating = true;

        while (isCreating)
        {
            int choice = _createInvoiceView.Show();

            switch (choice)
            {
                case -1:
                    isCreating = false;

                    foreach (var detail in draftDetails)
                    {
                        _productService.IncrementStock(detail.ProductId, detail.Quantity);
                    }

                    break;
                case 0:
                    // Ver detalle..
                    ShowDraftDetail(draftDetails);
                    break;
                case 1:
                    // Gestionar productos..
                    AddOrModifyProductToDraft(draftDetails);
                    break;
                case 2:
                    // Eliminar productos..
                    RemoveProductFromDraft(draftDetails);
                    break;
                case 3:
                    // Emitir factura..
                    if (draftDetails.Count == 0) {
                        Console.WriteLine("\n\tNo puedes emitir una factura vacía.");
                    } else {
                        var result = _service.EmitInvoice(draftDetails);
                        Console.WriteLine($"\n\t{result.Message}");
                        if (result.Success)
                        {
                            isCreating = false; 
                        }
                    }
                    Console.ReadKey();
                    break;
            }
        }
    
        return false;
    }

    private void ShowDraftDetail(List<InvoiceDetail> draftDetails)
    {
        Console.Clear();
        Console.WriteLine("\n\t--- Vista Previa de la Factura ---");
        Console.WriteLine();

        decimal currentSubtotal = 0;

        foreach (var item in draftDetails)
        {
            Console.WriteLine($"\t{item.ProductName} x{item.Quantity} | Unidad: ${item.UnitPrice} | Total: ${item.RowTotal}");
            currentSubtotal += item.RowTotal;
        }

        Console.WriteLine("\n\t----------------------------------");
        Console.WriteLine($"\tSUBTOTAL: ${currentSubtotal}");
        Console.WriteLine($"\tIMPUESTOS ({_service.TaxRate * 100}%): ${Math.Round(currentSubtotal * _service.TaxRate, 2)}");
        Console.WriteLine($"\tTOTAL ESTIMADO: ${Math.Round(currentSubtotal * (1 + _service.TaxRate), 2)}");
        Console.Write("\n\tPresiona cualquier tecla para volver...");
        Console.ReadKey();
    }

    private bool AddOrModifyProductToDraft(List<InvoiceDetail> draftDetails)
    {
        Product? product = _productController.Select();
        bool isModifying = false;
        int productIndex = -1;

        if (product == null)
        {
            return false;
        }

        if ((productIndex = draftDetails.FindIndex(p => p.ProductId == product.Id)) != -1)
        {
            Console.WriteLine("\n\tEse producto ya se encuentra agregado al borrador.");
            Console.Write("\n\tA continuación ingresa la nueva cantidad.");
            isModifying = true;
        }

        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tPanel de administración");
        Console.WriteLine();
        Console.WriteLine($"\t{(isModifying ? "Modificando un producto de" : "Agregando un producto a")} la factura actual");

        string quantityInput;
        string promptInput = isModifying ?
            $"Ingresa la nueva cantidad de {product.Name}" :
            $"¿Cuántas unidades de {product.Name} desea agregar?";
        int quantity;

        do
        {
            quantityInput = PromptInput(promptInput);
        } while (!int.TryParse(quantityInput, out quantity) || quantity <= 0 || quantity > product.Stock);

        if (isModifying)
        {
            int oldQuantity = draftDetails[productIndex].Quantity;
            draftDetails[productIndex].Quantity = quantity;
            _productService.UpdateStock(product.Id, product.Stock + (oldQuantity - quantity));
        } else
        {
            draftDetails.Add(new InvoiceDetail
            {
                ProductId = product.Id,
                ProductName = product.Name ?? "N/A",
                Quantity = quantity,
                UnitPrice = product.Price
            });

            _productService.UpdateStock(product.Id, product.Stock - quantity);
        }
        
        Console.WriteLine(isModifying ? "\n\tSe actualizó la nueva cantidad del producto." : "\n\tProducto agregado al borrador.");
        Console.Write("\n\tPresiona una tecla para continuar...");
        Console.ReadKey();

        return true;
    }

    private bool RemoveProductFromDraft(List<InvoiceDetail> draftDetails)
    {
        List<Product> draftProducts = [.. draftDetails.Select(d => new Product{
            Id = d.ProductId,
            Name = d.ProductName
        })];

        if (draftProducts.Count == 0)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\tPanel de administración");
            Console.WriteLine();
            Console.WriteLine($"\tEliminar producto de la factura");
            Console.WriteLine("\n\tNo hay productos en la factura. Prueba agregando uno.");
            Console.Write("\n\tPresiona una tecla para volver...");
            Console.ReadKey();
        }

        Product? product = _productController.Select(draftProducts, false, false);
        InvoiceDetail? detail = draftDetails.Find(p => p.ProductId == product?.Id);

        if (product == null || detail == null)
        {
            return false;
        }

        _productService.IncrementStock(product.Id, detail.Quantity);
        draftDetails.Remove(detail);

        return true;
    }

    private static string PromptInput(string message)
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