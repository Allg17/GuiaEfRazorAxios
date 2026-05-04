using GuiaEfRazorAxios.Dtos; // Importa DTOs usados para selects/API.
using GuiaEfRazorAxios.Interfaces; // Importa contratos de repositorio y servicio.
using GuiaEfRazorAxios.Models; // Importa entidad Categoria.

namespace GuiaEfRazorAxios.Services; // Namespace de servicios de negocio.

/// <summary>
/// Servicio de negocio para categorias.
/// Contiene reglas y coordinacion; no conoce detalles de SQL porque usa interfaces.
/// </summary>
public class CategoriaService : ICategoriaService // Implementa el contrato ICategoriaService.
{
    private readonly ICategoriaRepository _categoriaRepository; // Repositorio usado para datos de categorias.
    private readonly ILogger<CategoriaService> _logger; // Logger usado para registrar pasos de negocio.

    public CategoriaService(ICategoriaRepository categoriaRepository, ILogger<CategoriaService> logger) // Constructor DI.
    {
        _categoriaRepository = categoriaRepository; // Guarda repositorio inyectado.
        _logger = logger; // Guarda logger inyectado.
    }

    public async Task<IReadOnlyList<Categoria>> GetCategoriasWithProductsAsync() // Devuelve categorias para vista.
    {
        _logger.LogInformation("Servicio: preparando listado de categorias con conteo de productos."); // Log del caso de uso.
        return await _categoriaRepository.GetAllWithProductsAsync(); // Delega consulta al repositorio.
    }

    public async Task<IReadOnlyList<CategoriaOptionDto>> GetCategoriaOptionsAsync() // Devuelve categorias ligeras.
    {
        _logger.LogInformation("Servicio: preparando categorias para combos y API."); // Log del caso de uso.
        var categorias = await _categoriaRepository.GetAllAsync(); // Obtiene entidades desde repositorio.

        return categorias // Inicia transformacion de entidades a DTOs.
            .Select(categoria => new CategoriaOptionDto // Crea DTO por cada categoria.
            {
                Id = categoria.Id, // Copia Id para usarlo como value del select.
                Nombre = categoria.Nombre // Copia Nombre para mostrarlo al usuario.
            })
            .ToList(); // Materializa resultado en lista.
    }

    public async Task<Categoria?> GetCategoriaAsync(int id) // Busca categoria simple.
    {
        _logger.LogInformation("Servicio: obteniendo categoria {CategoriaId}.", id); // Log con Id recibido.
        return await _categoriaRepository.GetByIdAsync(id); // Delega busqueda.
    }

    public async Task<Categoria?> GetCategoriaWithProductsAsync(int id) // Busca categoria con productos.
    {
        _logger.LogInformation("Servicio: obteniendo categoria {CategoriaId} con productos.", id); // Log con Id.
        return await _categoriaRepository.GetByIdWithProductsAsync(id); // Delega busqueda con Include.
    }

    public async Task CreateCategoriaAsync(Categoria categoria) // Caso de uso crear categoria.
    {
        _logger.LogInformation("Servicio: creando categoria {CategoriaNombre}.", categoria.Nombre); // Log de alta.
        await _categoriaRepository.AddAsync(categoria); // Prepara insercion.
        await _categoriaRepository.SaveChangesAsync(); // Guarda cambios.
    }

    public async Task<OperationResult> UpdateCategoriaAsync(int id, Categoria categoria) // Caso de uso editar.
    {
        _logger.LogInformation("Servicio: actualizando categoria {CategoriaId}.", id); // Log de inicio.

        if (id != categoria.Id) // Valida consistencia entre ruta y formulario.
        {
            _logger.LogWarning("Servicio: intento de actualizar categoria con Id inconsistente. Ruta {RouteId}, modelo {ModelId}.", id, categoria.Id); // Log de advertencia.
            return OperationResult.Failure("El Id de la ruta no coincide con el Id del formulario."); // Devuelve error controlado.
        }

        var categoriaExists = await _categoriaRepository.GetByIdAsync(id); // Verifica que exista antes de actualizar.
        if (categoriaExists is null) // Si no existe, no se puede editar.
        {
            _logger.LogWarning("Servicio: no se encontro la categoria {CategoriaId} para actualizar.", id); // Log de caso no encontrado.
            return OperationResult.Failure("La categoria no existe."); // Error de negocio.
        }

        _categoriaRepository.Update(categoria); // Marca entidad como modificada.
        await _categoriaRepository.SaveChangesAsync(); // Persiste cambios.

        return OperationResult.Success(); // Informa exito.
    }

    public async Task<OperationResult> DeleteCategoriaAsync(int id) // Caso de uso eliminar.
    {
        _logger.LogInformation("Servicio: eliminando categoria {CategoriaId}.", id); // Log de inicio.

        var categoria = await _categoriaRepository.GetByIdWithProductsAsync(id); // Carga categoria con productos para validar regla.
        if (categoria is null) // Si no existe, no se puede eliminar.
        {
            _logger.LogWarning("Servicio: no se encontro la categoria {CategoriaId} para eliminar.", id); // Log de no encontrado.
            return OperationResult.Failure("La categoria no existe."); // Error controlado.
        }

        if (categoria.Productos.Any()) // Regla de negocio: no eliminar si tiene hijos.
        {
            _logger.LogWarning("Servicio: categoria {CategoriaId} no eliminada porque tiene productos.", id); // Log de regla bloqueante.
            return OperationResult.Failure("No se puede eliminar una categoria que tiene productos relacionados."); // Mensaje para usuario.
        }

        _categoriaRepository.Remove(categoria); // Marca eliminacion.
        await _categoriaRepository.SaveChangesAsync(); // Guarda eliminacion.

        return OperationResult.Success(); // Informa exito.
    }
}
