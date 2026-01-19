using System.Globalization;
using App.Model;

namespace App.Repository;

public class InvoiceRepository : BaseRepository<Invoice>
{
    public InvoiceRepository() : base(Invoice.FullPath, Invoice.Headers) { }

    protected override string MapToText(Invoice item)
    {
        return $"{item.Id},{item.Date},{item.Subtotal.ToString(CultureInfo.InvariantCulture)},{item.Taxes.ToString(CultureInfo.InvariantCulture)},{item.Total.ToString(CultureInfo.InvariantCulture)}";
    }

    protected override Invoice MapFromText(string line)
    {
        string[] parts = line.Split(',');

        return new Invoice
        {
            Id = parts[0],
            Date = DateTime.Parse(parts[1]),
            Subtotal = decimal.Parse(parts[2], CultureInfo.InvariantCulture),
            Taxes = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
            Total = decimal.Parse(parts[4], CultureInfo.InvariantCulture)
        };
    }
}