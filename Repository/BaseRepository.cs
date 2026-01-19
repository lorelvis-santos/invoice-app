using App.Core;
using App.Data;

namespace App.Repository;

public abstract class BaseRepository<T>
{
    protected readonly string Path;
    protected readonly string Headers;

    protected BaseRepository(string path, string headers)
    {
        Path = path;
        Headers = headers;
    }

    protected bool EnsureHeaders()
    {
        return Database.EnsureHeaders(Path, Headers);
    }

    public bool Create(T item)
    {
        EnsureHeaders();

        string row = MapToText(item);

        return Database.Append(Path, row);
    }

    // Obtiene todos los objetos del archivo, ignorando los headers.
    public List<T> GetAll()
    {
        EnsureHeaders();

        List<string> lines = FileUtils.ReadFile(Path, true);
        List<T> items = [];

        foreach (string line in lines)
        {
            items.Add(MapFromText(line));
        }

        return items;
    }

    // Guarda una lista de objetos en un archivo, incluyendo headers.
    public bool SaveAll(List<T> items)
    {
        EnsureHeaders();

        List<string> lines = [Headers];

        foreach (T item in items)
        {
            lines.Add(MapToText(item));
        }

        return Database.Save(Path, lines);
    }

    protected abstract string MapToText(T item);
    protected abstract T MapFromText(string line);
}