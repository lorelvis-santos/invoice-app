using App.Model;
using App.View.Common;

namespace App.View;

public class InvoiceDraftItemOptionsMenu : IView
{
    public InvoiceItem? DraftItem { get; set; }
    public int Show()
    {
        return Menu.Show(
            "Gestor de Facturas",
            [
                "Editar",
                "Eliminar"
            ],
            $"Elige una opción para {DraftItem?.ProductName ?? "el producto seleccionado"}",
            false,
            [
                "Presiona [ESC] para volver"
            ]
        );
    }
}