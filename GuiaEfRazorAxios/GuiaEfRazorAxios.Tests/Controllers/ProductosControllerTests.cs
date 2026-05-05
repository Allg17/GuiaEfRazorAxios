using GuiaEfRazorAxios.Controllers;
using GuiaEfRazorAxios.Dtos;
using GuiaEfRazorAxios.Interfaces;
using GuiaEfRazorAxios.Models;
using GuiaEfRazorAxios.Services;
using GuiaEfRazorAxios.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;

namespace GuiaEfRazorAxios.Tests.Controllers;

public class ProductosControllerTests
{
    private readonly Mock<IProductoService> _productoService = new();
    private readonly Mock<ICategoriaService> _categoriaService = new();
    private readonly ProductosController _controller;

    public ProductosControllerTests()
    {
        _controller = new ProductosController(
            _productoService.Object,
            _categoriaService.Object,
            LoggerHelper.Create<ProductosController>());
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithProductos()
    {
        var productos = new List<Producto> { new() { Id = 1, Nombre = "Laptop" } };
        _productoService
            .Setup(service => service.GetProductosWithCategoryAsync())
            .ReturnsAsync(productos);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(productos, viewResult.Model);
    }

    [Fact]
    public async Task CreateGet_ShouldLoadCategoriasSelectListAndReturnEmptyProducto()
    {
        SetupCategorias();

        var result = await _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Producto>(viewResult.Model);
        Assert.IsType<SelectList>(_controller.ViewBag.CategoriaId);
    }

    [Fact]
    public async Task CreatePost_WhenModelStateIsInvalid_ShouldReturnViewAndReloadCategorias()
    {
        SetupCategorias();
        var producto = new Producto { Nombre = "", CategoriaId = 1 };
        _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

        var result = await _controller.Create(producto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(producto, viewResult.Model);
        Assert.IsType<SelectList>(_controller.ViewBag.CategoriaId);
        _productoService.Verify(service => service.CreateProductoAsync(It.IsAny<Producto>()), Times.Never);
    }

    [Fact]
    public async Task CreatePost_WhenServiceFails_ShouldReturnViewWithModelError()
    {
        SetupCategorias();
        var producto = new Producto { Nombre = "Mouse", CategoriaId = 99 };
        _productoService
            .Setup(service => service.CreateProductoAsync(producto))
            .ReturnsAsync(OperationResult.Failure("La categoria seleccionada no existe."));

        var result = await _controller.Create(producto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(producto, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task CreatePost_WhenServiceSucceeds_ShouldRedirectToIndex()
    {
        var producto = new Producto { Nombre = "Mouse", CategoriaId = 1 };
        _productoService
            .Setup(service => service.CreateProductoAsync(producto))
            .ReturnsAsync(OperationResult.Success());

        var result = await _controller.Create(producto);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ProductosController.Index), redirect.ActionName);
    }

    private void SetupCategorias()
    {
        _categoriaService
            .Setup(service => service.GetCategoriaOptionsAsync())
            .ReturnsAsync(new List<CategoriaOptionDto>
            {
                new() { Id = 1, Nombre = "Computadoras" },
                new() { Id = 2, Nombre = "Perifericos" }
            });
    }
}
