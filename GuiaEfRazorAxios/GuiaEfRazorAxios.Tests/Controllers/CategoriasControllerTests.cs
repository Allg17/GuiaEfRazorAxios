using GuiaEfRazorAxios.Controllers;
using GuiaEfRazorAxios.Interfaces;
using GuiaEfRazorAxios.Models;
using GuiaEfRazorAxios.Services;
using GuiaEfRazorAxios.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GuiaEfRazorAxios.Tests.Controllers;

public class CategoriasControllerTests
{
    private readonly Mock<ICategoriaService> _categoriaService = new();
    private readonly CategoriasController _controller;

    public CategoriasControllerTests()
    {
        _controller = new CategoriasController(
            _categoriaService.Object,
            LoggerHelper.Create<CategoriasController>());
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithCategorias()
    {
        var categorias = new List<Categoria> { new() { Id = 1, Nombre = "Computadoras" } };
        _categoriaService
            .Setup(service => service.GetCategoriasWithProductsAsync())
            .ReturnsAsync(categorias);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(categorias, viewResult.Model);
    }

    [Fact]
    public async Task Details_WhenCategoriaDoesNotExist_ShouldReturnNotFound()
    {
        _categoriaService
            .Setup(service => service.GetCategoriaWithProductsAsync(99))
            .ReturnsAsync((Categoria?)null);

        var result = await _controller.Details(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_WhenCategoriaExists_ShouldReturnViewWithCategoria()
    {
        var categoria = new Categoria { Id = 1, Nombre = "Computadoras" };
        _categoriaService
            .Setup(service => service.GetCategoriaWithProductsAsync(1))
            .ReturnsAsync(categoria);

        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(categoria, viewResult.Model);
    }

    [Fact]
    public async Task CreatePost_WhenModelStateIsInvalid_ShouldReturnSameView()
    {
        var categoria = new Categoria();
        _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

        var result = await _controller.Create(categoria);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(categoria, viewResult.Model);
        _categoriaService.Verify(service => service.CreateCategoriaAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task CreatePost_WhenValid_ShouldCreateAndRedirectToIndex()
    {
        var categoria = new Categoria { Nombre = "Software" };

        var result = await _controller.Create(categoria);

        _categoriaService.Verify(service => service.CreateCategoriaAsync(categoria), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CategoriasController.Index), redirect.ActionName);
    }

    [Fact]
    public async Task EditGet_WhenCategoriaDoesNotExist_ShouldReturnNotFound()
    {
        _categoriaService
            .Setup(service => service.GetCategoriaAsync(99))
            .ReturnsAsync((Categoria?)null);

        var result = await _controller.Edit(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task EditPost_WhenServiceFails_ShouldReturnViewWithModelError()
    {
        var categoria = new Categoria { Id = 1, Nombre = "Software" };
        _categoriaService
            .Setup(service => service.UpdateCategoriaAsync(1, categoria))
            .ReturnsAsync(OperationResult.Failure("La categoria no existe."));

        var result = await _controller.Edit(1, categoria);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(categoria, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task DeleteConfirmed_WhenServiceSucceeds_ShouldRedirectToIndex()
    {
        _categoriaService
            .Setup(service => service.DeleteCategoriaAsync(1))
            .ReturnsAsync(OperationResult.Success());

        var result = await _controller.DeleteConfirmed(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CategoriasController.Index), redirect.ActionName);
    }

    [Fact]
    public async Task DeleteConfirmed_WhenServiceFails_ShouldReturnViewWithModelError()
    {
        var categoria = new Categoria { Id = 1, Nombre = "Perifericos" };
        _categoriaService
            .Setup(service => service.DeleteCategoriaAsync(1))
            .ReturnsAsync(OperationResult.Failure("No se puede eliminar."));
        _categoriaService
            .Setup(service => service.GetCategoriaWithProductsAsync(1))
            .ReturnsAsync(categoria);

        var result = await _controller.DeleteConfirmed(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(categoria, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
    }
}
