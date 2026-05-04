namespace GuiaEfRazorAxios.Data; // Namespace de la capa de datos.

/// <summary>
/// Clase auxiliar para preparar la base de datos al iniciar la aplicacion.
/// En proyectos academicos simplifica la primera ejecucion.
/// </summary>
public static class DatabaseSeeder // Clase static porque no necesita guardar estado ni crear instancias.
{
    public static async Task CreateDatabaseIfNeededAsync(WebApplication app) // Metodo async porque consulta SQL Server.
    {
        using var scope = app.Services.CreateScope(); // Crea un alcance de DI para resolver servicios Scoped.
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Obtiene AppDbContext desde el contenedor.
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder"); // Crea logger especifico.

        logger.LogInformation("Base de datos: verificando si SQL LocalDB y las tablas existen."); // Log de inicio.
        await context.Database.EnsureCreatedAsync(); // Crea base y tablas si aun no existen.
        logger.LogInformation("Base de datos: verificacion finalizada."); // Log de finalizacion.
    } // Fin del metodo de preparacion.
} // Fin de DatabaseSeeder.
