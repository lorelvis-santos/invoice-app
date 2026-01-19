namespace App.View;

public static class HomeMenu
{
    public static int Show()
    {
        return Menu.Show(
            "Bienvenido a tu Gestor de Facturas",
            [
                "Ver facturas",
                "Ver productos"
            ]
        );
    }
}