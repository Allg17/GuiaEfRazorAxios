using GuiaEfRazorAxios.Dtos; // Permite devolver DTOs para la API o combos.
using GuiaEfRazorAxios.Models; // Permite recibir/devolver entidades Categoria en vistas Razor.
using GuiaEfRazorAxios.Services; // Permite usar OperationResult como resultado de reglas de negocio.

namespace GuiaEfRazorAxios.Interfaces; // Todas las interfaces viven en una carpeta separada.

/// <summary>
/// Define las operaciones de negocio disponibles para categorias.
/// El controlador conoce esta interfaz, no la implementacion concreta.
/// </summary>
public interface ICategoriaService // Contrato de la capa de servicios para categorias.
{
    Task<IReadOnlyList<Categoria>> GetCategoriasWithProductsAsync(); // Entrega categorias con productos para vistas Razor.
    Task<IReadOnlyList<CategoriaOptionDto>> GetCategoriaOptionsAsync(); // Entrega datos ligeros para selects y API.
    Task<Categoria?> GetCategoriaAsync(int id); // Busca una categoria sin cargar relaciones.
    Task<Categoria?> GetCategoriaWithProductsAsync(int id); // Busca una categoria con sus productos relacionados.
    Task CreateCategoriaAsync(Categoria categoria); // Crea una categoria nueva.
    Task<OperationResult> UpdateCategoriaAsync(int id, Categoria categoria); // Edita y devuelve exito/error de negocio.
    Task<OperationResult> DeleteCategoriaAsync(int id); // Elimina validando reglas como productos relacionados.
}
