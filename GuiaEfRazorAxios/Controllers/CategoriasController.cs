using GuiaEfRazorAxios.Interfaces; // Importa ICategoriaService para depender de una abstraccion.
using GuiaEfRazorAxios.Models; // Importa Categoria para recibir formularios Razor.
using Microsoft.AspNetCore.Mvc; // Importa Controller, IActionResult y atributos MVC.

namespace GuiaEfRazorAxios.Controllers;

/// <summary>
/// Controlador MVC de categorias.
/// Su responsabilidad es recibir solicitudes HTTP y delegar el trabajo al servicio.
/// </summary>
public class CategoriasController : Controller
{
    private readonly ICategoriaService _categoriaService; // Servicio con reglas de negocio de categorias.
    private readonly ILogger<CategoriasController> _logger; // Logger para registrar solicitudes y decisiones.

    public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
    {
        _categoriaService = categoriaService; // Guarda servicio inyectado por ASP.NET Core.
        _logger = logger; // Guarda logger inyectado.
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Controlador: solicitud GET /Categorias.");
        var categorias = await _categoriaService.GetCategoriasWithProductsAsync(); // Pide datos al servicio.
        return View(categorias); // Envia datos a Views/Categorias/Index.cshtml.
    }

    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("Controlador: solicitud GET /Categorias/Details/{CategoriaId}.", id);
        var categoria = await _categoriaService.GetCategoriaWithProductsAsync(id); // Busca categoria con productos.
        return categoria is null ? NotFound() : View(categoria); // NotFound devuelve HTTP 404 si no existe.
    }

    public IActionResult Create()
    {
        _logger.LogInformation("Controlador: solicitud GET /Categorias/Create.");
        return View(new Categoria()); // Envia entidad vacia para que la vista genere campos iniciales.
    }

    [HttpPost] // Indica que esta accion atiende envios POST del formulario.
    [ValidateAntiForgeryToken] // Protege contra CSRF validando el token generado por el Form Tag Helper.
    public async Task<IActionResult> Create(Categoria categoria)
    {
        _logger.LogInformation("Controlador: solicitud POST /Categorias/Create.");

        if (!ModelState.IsValid) // Revisa validaciones como Required y StringLength del modelo.
        {
            _logger.LogWarning("Controlador: formulario de categoria invalido.");
            return View(categoria); // Regresa a la vista mostrando los errores y datos capturados.
        }

        await _categoriaService.CreateCategoriaAsync(categoria); // Delega creacion al servicio.
        return RedirectToAction(nameof(Index)); // Redirige al listado despues de guardar correctamente.
    }

    public async Task<IActionResult> Edit(int id)
    {
        _logger.LogInformation("Controlador: solicitud GET /Categorias/Edit/{CategoriaId}.", id);
        var categoria = await _categoriaService.GetCategoriaAsync(id); // Busca entidad existente para llenar formulario.
        return categoria is null ? NotFound() : View(categoria); // Si no existe devuelve 404.
    }

    [HttpPost] // Atiende el envio POST del formulario de edicion.
    [ValidateAntiForgeryToken] // Verifica que el POST venga desde el formulario legitimo de la aplicacion.
    public async Task<IActionResult> Edit(int id, Categoria categoria)
    {
        _logger.LogInformation("Controlador: solicitud POST /Categorias/Edit/{CategoriaId}.", id);

        if (!ModelState.IsValid) // Valida datos enviados por el formulario.
        {
            _logger.LogWarning("Controlador: formulario de edicion de categoria invalido.");
            return View(categoria); // Re-renderiza la vista con mensajes de validacion.
        }

        var result = await _categoriaService.UpdateCategoriaAsync(id, categoria); // Servicio verifica reglas y existencia.
        if (!result.Succeeded) // OperationResult comunica errores de negocio sin excepciones.
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!); // Error general para asp-validation-summary.
            return View(categoria); // Vuelve a mostrar formulario.
        }

        return RedirectToAction(nameof(Index)); // Evita reenvio del formulario al refrescar.
    }

    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Controlador: solicitud GET /Categorias/Delete/{CategoriaId}.", id);
        var categoria = await _categoriaService.GetCategoriaWithProductsAsync(id); // Carga productos para informar la regla.
        return categoria is null ? NotFound() : View(categoria); // Muestra confirmacion si existe.
    }

    [HttpPost, ActionName("Delete")] // Mantiene la ruta /Delete aunque el metodo C# se llame DeleteConfirmed.
    [ValidateAntiForgeryToken] // Protege la eliminacion contra solicitudes POST falsificadas.
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _logger.LogInformation("Controlador: solicitud POST /Categorias/Delete/{CategoriaId}.", id);

        var result = await _categoriaService.DeleteCategoriaAsync(id); // Servicio aplica regla: no borrar con productos.
        if (!result.Succeeded) // Si no se pudo eliminar, se devuelve la vista con error.
        {
            var categoria = await _categoriaService.GetCategoriaWithProductsAsync(id); // Recarga datos para redibujar vista.
            ModelState.AddModelError(string.Empty, result.ErrorMessage!); // Muestra motivo de fallo.
            return categoria is null ? NotFound() : View(categoria); // Si desaparecio, 404; si existe, muestra error.
        }

        return RedirectToAction(nameof(Index)); // Si elimino correctamente, vuelve al listado.
    }
}
