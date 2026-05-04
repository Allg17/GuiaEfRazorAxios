using GuiaEfRazorAxios.Data; // Importa AppDbContext para consultar SQL Server mediante EF Core.
using GuiaEfRazorAxios.Interfaces; // Importa ICategoriaRepository, contrato que esta clase implementa.
using GuiaEfRazorAxios.Models; // Importa la entidad Categoria.
using Microsoft.EntityFrameworkCore; // Importa Include, ToListAsync y FirstOrDefaultAsync.

namespace GuiaEfRazorAxios.Repositories; // Namespace de implementaciones de acceso a datos.

/// <summary>
/// Implementacion concreta del Repository Pattern para Categoria.
/// Su responsabilidad unica es consultar y modificar datos de categorias.
/// </summary>
public class CategoriaRepository : ICategoriaRepository // Implementa el contrato definido en Interfaces.
{
    private readonly AppDbContext _context; // Campo privado para acceder a EF Core.
    private readonly ILogger<CategoriaRepository> _logger; // Campo privado para registrar pasos del repositorio.

    public CategoriaRepository(AppDbContext context, ILogger<CategoriaRepository> logger) // Constructor usado por DI.
    {
        _context = context; // Guarda el DbContext recibido por inyeccion de dependencias.
        _logger = logger; // Guarda el logger recibido por inyeccion de dependencias.
    }

    public async Task<IReadOnlyList<Categoria>> GetAllWithProductsAsync() // Lista categorias con relacion incluida.
    {
        _logger.LogInformation("Consultando categorias con sus productos relacionados."); // Log de lectura.

        return await _context.Categorias // Inicia consulta sobre tabla Categorias.
            .Include(categoria => categoria.Productos) // Carga los productos relacionados.
            .OrderBy(categoria => categoria.Nombre) // Ordena alfabeticamente para mejorar la vista.
            .ToListAsync(); // Ejecuta la consulta de forma asincrona.
    }

    public async Task<IReadOnlyList<Categoria>> GetAllAsync() // Lista categorias sin productos.
    {
        _logger.LogInformation("Consultando categorias para listas de seleccion."); // Log de lectura simple.

        return await _context.Categorias // Inicia consulta sobre Categorias.
            .OrderBy(categoria => categoria.Nombre) // Ordena por nombre.
            .ToListAsync(); // Ejecuta consulta y devuelve lista.
    }

    public async Task<Categoria?> GetByIdAsync(int id) // Busca por llave primaria.
    {
        _logger.LogInformation("Buscando categoria por Id {CategoriaId}.", id); // Log con parametro estructurado.
        return await _context.Categorias.FindAsync(id); // FindAsync usa la llave primaria y puede devolver null.
    }

    public async Task<Categoria?> GetByIdWithProductsAsync(int id) // Busca categoria y productos relacionados.
    {
        _logger.LogInformation("Buscando categoria {CategoriaId} con productos.", id); // Log de consulta con relacion.

        return await _context.Categorias // Inicia consulta sobre Categorias.
            .Include(categoria => categoria.Productos) // Incluye coleccion Productos.
            .FirstOrDefaultAsync(categoria => categoria.Id == id); // Devuelve primera coincidencia o null.
    }

    public async Task AddAsync(Categoria categoria) // Prepara insercion.
    {
        _logger.LogInformation("Agregando categoria {CategoriaNombre}.", categoria.Nombre); // Log de alta.
        await _context.Categorias.AddAsync(categoria); // Agrega entidad al seguimiento de EF Core.
    }

    public void Update(Categoria categoria) // Prepara actualizacion.
    {
        _logger.LogInformation("Marcando categoria {CategoriaId} para actualizar.", categoria.Id); // Log de edicion.
        _context.Categorias.Update(categoria); // Marca la entidad como modificada.
    }

    public void Remove(Categoria categoria) // Prepara eliminacion.
    {
        _logger.LogInformation("Marcando categoria {CategoriaId} para eliminar.", categoria.Id); // Log de baja.
        _context.Categorias.Remove(categoria); // Marca la entidad para eliminar.
    }

    public async Task SaveChangesAsync() // Confirma cambios.
    {
        _logger.LogInformation("Guardando cambios de categorias en SQL Server."); // Log antes de persistir.
        await _context.SaveChangesAsync(); // Ejecuta INSERT, UPDATE o DELETE pendientes.
    }
}
