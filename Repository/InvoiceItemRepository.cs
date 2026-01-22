using System.Globalization;
using App.Data;
using App.Model;

namespace App.Repository;

public class InvoiceItemRepository : BaseRepository<InvoiceItem>
{
    public InvoiceItemRepository() : base(InvoiceItem.FullPath, InvoiceItem.Headers) { }

    protected override string MapToText(InvoiceItem item)
    {
        return $"{item.Id},{item.InvoiceId},{item.ProductId},{item.ProductName},{item.Quantity},{item.UnitPrice.ToString(CultureInfo.InvariantCulture)},{item.RowTotal.ToString(CultureInfo.InvariantCulture)}";
    }

    protected override InvoiceItem MapFromText(string line)
    {
        string[] parts = line.Split(',');

        return new InvoiceItem
        {
            Id = parts[0],
            InvoiceId = parts[1],
            ProductId = parts[2],
            ProductName = parts[3],
            Quantity = int.Parse(parts[4]),
            UnitPrice = decimal.Parse(parts[5], CultureInfo.InvariantCulture)
        };
    }

    public List<InvoiceItem> GetItemsByInvoiceId(string invoiceId)
    {
        EnsureHeaders();

        List<InvoiceItem> items = GetAll(reverse: true);
        
        return items.FindAll(d => d.InvoiceId == invoiceId);
    }
}