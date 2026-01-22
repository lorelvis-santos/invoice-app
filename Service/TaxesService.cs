using App.Model;
using App.Repository;

namespace App.Service;

public class TaxesService
{
    public static readonly decimal TaxRate = 0.18m; // 18% del ITBIS

    public static decimal CalculateTaxes(decimal amount)
    {
        return amount * TaxRate;
    }
}