namespace App.Model;

public abstract class BaseModel
{
    // Usaremos UUID para el manejo interno del id de los modelos
    public string Id { get; set; } = Guid.NewGuid().ToString();
}