namespace App.Views;

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