using GuiaEfRazorAxios.Controllers;
using GuiaEfRazorAxios.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;

namespace GuiaEfRazorAxios.Tests.Controllers;

public class ProductosAxiosControllerTests
{
    [Fact]
    public void Index_ShouldReturnView()
    {
        var controller = new ProductosAxiosController(LoggerHelper.Create<ProductosAxiosController>());

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }
}
