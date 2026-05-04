using GuiaEfRazorAxios.Dtos; // DTOs usados por la API.
using GuiaEfRazorAxios.Models; // Entidad Producto usada por las vistas Razor.
using GuiaEfRazorAxios.Services; // OperationResult comunica exito o error de negocio.

namespace GuiaEfRazorAxios.Interfaces; // Namespace dedicado a contratos.

/// <summary>
/// Define las operaciones de negocio disponibles para productos.
/// La capa web depende de esta interfaz para no conocer detalles de base de datos.
/// </summary>
public interface IProductoService // Contrato de la capa de servicios para productos.
{
    Task<IReadOnlyList<Producto>> GetProductosWithCategoryAsync(); // Lista productos con categoria para Razor.
    Task<IReadOnlyList<ProductoListDto>> GetProductosForApiAsync(); // Lista productos transformados a DTO para JSON.
    Task<OperationResult> CreateProductoAsync(Producto producto); // Crea producto desde formulario Razor.
    Task<OperationResult> CreateProductoFromApiAsync(ProductoCreateDto productoDto); // Crea producto desde JSON enviado por Axios.
}
