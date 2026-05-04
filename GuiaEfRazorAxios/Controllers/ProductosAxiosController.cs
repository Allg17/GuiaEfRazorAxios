using Microsoft.AspNetCore.Mvc; // Importa Controller e IActionResult para MVC.

namespace GuiaEfRazorAxios.Controllers; // Namespace de controladores.

/// <summary>
/// Controlador MVC para la vista HTML con Axios.
/// La vista vive en Views/ProductosAxios/Index.cshtml para seguir el orden MVC.
/// </summary>
public class ProductosAxiosController : Controller // Hereda Controller porque devuelve una vista Razor.
{
    private readonly ILogger<ProductosAxiosController> _logger; // Logger para registrar cuando se abre la vista.

    public ProductosAxiosController(ILogger<ProductosAxiosController> logger) // Constructor DI.
    {
        _logger = logger; // Guarda logger inyectado.
    }

    public IActionResult Index() // Accion que responde a /ProductosAxios.
    {
        _logger.LogInformation("Controlador: mostrando vista HTML con Axios."); // Registra apertura de vista.
        return View(); // Devuelve Views/ProductosAxios/Index.cshtml.
    }
}
