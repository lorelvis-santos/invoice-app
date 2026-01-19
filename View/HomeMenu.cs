using App.View.Common;

namespace App.View;

public class HomeMenu : IView
{
    public int Show()
    {
        return Menu.Show(
            "Gestor de Facturas",
            [
                "Ver facturas",
                "Ver productos"
            ]
        );
    }
}