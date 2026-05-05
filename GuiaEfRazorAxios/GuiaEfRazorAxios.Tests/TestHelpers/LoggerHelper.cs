using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GuiaEfRazorAxios.Tests.TestHelpers;

/// <summary>
/// Helper para crear loggers nulos en pruebas unitarias.
/// NullLogger cumple la interfaz ILogger sin escribir en consola ni archivos.
/// </summary>
public static class LoggerHelper
{
    public static ILogger<T> Create<T>() => NullLogger<T>.Instance;
}
