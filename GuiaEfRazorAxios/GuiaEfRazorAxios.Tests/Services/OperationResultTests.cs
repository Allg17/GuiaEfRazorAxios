using GuiaEfRazorAxios.Services;

namespace GuiaEfRazorAxios.Tests.Services;

public class OperationResultTests
{
    [Fact]
    public void Success_ShouldReturnSucceededTrueAndNoError()
    {
        var result = OperationResult.Success();

        Assert.True(result.Succeeded);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Failure_ShouldReturnSucceededFalseAndErrorMessage()
    {
        var result = OperationResult.Failure("Error de prueba");

        Assert.False(result.Succeeded);
        Assert.Equal("Error de prueba", result.ErrorMessage);
    }
}
