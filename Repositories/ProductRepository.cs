using App.Models;
using App.Data;

namespace App.Repositories;

public class ProductRepository
{
    private readonly string path = Product.GetPath();
    private readonly string headers = Product.GetHeaders();
    private readonly int limit = 3;
    
    public List<string> Get()
    {
        EnsureHeaders();
        return Database.Load(path, limit);
    }

    public bool Save(List<string> data)
    {
        EnsureHeaders();
        return Database.Save(path, data);
    }

    public bool EnsureHeaders()
    {
        return Database.EnsureHeaders(path, headers);
    }
}