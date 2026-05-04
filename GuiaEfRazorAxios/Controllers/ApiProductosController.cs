using GuiaEfRazorAxios.Dtos; // Importa ProductoCreateDto para recibir JSON de Axios.
using GuiaEfRazorAxios.Interfaces; // Importa interfaces de servicios para depender de abstracciones.
using Microsoft.AspNetCore.Mvc; // Importa ControllerBase, IActionResult y atributos HTTP.

namespace GuiaEfRazorAxios.Controllers; // Namespace de controladores.

/// <summary>
/// Controlador API.
/// Devuelve JSON para clientes como JavaScript con Axios.
/// </summary>
[ApiController] // Activa comportamientos de API: validacion automatica y binding desde JSON.
[Route("api/productos")] // Define la ruta base del controlador.
public class ApiProductosController : ControllerBase // ControllerBase se usa para APIs sin vistas Razor.
{
    private readonly IProductoService _productoService; // Servicio de productos, contiene reglas de negocio.
    private readonly ICategoriaService _categoriaService; // Servicio de categorias, usado para combos.
    private readonly ILogger<ApiProductosController> _logger; // Logger para registrar solicitudes API.

    public ApiProductosController( // Constructor usado por inyeccion de dependencias.
        IProductoService productoService, // ASP.NET inyecta implementacion registrada en Program.cs.
        ICategoriaService categoriaService, // ASP.NET inyecta servicio de categorias.
        ILogger<ApiProductosController> logger) // ASP.NET inyecta logger tipado.
    {
        _productoService = productoService; // Guarda servicio de productos.
        _categoriaService = categoriaService; // Guarda servicio de categorias.
        _logger = logger; // Guarda logger.
    }

    [HttpGet] // Responde a GET /api/productos.
    public async Task<IActionResult> GetProductos() // Devuelve productos en formato JSON.
    {
        _logger.LogInformation("API: solicitud GET /api/productos."); // Registra la solicitud.
        var productos = await _productoService.GetProductosForApiAsync(); // Pide DTOs al servicio.
        return Ok(productos); // Devuelve HTTP 200 con JSON.
    }

    [HttpGet("categorias")] // Responde a GET /api/productos/categorias.
    public async Task<IActionResult> GetCategorias() // Devuelve categorias para el select.
    {
        _logger.LogInformation("API: solicitud GET /api/productos/categorias."); // Log de lectura.
        var categorias = await _categoriaService.GetCategoriaOptionsAsync(); // Pide DTOs livianos.
        return Ok(categorias); // Devuelve HTTP 200 con JSON.
    }

    [HttpPost] // Responde a POST /api/productos.
    public async Task<IActionResult> CreateProducto([FromBody] ProductoCreateDto productoDto) // [FromBody] indica que el DTO se construye leyendo el JSON del cuerpo HTTP.
    {
        _logger.LogInformation("API: solicitud POST /api/productos."); // Registra intento de creacion.

        if (!ModelState.IsValid) // Verifica atributos de validacion del DTO.
        {
            _logger.LogWarning("API: DTO de producto invalido."); // Log de error de cliente.
            return ValidationProblem(ModelState); // Devuelve HTTP 400 con detalles de validacion.
        }

        var result = await _productoService.CreateProductoFromApiAsync(productoDto); // Delega creacion al servicio.
        if (!result.Succeeded) // Revisa errores de negocio.
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!); // Agrega mensaje al ModelState.
            return ValidationProblem(ModelState); // Devuelve error entendible por el cliente.
        }

        // Created devuelve HTTP 201 Created.
        // El primer parametro indica una URL relacionada al recurso creado.
        // El segundo parametro seria el cuerpo de respuesta; aqui se usa null porque la vista solo necesita saber que se guardo.
        return Created("/api/productos", null);
    }
}
