using GuiaEfRazorAxios.Data; // Importa AppDbContext para usar EF Core.
using GuiaEfRazorAxios.Interfaces; // Importa IProductoRepository.
using GuiaEfRazorAxios.Models; // Importa la entidad Producto.
using Microsoft.EntityFrameworkCore; // Importa Include, OrderBy y ToListAsync.

namespace GuiaEfRazorAxios.Repositories; // Namespace de repositorios concretos.

/// <summary>
/// Implementacion concreta del Repository Pattern para Producto.
/// Encapsula consultas y escrituras de productos.
/// </summary>
public class ProductoRepository : IProductoRepository // Implementa el contrato de producto.
{
    private readonly AppDbContext _context; // DbContext usado para hablar con SQL Server.
    private readonly ILogger<ProductoRepository> _logger; // Logger especifico de este repositorio.

    public ProductoRepository(AppDbContext context, ILogger<ProductoRepository> logger) // Constructor de DI.
    {
        _context = context; // Guarda el contexto inyectado.
        _logger = logger; // Guarda el logger inyectado.
    }

    public async Task<IReadOnlyList<Producto>> GetAllWithCategoryAsync() // Lista productos con su categoria.
    {
        _logger.LogInformation("Consultando productos con su categoria."); // Registra inicio de consulta.

        return await _context.Productos // Inicia consulta en tabla Productos.
            .Include(producto => producto.Categoria) // Carga la entidad Categoria relacionada.
            .OrderBy(producto => producto.Nombre) // Ordena por nombre para presentacion.
            .ToListAsync(); // Ejecuta consulta asincrona.
    }

    public async Task AddAsync(Producto producto) // Prepara producto nuevo.
    {
        _logger.LogInformation("Agregando producto {ProductoNombre}.", producto.Nombre); // Log de creacion.
        await _context.Productos.AddAsync(producto); // Agrega producto al contexto.
    }

    public async Task SaveChangesAsync() // Confirma cambios pendientes.
    {
        _logger.LogInformation("Guardando cambios de productos en SQL Server."); // Log antes de guardar.
        await _context.SaveChangesAsync(); // Persiste cambios en SQL Server.
    }
}
