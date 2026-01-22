using App.View.Common;

namespace App.View;

public class CreateInvoiceMenu : IView
{
    public int Show()
    {
        return Menu.Show(
            "Gestor de Facturas",
            [
                "Ver detalle",
                "Agregar/modificar productos",
                "Modificar un producto",
                "Eliminar un producto",
                "Emitir factura"
            ],
            "Nueva factura [Borrador]"
        );
    }
}