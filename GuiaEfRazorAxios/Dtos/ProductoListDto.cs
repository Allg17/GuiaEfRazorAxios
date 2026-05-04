namespace GuiaEfRazorAxios.Dtos; // Namespace de DTOs.

/// <summary>
/// DTO de salida para listar productos por API.
/// El navegador no necesita recibir la entidad completa ni relaciones internas de EF Core.
/// </summary>
public class ProductoListDto // Modelo de respuesta JSON.
{
    public int Id { get; set; } // Identificador del producto.
    public string Nombre { get; set; } = string.Empty; // Nombre del producto.
    public decimal Precio { get; set; } // Precio que se mostrara en la tabla.
    public int Stock { get; set; } // Stock disponible.
    public string Categoria { get; set; } = string.Empty; // Nombre de la categoria ya transformado a texto.
}
