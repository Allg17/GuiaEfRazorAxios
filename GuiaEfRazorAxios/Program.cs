using GuiaEfRazorAxios.Data; // Importa AppDbContext y DatabaseSeeder, que pertenecen a la capa de datos.
using GuiaEfRazorAxios.Interfaces; // Importa los contratos para registrar dependencias por abstraccion.
using GuiaEfRazorAxios.Repositories; // Importa las implementaciones concretas del patron Repository.
using GuiaEfRazorAxios.Services; // Importa las implementaciones concretas de la capa de servicios.
using Microsoft.EntityFrameworkCore; // Importa UseSqlServer y herramientas de Entity Framework Core.

// Crea el builder principal de ASP.NET Core.
// El builder lee configuracion, variables de entorno, appsettings.json y prepara servicios.
var builder = WebApplication.CreateBuilder(args);

// Limpia proveedores de logs registrados por defecto.
// Se hace para que el ejemplo sea explicito y el estudiante vea que proveedores se usan.
builder.Logging.ClearProviders();

// Agrega logging por consola.
// Sirve para ver mensajes de controladores, servicios y repositorios al ejecutar dotnet run.
builder.Logging.AddConsole();

// Agrega logging para herramientas de depuracion como Visual Studio.
// Sirve para ver logs en la ventana Output/Debug.
builder.Logging.AddDebug();

// Registra MVC con controladores y vistas Razor.
// Sin esta linea, ASP.NET Core no podria resolver rutas hacia controladores MVC.
builder.Services.AddControllersWithViews();

// Registra AppDbContext en el contenedor de inyeccion de dependencias.
// Cada solicitud HTTP recibira una instancia Scoped del contexto.
builder.Services.AddDbContext<AppDbContext>(options =>
    // Configura SQL Server usando la cadena de conexion llamada DefaultConnection.
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra la interfaz ICategoriaRepository con su implementacion CategoriaRepository.
// Esto aplica inversion de dependencias: se pide una interfaz y ASP.NET entrega la clase concreta.
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// Registra la interfaz IProductoRepository con su implementacion ProductoRepository.
// Scoped significa que se crea una instancia por solicitud HTTP.
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Registra el servicio de negocio de categorias.
// Los controladores usan ICategoriaService en vez de instanciar CategoriaService manualmente.
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

// Registra el servicio de negocio de productos.
// Esta capa evita poner reglas de negocio dentro del controlador.
builder.Services.AddScoped<IProductoService, ProductoService>();

// Construye la aplicacion web con todos los servicios registrados arriba.
var app = builder.Build();

// Verifica la base de datos al iniciar.
// Para una practica academica usa EnsureCreatedAsync y datos semilla definidos en AppDbContext.
await DatabaseSeeder.CreateDatabaseIfNeededAsync(app);

// Si la app NO esta en modo desarrollo, usa una pagina controlada para errores.
// En desarrollo normalmente se muestra una pagina tecnica mas detallada.
if (!app.Environment.IsDevelopment())
{
    // Redirige errores no controlados hacia /Home/Error.
    app.UseExceptionHandler("/Home/Error");

    // Activa HTTP Strict Transport Security para entornos productivos.
    app.UseHsts();
}

// Redirige solicitudes HTTP hacia HTTPS cuando corresponde.
app.UseHttpsRedirection();

// Permite servir archivos estaticos desde wwwroot.
// Ejemplo: CSS, JavaScript, Bootstrap, imagenes y favicon.
app.UseStaticFiles();

// Activa el sistema de rutas.
// A partir de aqui ASP.NET Core puede decidir que controlador/accion ejecutar.
app.UseRouting();

// Activa autorizacion.
// Aunque este ejemplo no tiene login, se deja para mostrar el orden comun del pipeline.
app.UseAuthorization();

// Define la ruta MVC por defecto.
// Si el usuario entra a "/", se ejecuta HomeController.Index.
app.MapControllerRoute(
    // Nombre interno de la ruta.
    name: "default",
    // Patron: controlador, accion y parametro opcional id.
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicia la aplicacion y queda escuchando solicitudes HTTP.
app.Run();
