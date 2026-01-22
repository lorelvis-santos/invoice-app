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
    private readonly InvoiceItemsMenu _invoiceItemsView;
    private readonly InvoiceDraftItemsMenu _invoiceDraftItemsView;
    private readonly InvoiceDraftItemOptionsMenu _invoiceDraftItemOptionsView;
    private readonly ProductController _productController;
    private readonly InvoiceRepository _repo;
    private readonly InvoiceService _service;
    
    private List<Invoice> _invoices = [];

    public InvoiceController(
        InvoiceMenu view, 
        CreateInvoiceMenu createInvoiceView,
        InvoiceItemsMenu invoiceItemsView,
        InvoiceDraftItemsMenu invoiceDraftItemsView,
        InvoiceDraftItemOptionsMenu invoiceDraftItemOptionsView,
        ProductController productController,
        InvoiceRepository repo,
        InvoiceService service
    ) : base(view)
    {
        _view = view;
        _createInvoiceView = createInvoiceView;
        _invoiceItemsView = invoiceItemsView;
        _invoiceDraftItemsView = invoiceDraftItemsView;
        _invoiceDraftItemOptionsView = invoiceDraftItemOptionsView;
        _productController = productController;
        _repo = repo;
        _service = service;
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
        // Caso volver
        if (choice == -1)
        {
            return false;
        }

        // Caso especial: Insercion / Nueva Factura
        if (choice == -100)
        {
            CreateInvoice();
            return true;
        }

        // Visualizar detalles de factura
        return ShowInvoiceDetails(choice);
    }

    private bool ShowInvoiceDetails(int choice)
    {
        Invoice selectedInvoice = _invoices[choice];
        List<InvoiceItem>? invoiceItems = _service.GetInvoiceItems(selectedInvoice.Id);

        if (invoiceItems == null)
        {
            return false;
        }

        List<string> data = [.. invoiceItems.Select(d => $"Nombre: {d.ProductName} x{d.Quantity} | Unidad: {d.UnitPrice} | Total: ${d.RowTotal}" ?? "N/A")];

        int rowsPerPage = 10;
        _invoiceItemsView.Invoice = selectedInvoice;
        _invoiceItemsView.Pages = data.ToPages(rowsPerPage);
        _invoiceItemsView.RowsPerPage = rowsPerPage;
        _invoiceItemsView.Show();

        return true;
    }

    private bool CreateInvoice()
    {
        // Declaramos las variables que nos ayudaran a crear la factura
        List<InvoiceItem> draftItems = [];
        bool isCreatingLoop = true;

        decimal currentSubtotal = 0, currentTaxes = 0;
        List<string> data = [];
        int rowsPerPage = 10;

        while (isCreatingLoop)
        {
            currentSubtotal = draftItems.Sum(d => d.RowTotal);
            currentTaxes = TaxesService.CalculateTaxes(currentSubtotal);

            // Mostramos el detalle de la factura actual
            data = [.. draftItems.Select(d => $"Nombre: {d.ProductName} x{d.Quantity} | Unidad: {d.UnitPrice} | Total: ${d.RowTotal}" ?? "N/A")];

            _invoiceDraftItemsView.RowsPerPage = rowsPerPage;
            _invoiceDraftItemsView.Pages = data.ToPages(rowsPerPage);
            _invoiceDraftItemsView.CurrentSubtotal = currentSubtotal;
            _invoiceDraftItemsView.CurrentTaxes = currentTaxes;
            
            // Manejamos lo seleccionado por el usuario
            int choice = _invoiceDraftItemsView.Show();

            // Caso volver
            if (choice == -1)
            {
                isCreatingLoop = false;
                _service.DeleteDraft(draftItems);
                continue;
            }

            // Caso especial: Inserción
            if (choice == -100)
            {
                InsertDraftItem(draftItems);                
                continue;
            }

            // Caso especial: Emitir factura
            if (choice == -101) {
                isCreatingLoop = EmitInvoice(draftItems);
            }

            if (choice < 0 || choice >= draftItems.Count)
            {
                // Opcion invalida
                continue;
            }

            // El usuario seleccionó un item, le mostramos las opciones
            InvoiceItem selectedDraftItem = draftItems[choice];
            AlterDraftItem(selectedDraftItem, draftItems);
        }
    
        return false;
    }

    private bool InsertDraftItem(List<InvoiceItem> draftItems)
    {
        Product? product = _productController.Select();

        if (product == null)
        {
            return false;
        }

        if (product.Stock == 0)
        {
            Console.WriteLine("\n\tNo hay unidades disponibles de ese producto.");
            Console.ReadKey();
            return false;
        }

        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tPanel de administración");
        Console.WriteLine();
        Console.WriteLine($"\tAgregando un producto a la factura actual");

        string quantityInput;
        int quantity;

        do
        {
            quantityInput = PromptInput($"¿Cuántas unidades de {product.Name} desea agregar?");
        } while (!int.TryParse(quantityInput, out quantity) || quantity <= 0 || quantity > product.Stock);

        var (Success, Message) = _service.AddItemToDraft(product.Id, quantity, draftItems);

        Console.WriteLine($"\n\t{Message}");
        Console.ReadKey();

        return Success;
    }

    private bool AlterDraftItem(InvoiceItem draftItem, List<InvoiceItem> draftItems)
    {
        _invoiceDraftItemOptionsView.DraftItem = draftItem;
        int choice = _invoiceDraftItemOptionsView.Show();

        switch (choice)
        {
            case 0: // edit
                ModifyDraftItem(draftItem, draftItems);
                break;
            case 1: // delete
                RemoveDraftItem(draftItem, draftItems);
                break;
            default:
                return false;
        }

        return true;
    }

    private bool ModifyDraftItem(InvoiceItem draftItem, List<InvoiceItem> draftItems)
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tPanel de administración");
        Console.WriteLine();
        Console.WriteLine($"\tModificando el producto {draftItem.ProductName}");

        string quantityInput;
        int quantity;

        do
        {
            quantityInput = PromptInput($"Ingresa la nueva cantidad de unidades de {draftItem.ProductName}");
        } while (!int.TryParse(quantityInput, out quantity) || quantity <= 0);

        var (Success, Message) = _service.ModifyItemFromDraft(draftItem.ProductId, draftItem.Quantity, quantity, draftItems);

        Console.WriteLine($"\n\t{Message}");
        Console.ReadKey();

        return Success;
    }

    private bool RemoveDraftItem(InvoiceItem draftItem, List<InvoiceItem> draftItems)
    {
        var (Success, Message) = _service.RemoveItemFromDraft(draftItem, draftItems);
        Console.WriteLine($"\n\t{Message}");
        Console.ReadKey();
        return Success;
    }

    private bool EmitInvoice(List<InvoiceItem> draftItems)
    {
        if (draftItems.Count == 0) {
            Console.WriteLine("\n\tNo puedes emitir una factura vacía.");
            Console.ReadKey();
        } else {
            var (Success, Message) = _service.EmitInvoice(draftItems);

            Console.WriteLine($"\n\t{Message}");
            Console.ReadKey();
            
            if (Success)
            {
                return false; 
            }
        }

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