using GuiaEfRazorAxios.Models; // Importa la entidad Producto para usarla en el contrato.

namespace GuiaEfRazorAxios.Interfaces; // Namespace comun para abstracciones.

/// <summary>
/// Define las operaciones de acceso a datos para productos.
/// Mantiene las consultas de EF Core fuera de controladores y servicios.
/// </summary>
public interface IProductoRepository // Contrato que implementara ProductoRepository.
{
    Task<IReadOnlyList<Producto>> GetAllWithCategoryAsync(); // Lista productos incluyendo la categoria asociada.
    Task AddAsync(Producto producto); // Agrega un producto al seguimiento de EF Core.
    Task SaveChangesAsync(); // Guarda los cambios pendientes en SQL Server.
}
