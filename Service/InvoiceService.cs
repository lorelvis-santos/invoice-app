using App.Model;
using App.Repository;

namespace App.Service;

public class InvoiceService
{
    private readonly InvoiceRepository _invoiceRepo;
    private readonly InvoiceDetailRepository _detailRepo;
    public readonly decimal TaxRate = 0.18m; // 18% del ITBIS

    public InvoiceService(InvoiceRepository invoiceRepo, InvoiceDetailRepository detailRepo)
    {
        _invoiceRepo = invoiceRepo;
        _detailRepo = detailRepo;
    }

    public List<InvoiceDetail>? GetInvoiceDetails(string invoiceId)
    {
        if (!Guid.TryParse(invoiceId, out _))
        {
            return null;
        }

        return _detailRepo.GetDetailsByInvoiceId(invoiceId);
    }

    public (bool Success, string Message) EmitInvoice(List<InvoiceDetail> details)
    {
        if (details.Count == 0)
        {
            return (false, "No hay productos en la factura.");
        }

        decimal subtotal = details.Sum(product => product.RowTotal);
        decimal taxes = Math.Round(subtotal * TaxRate, 2);
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
        foreach (var detail in details)
        {
            detail.InvoiceId = invoice.Id;
            _detailRepo.Create(detail);
        }

        return (true, $"Factura emitida con éxito. Total: ${total}");
    }
}