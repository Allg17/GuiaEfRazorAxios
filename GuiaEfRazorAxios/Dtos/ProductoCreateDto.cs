using System.ComponentModel.DataAnnotations; // Importa atributos de validacion.

namespace GuiaEfRazorAxios.Dtos; // Namespace de DTOs.

/// <summary>
/// DTO de entrada para crear productos desde Axios.
/// Evita recibir directamente la entidad Producto desde la API.
/// </summary>
public class ProductoCreateDto // Representa el JSON que envia el navegador.
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")] // Requiere nombre.
    [StringLength(100, ErrorMessage = "El nombre no debe superar los 100 caracteres.")] // Limita longitud.
    public string Nombre { get; set; } = string.Empty; // Nombre enviado por JavaScript.

    [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor que cero.")] // Valida precio positivo.
    public decimal Precio { get; set; } // Precio enviado por JavaScript.

    [Range(0, 999999, ErrorMessage = "El stock no puede ser negativo.")] // Valida stock no negativo.
    public int Stock { get; set; } // Stock enviado por JavaScript.

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoria valida.")] // Evita CategoriaId cero.
    public int CategoriaId { get; set; } // Id de la categoria seleccionada en el select.
}
