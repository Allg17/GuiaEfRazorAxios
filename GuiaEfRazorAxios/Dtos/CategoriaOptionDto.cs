namespace GuiaEfRazorAxios.Dtos; // Namespace para objetos que transfieren datos.

/// <summary>
/// DTO usado para mostrar categorias en combos/selects.
/// Solo contiene lo necesario: Id y Nombre.
/// </summary>
public class CategoriaOptionDto // Clase simple sin logica de negocio.
{
    public int Id { get; set; } // Identificador que se enviara como value del option HTML.
    public string Nombre { get; set; } = string.Empty; // Texto visible que se mostrara al usuario.
}
