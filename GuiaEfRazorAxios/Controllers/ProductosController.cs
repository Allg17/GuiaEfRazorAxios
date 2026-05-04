using GuiaEfRazorAxios.Interfaces; // Importa interfaces para usar servicios sin depender de clases concretas.
using GuiaEfRazorAxios.Models; // Importa la entidad Producto usada por las vistas Razor.
using Microsoft.AspNetCore.Mvc; // Importa Controller, IActionResult y atributos MVC.
using Microsoft.AspNetCore.Mvc.Rendering; // Importa SelectList, usado para llenar el combo de categorias.

namespace GuiaEfRazorAxios.Controllers;

/// <summary>
/// Controlador MVC de productos.
/// No consulta EF Core directamente; delega a servicios de aplicacion.
/// </summary>
public class ProductosController : Controller
{
    private readonly IProductoService _productoService; // Servicio que contiene reglas de negocio de productos.
    private readonly ICategoriaService _categoriaService; // Servicio que obtiene categorias para el select.
    private readonly ILogger<ProductosController> _logger; // Logger para registrar pasos del controlador.

    public ProductosController(
        IProductoService productoService,
        ICategoriaService categoriaService,
        ILogger<ProductosController> logger)
    {
        _productoService = productoService; // Guarda servicio inyectado por DI.
        _categoriaService = categoriaService; // Guarda servicio de categorias inyectado.
        _logger = logger; // Guarda logger inyectado.
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Controlador: solicitud GET /Productos.");
        var productos = await _productoService.GetProductosWithCategoryAsync(); // Pide productos al servicio, no al DbContext.
        return View(productos); // Envia la lista a Views/Productos/Index.cshtml.
    }

    public async Task<IActionResult> Create()
    {
        _logger.LogInformation("Controlador: solicitud GET /Productos/Create.");
        await LoadCategoriasAsync(); // Carga opciones del select antes de mostrar el formulario.
        return View(new Producto()); // Envia un Producto vacio para que Razor construya el formulario.
    }

    [HttpPost] // Atiende el envio POST del formulario de creacion.
    [ValidateAntiForgeryToken] // Valida el token antifalsificacion generado automaticamente en el formulario Razor.
    public async Task<IActionResult> Create(Producto producto)
    {
        _logger.LogInformation("Controlador: solicitud POST /Productos/Create.");

        if (!ModelState.IsValid) // ModelState contiene errores de validacion de DataAnnotations o conversion de datos.
        {
            _logger.LogWarning("Controlador: formulario de producto invalido.");
            await LoadCategoriasAsync(producto.CategoriaId); // Recarga el select porque ViewBag no persiste entre requests.
            return View(producto); // Devuelve la misma vista con los datos ingresados y errores de validacion.
        }

        var result = await _productoService.CreateProductoAsync(producto); // Delega la regla de creacion al servicio.
        if (!result.Succeeded) // Si el servicio detecta una regla incumplida, no se redirige.
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!); // Agrega error general visible en asp-validation-summary.
            await LoadCategoriasAsync(producto.CategoriaId); // Vuelve a preparar el select para redibujar el formulario.
            return View(producto); // Muestra formulario con mensaje de error.
        }

        return RedirectToAction(nameof(Index)); // Patron PRG: despues de guardar, redirige para evitar doble envio con F5.
    }

    private async Task LoadCategoriasAsync(int? selectedId = null) // Metodo auxiliar para no repetir codigo del select.
    {
        _logger.LogInformation("Controlador: preparando SelectList de categorias para la vista.");
        var categorias = await _categoriaService.GetCategoriaOptionsAsync(); // Obtiene DTOs con Id y Nombre.

        // ViewBag permite pasar datos dinamicos a la vista sin crear un ViewModel especifico.
        // CategoriaId debe coincidir con asp-items="ViewBag.CategoriaId" en Views/Productos/Create.cshtml.
        // SelectList recibe: lista, propiedad value, propiedad texto, valor seleccionado.
        ViewBag.CategoriaId = new SelectList(categorias, "Id", "Nombre", selectedId);
    }
}
