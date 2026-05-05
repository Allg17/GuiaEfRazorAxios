using GuiaEfRazorAxios.Dtos;
using GuiaEfRazorAxios.Interfaces;
using GuiaEfRazorAxios.Models;
using GuiaEfRazorAxios.Services;
using GuiaEfRazorAxios.Tests.TestHelpers;
using Moq;

namespace GuiaEfRazorAxios.Tests.Services;

public class ProductoServiceTests
{
    private readonly Mock<IProductoRepository> _productoRepository = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly ProductoService _service;

    public ProductoServiceTests()
    {
        _service = new ProductoService(
            _productoRepository.Object,
            _categoriaRepository.Object,
            LoggerHelper.Create<ProductoService>());
    }

    [Fact]
    public async Task GetProductosForApiAsync_ShouldMapProductoEntitiesToDtos()
    {
        _productoRepository
            .Setup(repository => repository.GetAllWithCategoryAsync())
            .ReturnsAsync(new List<Producto>
            {
                new()
                {
                    Id = 1,
                    Nombre = "Laptop",
                    Precio = 8500,
                    Stock = 4,
                    Categoria = new Categoria { Id = 1, Nombre = "Computadoras" }
                }
            });

        IReadOnlyList<ProductoListDto> result = await _service.GetProductosForApiAsync();

        var producto = Assert.Single(result);
        Assert.Equal(1, producto.Id);
        Assert.Equal("Laptop", producto.Nombre);
        Assert.Equal(8500, producto.Precio);
        Assert.Equal(4, producto.Stock);
        Assert.Equal("Computadoras", producto.Categoria);
    }

    [Fact]
    public async Task CreateProductoAsync_WhenCategoriaDoesNotExist_ShouldReturnFailure()
    {
        var producto = new Producto { Nombre = "Mouse", CategoriaId = 99 };
        _categoriaRepository
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Categoria?)null);

        var result = await _service.CreateProductoAsync(producto);

        Assert.False(result.Succeeded);
        Assert.Equal("La categoria seleccionada no existe.", result.ErrorMessage);
        _productoRepository.Verify(repository => repository.AddAsync(It.IsAny<Producto>()), Times.Never);
        _productoRepository.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateProductoAsync_WhenCategoriaExists_ShouldAddAndSave()
    {
        var producto = new Producto { Nombre = "Mouse", CategoriaId = 1 };
        _categoriaRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(new Categoria { Id = 1, Nombre = "Perifericos" });

        var result = await _service.CreateProductoAsync(producto);

        Assert.True(result.Succeeded);
        _productoRepository.Verify(repository => repository.AddAsync(producto), Times.Once);
        _productoRepository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateProductoFromApiAsync_ShouldConvertDtoToEntityAndReuseCreateRule()
    {
        var dto = new ProductoCreateDto
        {
            Nombre = "Teclado",
            Precio = 420,
            Stock = 10,
            CategoriaId = 2
        };

        _categoriaRepository
            .Setup(repository => repository.GetByIdAsync(2))
            .ReturnsAsync(new Categoria { Id = 2, Nombre = "Perifericos" });

        var result = await _service.CreateProductoFromApiAsync(dto);

        Assert.True(result.Succeeded);
        _productoRepository.Verify(
            repository => repository.AddAsync(It.Is<Producto>(producto =>
                producto.Nombre == "Teclado" &&
                producto.Precio == 420 &&
                producto.Stock == 10 &&
                producto.CategoriaId == 2)),
            Times.Once);
        _productoRepository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }
}
