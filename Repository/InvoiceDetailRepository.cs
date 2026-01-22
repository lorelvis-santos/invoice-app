using System.Globalization;
using App.Data;
using App.Model;

namespace App.Repository;

public class InvoiceDetailRepository : BaseRepository<InvoiceDetail>
{
    public InvoiceDetailRepository() : base(InvoiceDetail.FullPath, InvoiceDetail.Headers) { }

    protected override string MapToText(InvoiceDetail item)
    {
        return $"{item.Id},{item.InvoiceId},{item.ProductId},{item.ProductName},{item.Quantity},{item.UnitPrice.ToString(CultureInfo.InvariantCulture)},{item.RowTotal.ToString(CultureInfo.InvariantCulture)}";
    }

    protected override InvoiceDetail MapFromText(string line)
    {
        string[] parts = line.Split(',');

        return new InvoiceDetail
        {
            Id = parts[0],
            InvoiceId = parts[1],
            ProductId = parts[2],
            ProductName = parts[3],
            Quantity = int.Parse(parts[4]),
            UnitPrice = decimal.Parse(parts[5], CultureInfo.InvariantCulture)
        };
    }

    public List<InvoiceDetail> GetDetailsByInvoiceId(string invoiceId)
    {
        EnsureHeaders();

        List<InvoiceDetail> details = GetAll(reverse: true);
        
        return details.FindAll(d => d.InvoiceId == invoiceId);
    }
}