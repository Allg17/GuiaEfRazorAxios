using System.ComponentModel.DataAnnotations; // Importa atributos de validacion como Required y StringLength.

namespace GuiaEfRazorAxios.Models; // Agrupa las entidades del dominio en el namespace Models.

/// <summary>
/// Entidad que representa la tabla Categorias en SQL Server.
/// Es el lado "uno" de la relacion uno-a-muchos con Producto.
/// </summary>
public class Categoria // Clase publica porque EF Core, controladores y vistas necesitan usarla.
{
    public int Id { get; set; } // Llave primaria; EF Core la reconoce por convencion al llamarse Id.

    [Required(ErrorMessage = "El nombre de la categoria es obligatorio.")] // Valida que el usuario envie un nombre.
    [StringLength(80, ErrorMessage = "El nombre no debe superar los 80 caracteres.")] // Limita longitud para UI y base.
    public string Nombre { get; set; } = string.Empty; // Nombre visible de la categoria; string.Empty evita null.

    [StringLength(250, ErrorMessage = "La descripcion no debe superar los 250 caracteres.")] // Limita texto opcional.
    public string? Descripcion { get; set; } // Descripcion opcional; el signo ? permite null.

    /// <summary>
    /// Propiedad de navegacion hacia muchos productos.
    /// EF Core la usa para materializar la relacion cuando se usa Include.
    /// </summary>
    public ICollection<Producto> Productos { get; set; } = new List<Producto>(); // Coleccion inicializada para evitar null.
}
