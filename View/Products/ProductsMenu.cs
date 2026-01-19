using App.View.Common;

namespace App.View;

public class ProductsMenu : IView
{
    // Propiedades para guardar la configuración antes de llamar a Show()
    public string[][] Pages { get; set; } = [];
    public int RowsPerPage { get; set; } = 10;
    public string[]? Tips { get; set; } = null;
    public string Subtitle { get; set; } = "¡Estás administrando los productos!";

    public int Show()
    {
        return Menu.Show(
            "Gestor de Facturas",
            Pages,
            1,
            RowsPerPage,
            false,
            Subtitle,
            Tips
        );
    }
}