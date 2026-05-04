using System.ComponentModel.DataAnnotations; // Importa validaciones para formularios y API.
using System.ComponentModel.DataAnnotations.Schema; // Importa Column para configurar tipos SQL.

namespace GuiaEfRazorAxios.Models; // Namespace de entidades del dominio.

/// <summary>
/// Entidad que representa la tabla Productos en SQL Server.
/// Es el lado "muchos" de la relacion: muchos productos pertenecen a una categoria.
/// </summary>
public class Producto // Clase publica para que EF Core, servicios, controladores y vistas puedan usarla.
{
    public int Id { get; set; } // Llave primaria autoincremental por convencion de EF Core.

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")] // Obliga a llenar el nombre.
    [StringLength(100, ErrorMessage = "El nombre no debe superar los 100 caracteres.")] // Evita nombres demasiado largos.
    public string Nombre { get; set; } = string.Empty; // Nombre del producto; se inicializa para evitar null.

    [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor que cero.")] // Valida rango aceptable.
    [Column(TypeName = "decimal(10,2)")] // Define precision decimal en SQL Server: 10 digitos, 2 decimales.
    public decimal Precio { get; set; } // Precio del producto.

    [Range(0, 999999, ErrorMessage = "El stock no puede ser negativo.")] // Evita cantidades negativas.
    public int Stock { get; set; } // Cantidad disponible.

    [Display(Name = "Categoria")] // Texto que Razor muestra en etiquetas de formularios.
    public int CategoriaId { get; set; } // Llave foranea que apunta a Categoria.Id.

    /// <summary>
    /// Propiedad de navegacion hacia la categoria.
    /// Permite escribir producto.Categoria.Nombre cuando la relacion fue cargada.
    /// </summary>
    public Categoria? Categoria { get; set; } // Puede ser null si no se cargo con Include.
}
