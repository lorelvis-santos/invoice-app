using App.Service;
using App.View.Common;

namespace App.View;

public class InvoiceDraftItemsMenu : IView
{
    // Propiedades para guardar la configuración antes de llamar a Show()
    public decimal CurrentSubtotal { get; set; } = 0;
    public decimal CurrentTaxes { get; set; } = 0;
    public decimal CurrentTotal { get => CurrentSubtotal + CurrentTaxes; }
    public string[][] Pages { get; set; } = [];
    public int RowsPerPage { get; set; } = 10;
    public string[]? Tips 
    {
        get
        {
            return [
                $"SUBTOTAL: ${Math.Round(CurrentSubtotal, 2)}",
                $"IMPUESTOS: ${Math.Round(CurrentTaxes, 2)}",
                $"TOTAL ESTIMADO: ${Math.Round(CurrentTotal, 2 )}",
                "",
                "Presiona [I] para agregar un producto",
                "Presiona [ENTER] para ver las opciones de un producto",
                "Presiona [E] para emitir la factura"
            ];
        } 
    }
    public string Subtitle { 
        get {
            return $"¡Estás viendo los detalles del borrador de la factura!";
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