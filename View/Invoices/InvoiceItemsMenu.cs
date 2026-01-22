using App.Model;
using App.View.Common;

namespace App.View;

public class InvoiceItemsMenu : IView
{
    // Propiedades para guardar la configuración antes de llamar a Show()
    public Invoice Invoice { get; set; } = new();
    public string[][] Pages { get; set; } = [];
    public int RowsPerPage { get; set; } = 10;
    public string[]? Tips 
    {
        get
        {
            return [
                $"SUBTOTAL: ${Math.Round(Invoice.Subtotal, 2)}",
                $"IMPUESTOS: ${Math.Round(Invoice.Taxes, 2)}",
                $"TOTAL ESTIMADO: ${Math.Round(Invoice.Total, 2 )}"
            ];
        } 
    }
    public string Subtitle { 
        get {
            return $"¡Estás viendo los detalles de la factura #{Invoice.ShortId}!";
        }
    }

    public int Show()
    {
        return Menu.Show(
            "Gestor de Facturas",
            Pages,
            1,
            RowsPerPage,
            true,
            Subtitle,
            Tips
        );
    }
}