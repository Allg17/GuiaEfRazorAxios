using GuiaEfRazorAxios.Dtos; // Importa DTOs de entrada y salida.
using GuiaEfRazorAxios.Interfaces; // Importa contratos de repositorios y servicio.
using GuiaEfRazorAxios.Models; // Importa entidad Producto.

namespace GuiaEfRazorAxios.Services; // Namespace de servicios.

/// <summary>
/// Servicio de negocio para productos.
/// Valida reglas y transforma datos antes de devolverlos a controladores.
/// </summary>
public class ProductoService : IProductoService // Implementa contrato de productos.
{
    private readonly IProductoRepository _productoRepository; // Repositorio de productos.
    private readonly ICategoriaRepository _categoriaRepository; // Repositorio de categorias para validar FK.
    private readonly ILogger<ProductoService> _logger; // Logger de capa de negocio.

    public ProductoService( // Constructor usado por inyeccion de dependencias.
        IProductoRepository productoRepository, // Dependencia para productos.
        ICategoriaRepository categoriaRepository, // Dependencia para categorias.
        ILogger<ProductoService> logger) // Dependencia para logging.
    {
        _productoRepository = productoRepository; // Guarda repositorio de productos.
        _categoriaRepository = categoriaRepository; // Guarda repositorio de categorias.
        _logger = logger; // Guarda logger.
    }

    public async Task<IReadOnlyList<Producto>> GetProductosWithCategoryAsync() // Lista para vista Razor.
    {
        _logger.LogInformation("Servicio: preparando listado de productos con categorias."); // Log de caso de uso.
        return await _productoRepository.GetAllWithCategoryAsync(); // Delega consulta al repositorio.
    }

    public async Task<IReadOnlyList<ProductoListDto>> GetProductosForApiAsync() // Lista para API JSON.
    {
        _logger.LogInformation("Servicio: preparando productos para respuesta JSON."); // Log de transformacion.
        var productos = await _productoRepository.GetAllWithCategoryAsync(); // Obtiene entidades con categoria.

        return productos // Inicia proyeccion de entidad a DTO.
            .Select(producto => new ProductoListDto // Crea objeto plano para JSON.
            {
                Id = producto.Id, // Copia identificador.
                Nombre = producto.Nombre, // Copia nombre.
                Precio = producto.Precio, // Copia precio.
                Stock = producto.Stock, // Copia stock.
                Categoria = producto.Categoria?.Nombre ?? string.Empty // Convierte relacion Categoria a texto.
            })
            .ToList(); // Materializa la lista DTO.
    }

    public async Task<OperationResult> CreateProductoAsync(Producto producto) // Crea desde Razor.
    {
        _logger.LogInformation("Servicio: creando producto {ProductoNombre} desde Razor.", producto.Nombre); // Log de alta.

        var categoria = await _categoriaRepository.GetByIdAsync(producto.CategoriaId); // Verifica que exista la categoria.
        if (categoria is null) // Si no existe la FK, se bloquea la creacion.
        {
            _logger.LogWarning("Servicio: no se creo producto porque la categoria {CategoriaId} no existe.", producto.CategoriaId); // Log de advertencia.
            return OperationResult.Failure("La categoria seleccionada no existe."); // Error de negocio.
        }

        await _productoRepository.AddAsync(producto); // Prepara insercion.
        await _productoRepository.SaveChangesAsync(); // Guarda en SQL Server.

        return OperationResult.Success(); // Informa exito.
    }

    public async Task<OperationResult> CreateProductoFromApiAsync(ProductoCreateDto productoDto) // Crea desde Axios/API.
    {
        _logger.LogInformation("Servicio: creando producto {ProductoNombre} desde API.", productoDto.Nombre); // Log de API.

        var producto = new Producto // Convierte DTO a entidad de dominio.
        {
            Nombre = productoDto.Nombre, // Copia nombre del JSON.
            Precio = productoDto.Precio, // Copia precio del JSON.
            Stock = productoDto.Stock, // Copia stock del JSON.
            CategoriaId = productoDto.CategoriaId // Copia FK enviada por el select.
        };

        return await CreateProductoAsync(producto); // Reutiliza regla de creacion comun.
    }
}
