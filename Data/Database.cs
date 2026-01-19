using App.Core;
using App.Extensions;

namespace App.Data;

public static class Database
{
    public static List<string> Load(string path, int limitOfRows = -1)
    {
        if (limitOfRows > 0)
            return FileUtils.ReadFile(path, false).TakeFirstN(limitOfRows);

        return FileUtils.ReadFile(path, false);
    }

    public static List<string> Reverse(string path, string headers)
    {
        List<string> lines = FileUtils.ReadFile(path, true);
        lines.Reverse();
        lines.Insert(0, headers);
        return lines;
    }

    public static bool Save(string path, List<string> lines)
    {
        return FileUtils.WriteToFile(path, lines);
    }

    public static bool Clear(string path, string headers)
    {
        return FileUtils.WriteToFile(path, [headers]);
    }

    public static bool EnsureHeaders(string path, string separatedHeadersByComma)
    {
        return EnsureHeaders(path, [.. separatedHeadersByComma.Split(',')]);
    }

    public static bool EnsureHeaders(string path, List<string> expectedHeaders)
    {
        List<string> lines = [];

        if (FileUtils.Exists(path)) {
            lines = FileUtils.ReadFile(path, false);
        }

        if (lines.Count == 0 || !string.Equals(lines[0], string.Join(",", expectedHeaders), StringComparison.OrdinalIgnoreCase))
        {
            List<string> data = Load(path);
            data.Insert(0, string.Join(",", expectedHeaders));
            FileUtils.WriteToFile(path, data);
            return false;
        }

        return true;
    }

    public static int FindRowIndex(List<string> lines, string header, string value)
    {
        if (lines.Count < 2)
            return -1;

        // Obtenemos los headers del archivo (.txt o .csv) recibido
        List<string> headers = [.. lines[0].Split(',')];

        // Obtenemos el índice de la columna que nos interesa
        int columnIndex = headers.IndexOf(header);

        if (columnIndex == -1)
            return -1;

        // Revisamos cada una de las líneas
        for (int i = 1; i < lines.Count; i++)
        {
            string[] fields = lines[i].Split(',');
            if (fields.Length > columnIndex && fields[columnIndex] == value)
                return i;
        }

        return -1;
    }

    public static List<string> Where(List<string> lines, string header, string value)
    {
        if (lines.Count < 2)
            return [];

        // Obtenemos los headers del archivo (.txt o .csv) recibido
        List<string> headers = [.. lines[0].Split(',')];

        // Obtenemos el índice de la columna que nos interesa
        int columnIndex = headers.IndexOf(header);

        if (columnIndex == -1)
            return [];

        List<string> rows = [];

        // Revisamos cada una de las líneas
        for (int i = 1; i < lines.Count; i++)
        {
            string[] fields = lines[i].Split(',');
            if (fields.Length > columnIndex && fields[columnIndex] == value)
                rows.Add(lines[i]);
        }

        return rows;
    }

    public static List<string> AddRow(List<string> lines, string row)
    {
        lines.Add(row);
        return lines;
    }

    public static List<string> RemoveRow(List<string> lines, int index)
    {
        if (index >= 0 && index < lines.Count)
        {
            lines.RemoveAt(index);
        }
        return lines;
    }

    public static List<string> UpdateRow(List<string> lines, int index, string newRow)
    {
        // Recordar que el índice 0 es para los headers
        if (index <= 0 || index >= lines.Count)
            return lines;

        lines[index] = newRow;

        return lines;
    }

    public static List<string> Pagination(List<string> lines, int page = 1, int rowsPerPage = 15, bool removeFirstLine = true)
    {
        if (removeFirstLine && lines.Count > 0)
        {
            lines.RemoveAt(0); // Quitamos los headers para paginar solo datos
        }

        return lines.GetPagination(page, rowsPerPage);
    }
}