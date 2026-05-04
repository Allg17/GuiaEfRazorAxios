namespace GuiaEfRazorAxios.Services; // Namespace de servicios y objetos auxiliares de negocio.

/// <summary>
/// Resultado estandar para operaciones de negocio.
/// Permite comunicar exito o error sin lanzar excepciones para situaciones esperadas.
/// </summary>
public class OperationResult // Clase usada por servicios para devolver resultado.
{
    private OperationResult(bool succeeded, string? errorMessage) // Constructor privado para obligar a usar metodos factory.
    {
        Succeeded = succeeded; // Guarda si la operacion fue exitosa.
        ErrorMessage = errorMessage; // Guarda el mensaje de error cuando existe.
    }

    public bool Succeeded { get; } // Propiedad de solo lectura que indica exito o fallo.
    public string? ErrorMessage { get; } // Mensaje opcional para mostrar o registrar errores de negocio.

    public static OperationResult Success() => new(true, null); // Crea un resultado exitoso.
    public static OperationResult Failure(string errorMessage) => new(false, errorMessage); // Crea un resultado fallido.
}
