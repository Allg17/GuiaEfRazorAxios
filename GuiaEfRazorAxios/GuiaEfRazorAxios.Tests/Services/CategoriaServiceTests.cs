using GuiaEfRazorAxios.Dtos;
using GuiaEfRazorAxios.Interfaces;
using GuiaEfRazorAxios.Models;
using GuiaEfRazorAxios.Services;
using GuiaEfRazorAxios.Tests.TestHelpers;
using Moq;

namespace GuiaEfRazorAxios.Tests.Services;

public class CategoriaServiceTests
{
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly CategoriaService _service;

    public CategoriaServiceTests()
    {
        _service = new CategoriaService(_categoriaRepository.Object, LoggerHelper.Create<CategoriaService>());
    }

    [Fact]
    public async Task GetCategoriaOptionsAsync_ShouldMapCategoriasToDtos()
    {
        _categoriaRepository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<Categoria>
            {
                new() { Id = 1, Nombre = "Computadoras" },
                new() { Id = 2, Nombre = "Perifericos" }
            });

        IReadOnlyList<CategoriaOptionDto> result = await _service.GetCategoriaOptionsAsync();

        Assert.Collection(
            result,
            first =>
            {
                Assert.Equal(1, first.Id);
                Assert.Equal("Computadoras", first.Nombre);
            },
            second =>
            {
                Assert.Equal(2, second.Id);
                Assert.Equal("Perifericos", second.Nombre);
            });
    }

    [Fact]
    public async Task CreateCategoriaAsync_ShouldAddAndSave()
    {
        var categoria = new Categoria { Nombre = "Redes" };

        await _service.CreateCategoriaAsync(categoria);

        _categoriaRepository.Verify(repository => repository.AddAsync(categoria), Times.Once);
        _categoriaRepository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoriaAsync_WhenRouteIdDoesNotMatchModelId_ShouldReturnFailure()
    {
        var categoria = new Categoria { Id = 2, Nombre = "Software" };

        var result = await _service.UpdateCategoriaAsync(1, categoria);

        Assert.False(result.Succeeded);
        Assert.Equal("El Id de la ruta no coincide con el Id del formulario.", result.ErrorMessage);
        _categoriaRepository.Verify(repository => repository.Update(It.IsAny<Categoria>()), Times.Never);
        _categoriaRepository.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateCategoriaAsync_WhenCategoriaDoesNotExist_ShouldReturnFailure()
    {
        var categoria = new Categoria { Id = 1, Nombre = "Software" };
        _categoriaRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync((Categoria?)null);

        var result = await _service.UpdateCategoriaAsync(1, categoria);

        Assert.False(result.Succeeded);
        Assert.Equal("La categoria no existe.", result.ErrorMessage);
        _categoriaRepository.Verify(repository => repository.Update(It.IsAny<Categoria>()), Times.Never);
        _categoriaRepository.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateCategoriaAsync_WhenCategoriaExists_ShouldUpdateAndSave()
    {
        var categoria = new Categoria { Id = 1, Nombre = "Software" };
        _categoriaRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(new Categoria { Id = 1, Nombre = "Anterior" });

        var result = await _service.UpdateCategoriaAsync(1, categoria);

        Assert.True(result.Succeeded);
        _categoriaRepository.Verify(repository => repository.Update(categoria), Times.Once);
        _categoriaRepository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoriaAsync_WhenCategoriaDoesNotExist_ShouldReturnFailure()
    {
        _categoriaRepository
            .Setup(repository => repository.GetByIdWithProductsAsync(99))
            .ReturnsAsync((Categoria?)null);

        var result = await _service.DeleteCategoriaAsync(99);

        Assert.False(result.Succeeded);
        Assert.Equal("La categoria no existe.", result.ErrorMessage);
        _categoriaRepository.Verify(repository => repository.Remove(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCategoriaAsync_WhenCategoriaHasProducts_ShouldReturnFailure()
    {
        var categoria = new Categoria
        {
            Id = 1,
            Nombre = "Perifericos",
            Productos = new List<Producto> { new() { Id = 1, Nombre = "Mouse" } }
        };

        _categoriaRepository
            .Setup(repository => repository.GetByIdWithProductsAsync(1))
            .ReturnsAsync(categoria);

        var result = await _service.DeleteCategoriaAsync(1);

        Assert.False(result.Succeeded);
        Assert.Equal("No se puede eliminar una categoria que tiene productos relacionados.", result.ErrorMessage);
        _categoriaRepository.Verify(repository => repository.Remove(It.IsAny<Categoria>()), Times.Never);
        _categoriaRepository.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteCategoriaAsync_WhenCategoriaHasNoProducts_ShouldRemoveAndSave()
    {
        var categoria = new Categoria { Id = 1, Nombre = "Perifericos" };

        _categoriaRepository
            .Setup(repository => repository.GetByIdWithProductsAsync(1))
            .ReturnsAsync(categoria);

        var result = await _service.DeleteCategoriaAsync(1);

        Assert.True(result.Succeeded);
        _categoriaRepository.Verify(repository => repository.Remove(categoria), Times.Once);
        _categoriaRepository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }
}
