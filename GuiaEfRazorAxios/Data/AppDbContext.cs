using GuiaEfRazorAxios.Models; // Importa las entidades que EF Core convertira en tablas.
using Microsoft.EntityFrameworkCore; // Importa DbContext, DbSet y ModelBuilder.

namespace GuiaEfRazorAxios.Data; // Namespace de la capa de datos.

/// <summary>
/// DbContext principal de la aplicacion.
/// Su funcion es representar una sesion de trabajo con SQL Server.
/// </summary>
public class AppDbContext : DbContext // Hereda de DbContext para obtener funcionalidad de EF Core.
{
    public AppDbContext(DbContextOptions<AppDbContext> options) // Recibe opciones configuradas en Program.cs.
        : base(options) // Entrega esas opciones a la clase base DbContext.
    {
    } // Constructor vacio porque la configuracion se recibe por inyeccion de dependencias.

    public DbSet<Categoria> Categorias => Set<Categoria>(); // Representa la tabla Categorias.
    public DbSet<Producto> Productos => Set<Producto>(); // Representa la tabla Productos.

    protected override void OnModelCreating(ModelBuilder modelBuilder) // Metodo para configurar el modelo de EF Core.
    {
        base.OnModelCreating(modelBuilder); // Mantiene configuraciones base de EF Core.

        modelBuilder.Entity<Categoria>() // Selecciona la entidad Categoria para configurar su relacion.
            .HasMany(categoria => categoria.Productos) // Una categoria tiene muchos productos.
            .WithOne(producto => producto.Categoria) // Cada producto tiene una categoria.
            .HasForeignKey(producto => producto.CategoriaId) // La llave foranea esta en Producto.CategoriaId.
            .OnDelete(DeleteBehavior.Restrict); // Evita borrar categorias con productos relacionados.

        modelBuilder.Entity<Categoria>().HasData( // Carga datos iniciales para la tabla Categorias.
            new Categoria { Id = 1, Nombre = "Computadoras", Descripcion = "Equipos para trabajo, estudio y desarrollo." }, // Fila inicial 1.
            new Categoria { Id = 2, Nombre = "Perifericos", Descripcion = "Dispositivos de entrada y salida." } // Fila inicial 2.
        ); // Fin de datos semilla de categorias.

        modelBuilder.Entity<Producto>().HasData( // Carga datos iniciales para la tabla Productos.
            new Producto { Id = 1, Nombre = "Laptop para programacion", Precio = 8500, Stock = 4, CategoriaId = 1 }, // Producto relacionado a Computadoras.
            new Producto { Id = 2, Nombre = "Monitor 24 pulgadas", Precio = 1350, Stock = 8, CategoriaId = 2 }, // Producto relacionado a Perifericos.
            new Producto { Id = 3, Nombre = "Teclado mecanico", Precio = 420, Stock = 12, CategoriaId = 2 } // Producto relacionado a Perifericos.
        ); // Fin de datos semilla de productos.
    } // Fin de la configuracion del modelo.
} // Fin del DbContext.
