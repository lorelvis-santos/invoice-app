namespace App.Utils;

public static class ListUtils
{
    // Para obtener una cantidad N de elementos de un array, algo similar a un límite o top...
    public static List<T> TakeFirstN<T>(List<T> data, int limit)
    {
        // Validaciones iniciales
        if (data == null || data.Count == 0 || limit < 1)
            return [];

        List<T> items = [];

        if (limit == -1 || limit > data.Count)
        {
            limit = data.Count;
        }

        for (int i = 0; i < limit; i++)
        {
            items.Add(data[i]);
        }

        return items;
    }

    public static List<T> TakeFirstN<T>(List<T> data, int start, int limit = -1)
    {
        // Validaciones iniciales
        if (data == null || data.Count == 0 || start >= data.Count || limit < 1)
            return [];

        List<T> items = [];

        if (limit == -1 || limit > data.Count)
        {
            limit = data.Count;
        }
            
        for (int i = start; i < limit; i++)
        {
            items.Add(data[i]);
        }

        return items;
    }
}