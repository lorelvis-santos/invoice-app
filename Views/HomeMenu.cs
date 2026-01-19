using App.Views;

namespace App.Views;

public static class HomeMenu
{
    public static int Show()
    {
        return Menu.Show(
            "Bienvenido a tu gestor de facturas",
            [
                "Opción 1",
                "Opción 2"
            ]
        );
    }
}