namespace App.Core;

public static class FileUtils
{
    // Lee las lineas del archivo. Tiene como opción ignorar el encabezado.
    // En caso de que no exista el archivo, retorna un array vacío.
    public static List<string> ReadFile(string path, bool ignoreHeader = true)
    {
        if (!File.Exists(path))
            return [];

        List<string> lines = [.. File.ReadAllLines(path)];

        if (ignoreHeader)
        {
            lines.RemoveAt(0);
            return lines;
        }
        else
        {
            return lines;
        }
    }

    public static bool WriteToFile(string path, List<string> lines)
    {
        try
        {
            // Obtener la ruta de la carpeta (sin el nombre del archivo)
            string? directory = Path.GetDirectoryName(path);

            // Si la carpeta no existe, la creamos
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllLines(path, lines);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static bool Exists(string path)
    {
        return File.Exists(path);
    }
}