namespace App.Utils;

public static class PaginationUtils
{
    public static List<T> GetPagination<T>(List<T> items, int page = 1, int rowsPerPage = 15)
    {
        if (items == null || items.Count == 0 || page < 1 || page > GetTotalPages(items.Count, rowsPerPage))
            return [];

        int offset = rowsPerPage * (page - 1);

        return ListUtils.TakeFirstN(items, offset, offset + rowsPerPage);
    }

    public static int GetTotalPages(int arrayLength, int rowsPerPage)
    {
        int totalPages = arrayLength / rowsPerPage;

        if (arrayLength % rowsPerPage != 0)
            totalPages++;

        return totalPages;
    }

    public static int PreviousPage(int arrayLength, int page = 1, int rowsPerPage = 15)
    {
        return page - 1 == 0 ? GetTotalPages(arrayLength, rowsPerPage) : --page;
    }

    public static int PreviousPage(int pages, int page)
    {
        return page - 1 == 0 ? pages : --page;
    }

    public static int NextPage(int arrayLength, int page = 1, int rowsPerPage = 15)
    {
        return page >= GetTotalPages(arrayLength, rowsPerPage) ? 1 : ++page;
    }

    public static int NextPage(int pages, int page = 1)
    {
        return page >= pages ? 1 : ++page;
    }
}