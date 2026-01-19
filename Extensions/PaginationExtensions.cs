namespace App.Extensions;

// Migrado desde PaginationUtils, aprovechando el concepto de extender clases nativas
public static class PaginationExtensions
{
    // Extensión para obtener la paginación de una lista
    public static List<T> GetPagination<T>(this List<T> items, int page = 1, int rowsPerPage = 15)
    {
        // Validaciones básicas para evitar errores de índice
        if (items == null || items.Count == 0 || page < 1)
            return new List<T>();

        int totalPages = (int)Math.Ceiling((double)items.Count / rowsPerPage);
        
        if (page > totalPages)
            return new List<T>();

        int offset = rowsPerPage * (page - 1);

        return items.TakeFirstN(offset, offset + rowsPerPage);
    }

    // Extensión para calcular el total de páginas desde la lista
    public static int GetTotalPages<T>(this List<T> items, int rowsPerPage)
    {
        if (items == null || items.Count == 0) return 0;
        
        int totalPages = items.Count / rowsPerPage;
        if (items.Count % rowsPerPage != 0) totalPages++;
        
        return totalPages;
    }

    // Extensión para enteros: permite calcular la siguiente página desde el total
    // Uso: totalPages.NextPage(currentPage)
    public static int NextPage(this int totalPages, int currentPage)
    {
        if (totalPages <= 0) return 1;
        return currentPage >= totalPages ? 1 : ++currentPage;
    }

    // Extensión para enteros: permite calcular la página anterior desde el total
    // Uso: totalPages.PreviousPage(currentPage)
    public static int PreviousPage(this int totalPages, int currentPage)
    {
        if (totalPages <= 0) return 1;
        return currentPage - 1 <= 0 ? totalPages : --currentPage;
    }
}