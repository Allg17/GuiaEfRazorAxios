using GuiaEfRazorAxios.Models; // Importa la entidad Categoria para usarla en las firmas del contrato.

namespace GuiaEfRazorAxios.Interfaces; // Agrupa todas las abstracciones en una carpeta/namespace dedicado.

/// <summary>
/// Define las operaciones de lectura y escritura para categorias.
/// Esta interfaz aplica inversion de dependencias: las capas superiores dependen del contrato, no de EF Core.
/// </summary>
public interface ICategoriaRepository // Interface: solo declara que se puede hacer, no como se hace.
{
    Task<IReadOnlyList<Categoria>> GetAllWithProductsAsync(); // Obtiene categorias incluyendo sus productos relacionados.
    Task<IReadOnlyList<Categoria>> GetAllAsync(); // Obtiene categorias sin relaciones para listas simples.
    Task<Categoria?> GetByIdAsync(int id); // Busca una categoria por llave primaria; puede devolver null.
    Task<Categoria?> GetByIdWithProductsAsync(int id); // Busca una categoria e incluye sus productos.
    Task AddAsync(Categoria categoria); // Prepara una categoria nueva para guardarla en la base.
    void Update(Categoria categoria); // Marca una categoria existente como modificada.
    void Remove(Categoria categoria); // Marca una categoria existente para eliminacion.
    Task SaveChangesAsync(); // Confirma en SQL Server los cambios pendientes del DbContext.
}
