using GuiaEfRazorAxios.Controllers;
using GuiaEfRazorAxios.Dtos;
using GuiaEfRazorAxios.Interfaces;
using GuiaEfRazorAxios.Services;
using GuiaEfRazorAxios.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GuiaEfRazorAxios.Tests.Controllers;

public class ApiProductosControllerTests
{
    private readonly Mock<IProductoService> _productoService = new();
    private readonly Mock<ICategoriaService> _categoriaService = new();
    private readonly ApiProductosController _controller;

    public ApiProductosControllerTests()
    {
        _controller = new ApiProductosController(
            _productoService.Object,
            _categoriaService.Object,
            LoggerHelper.Create<ApiProductosController>());
    }

    [Fact]
    public async Task GetProductos_ShouldReturnOkWithProductos()
    {
        var productos = new List<ProductoListDto>
        {
            new() { Id = 1, Nombre = "Laptop", Categoria = "Computadoras", Precio = 8500, Stock = 4 }
        };
        _productoService
            .Setup(service => service.GetProductosForApiAsync())
            .ReturnsAsync(productos);

        var result = await _controller.GetProductos();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(productos, ok.Value);
    }

    [Fact]
    public async Task GetCategorias_ShouldReturnOkWithCategorias()
    {
        var categorias = new List<CategoriaOptionDto>
        {
            new() { Id = 1, Nombre = "Computadoras" }
        };
        _categoriaService
            .Setup(service => service.GetCategoriaOptionsAsync())
            .ReturnsAsync(categorias);

        var result = await _controller.GetCategorias();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(categorias, ok.Value);
    }

    [Fact]
    public async Task CreateProducto_WhenModelStateIsInvalid_ShouldReturnValidationProblem()
    {
        _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
        var dto = new ProductoCreateDto();

        var result = await _controller.CreateProducto(dto);

        Assert.IsType<ObjectResult>(result);
        _productoService.Verify(service => service.CreateProductoFromApiAsync(It.IsAny<ProductoCreateDto>()), Times.Never);
    }

    [Fact]
    public async Task CreateProducto_WhenServiceFails_ShouldReturnValidationProblem()
    {
        var dto = new ProductoCreateDto { Nombre = "Mouse", CategoriaId = 99, Precio = 10, Stock = 1 };
        _productoService
            .Setup(service => service.CreateProductoFromApiAsync(dto))
            .ReturnsAsync(OperationResult.Failure("La categoria seleccionada no existe."));

        var result = await _controller.CreateProducto(dto);

        Assert.IsType<ObjectResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task CreateProducto_WhenServiceSucceeds_ShouldReturnCreated()
    {
        var dto = new ProductoCreateDto { Nombre = "Mouse", CategoriaId = 1, Precio = 10, Stock = 1 };
        _productoService
            .Setup(service => service.CreateProductoFromApiAsync(dto))
            .ReturnsAsync(OperationResult.Success());

        var result = await _controller.CreateProducto(dto);

        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal("/api/productos", created.Location);
    }
}
