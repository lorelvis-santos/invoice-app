using App.Model;
using App.Repository;
using App.Controller;

namespace App.Service;

public class InvoiceService
{
    private readonly InvoiceRepository _invoiceRepo;
    private readonly InvoiceItemRepository _invoiceItemRepo;
    private readonly ProductController _productController;
    private readonly ProductService _productService;


    public InvoiceService(
        InvoiceRepository invoiceRepo, 
        InvoiceItemRepository invoiceItemRepo,
        ProductController productController,
        ProductService productService
    )
    {
        _invoiceRepo = invoiceRepo;
        _invoiceItemRepo = invoiceItemRepo;
        _productController = productController;
        _productService = productService;
    }

    public List<InvoiceItem>? GetInvoiceItems(string invoiceId)
    {
        if (!Guid.TryParse(invoiceId, out _))
        {
            return null;
        }

        return _invoiceItemRepo.GetItemsByInvoiceId(invoiceId);
    }

    public (bool Success, string Message) EmitInvoice(List<InvoiceItem> items)
    {
        if (items.Count == 0)
        {
            return (false, "No hay productos en la factura.");
        }

        decimal subtotal = Math.Round(items.Sum(product => product.RowTotal), 2);
        decimal taxes = Math.Round(TaxesService.CalculateTaxes(subtotal), 2);
        decimal total = subtotal + taxes;

        Invoice invoice = new() {
            Date = DateTime.Now,
            Subtotal = subtotal,
            Taxes = taxes,
            Total = total
        };

        if (!_invoiceRepo.Create(invoice))
        {
            return (false, "Error al guardar factura.");
        }

        // 3. Guardar detalles y confirmar stock
        foreach (var item in items)
        {
            item.InvoiceId = invoice.Id;
            _invoiceItemRepo.Create(item);
        }

        return (true, $"Factura emitida con éxito. Total: ${total}");
    }

    public (bool Success, string Message) AddItemToDraft(string productId, int quantity, List<InvoiceItem> draftItems)
    {
        Product? product = _productService.GetById(productId);

        if (product == null)
        {
            return (false, "El producto no existe.");
        }

        if (draftItems.FindIndex(p => p.ProductId == product.Id) != -1)
        {
            return (false, "Ese producto ya se encuentra agregado al borrador.");
        }

        if (quantity <= 0 || quantity > product.Stock)
        {
            return (false, "Cantidad inválida.");
        }

        draftItems.Add(new InvoiceItem
        {
            ProductId = product.Id,
            ProductName = product.Name ?? "N/A",
            Quantity = quantity,
            UnitPrice = product.Price
        });

        var result = _productService.UpdateStock(product.Id, product.Stock - quantity);

        return result.Success
            ? (true, "Producto agregado correctamente.")
            : (false, result.Message);
    }

    public (bool Success, string Message) ModifyItemFromDraft(
        string productId, 
        int oldQuantity, 
        int quantity,
        List<InvoiceItem> draftItems)
    {
        Product? product = _productService.GetById(productId);

        if (product == null)
        {
            return (false, "El producto no existe.");
        }

        int productIndex = draftItems.FindIndex(d => d.ProductId == product.Id);
        if (productIndex == -1)
        {
            return (false, "El producto no se encuentra en la lista.");
        }

        if (quantity <= 0 || quantity > (product.Stock + oldQuantity))
        {
            return (false, "Cantidad inválida.");
        }

        var (Success, Message) = _productService.UpdateStock(product.Id, product.Stock + oldQuantity - quantity);

        if (Success)
        {
            draftItems[productIndex].Quantity = quantity;
            return (true, "Producto actualizado correctamente.");
        } else
        {
            return (false, Message);
        }
    }

    public (bool Success, string Message) RemoveItemFromDraft(InvoiceItem draftItem, List<InvoiceItem> draftItems)
    {
        _productService.IncrementStock(draftItem.ProductId, draftItem.Quantity);
        draftItems.Remove(draftItem);

        return (true, "Producto eliminado correctamente.");
    }

    public (bool Success, string Message) DeleteDraft(List<InvoiceItem> draftItems)
    {
        foreach (var item in draftItems)
        {
            _productService.IncrementStock(item.ProductId, item.Quantity);
        }

        return (true, "Borrador de factura eliminado correctamente.");
    }
}