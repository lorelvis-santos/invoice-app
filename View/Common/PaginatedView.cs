namespace App.View.Common;

public class PaginatedView : IView {
    // Propiedades para los datos
    public string[][] Pages { get; set; } = [];
    public int RowsPerPage { get; set; }
    public string Title { get; set; } = "";

    public int Show() {
        // Aquí usas las propiedades en lugar de parámetros
        return Menu.Show(Title, Pages, 1, RowsPerPage); 
    }
}