# GuiaEfRazorAxios

Proyecto educativo en **ASP.NET Core MVC** para un estudiante de Ingenieria en Sistemas.

El objetivo es mostrar una aplicacion pequena, pero bien ordenada, documentada y con una arquitectura limpia:

- Controladores delgados.
- Logica de negocio en servicios.
- Acceso a datos en repositorios.
- Interfaces separadas en su propia carpeta.
- Entity Framework Core conectado a SQL Server LocalDB.
- Razor Views.
- Una vista en `Views` que usa HTML, JavaScript y Axios.
- Logging con `Microsoft.Extensions.Logging`.

## 1. Caso de Estudio

El sistema administra **categorias** y **productos**.

La relacion principal es:

```text
Categoria 1 ---- N Producto
```

Significa:

- Una categoria puede tener muchos productos.
- Un producto pertenece a una sola categoria.
- `Producto.CategoriaId` es la llave foranea.
- `Categoria.Productos` es la propiedad de navegacion hacia muchos productos.
- `Producto.Categoria` es la propiedad de navegacion hacia una categoria.

Ejemplo:

```text
Categoria: Perifericos
  - Monitor 24 pulgadas
  - Teclado mecanico
```

## 2. Tecnologias Usadas

- **ASP.NET Core MVC**: organiza la aplicacion en controladores y vistas.
- **Razor Views**: permite crear HTML dinamico con archivos `.cshtml`.
- **Entity Framework Core**: ORM que permite trabajar con SQL usando clases C#.
- **SQL Server LocalDB**: base de datos local para desarrollo.
- **Repository Pattern**: encapsula consultas y operaciones de base de datos.
- **Service Layer**: contiene reglas de negocio.
- **DTOs**: objetos para entrada y salida de datos en la API.
- **Axios**: libreria JavaScript para consumir endpoints HTTP.
- **Microsoft Logger**: registra eventos importantes en consola y debug.

## 3. Arquitectura Limpia Aplicada

El proyecto sigue este flujo:

```text
Navegador
  -> Controller
  -> Service
  -> Repository
  -> AppDbContext
  -> SQL Server
```

Responsabilidad de cada capa:

| Capa | Responsabilidad |
| --- | --- |
| `Controllers` | Reciben solicitudes HTTP y devuelven vistas, redirecciones o JSON. |
| `Services` | Ejecutan reglas de negocio y coordinan operaciones. |
| `Repositories` | Consultan y guardan datos usando Entity Framework Core. |
| `Interfaces` | Definen contratos para depender de abstracciones. |
| `Data` | Configura EF Core, DbContext y base de datos inicial. |
| `Models` | Representan entidades/tablas del dominio. |
| `Dtos` | Representan datos que entran o salen por API. |
| `Views` | Contienen las pantallas Razor. |
| `wwwroot` | Contiene archivos estaticos como CSS, JS y librerias. |

Regla importante del proyecto:

```text
Los controladores no contienen consultas EF Core ni reglas de negocio.
```

Eso ayuda a que el codigo sea:

- Mas facil de leer.
- Mas facil de probar.
- Mas facil de mantener.
- Mas ordenado para proyectos grandes.

## 4. Estructura del Proyecto

```text
GuiaEfRazorAxios/
  Controllers/
    ApiProductosController.cs
    CategoriasController.cs
    HomeController.cs
    ProductosAxiosController.cs
    ProductosController.cs

  Data/
    AppDbContext.cs
    DatabaseSeeder.cs

  Dtos/
    CategoriaOptionDto.cs
    ProductoCreateDto.cs
    ProductoListDto.cs

  Interfaces/
    ICategoriaRepository.cs
    ICategoriaService.cs
    IProductoRepository.cs
    IProductoService.cs

  Models/
    Categoria.cs
    ErrorViewModel.cs
    Producto.cs

  Repositories/
    CategoriaRepository.cs
    ProductoRepository.cs

  Services/
    CategoriaService.cs
    OperationResult.cs
    ProductoService.cs

  Views/
    Categorias/
    Home/
    Productos/
    ProductosAxios/
    Shared/

  wwwroot/
    css/
    js/
    lib/

  Program.cs
  appsettings.json
  GuiaEfRazorAxios.csproj
```

## 5. Explicacion de Carpetas

### `Models`

Contiene las clases que representan las tablas principales.

`Categoria.cs` representa la tabla `Categorias`.

Campos importantes:

- `Id`: llave primaria.
- `Nombre`: nombre de la categoria.
- `Descripcion`: descripcion opcional.
- `Productos`: coleccion de productos relacionados.

`Producto.cs` representa la tabla `Productos`.

Campos importantes:

- `Id`: llave primaria.
- `Nombre`: nombre del producto.
- `Precio`: precio del producto.
- `Stock`: cantidad disponible.
- `CategoriaId`: llave foranea.
- `Categoria`: categoria relacionada.

### `Data`

Contiene la configuracion de base de datos.

`AppDbContext.cs`:

- Hereda de `DbContext`.
- Expone `DbSet<Categoria>`.
- Expone `DbSet<Producto>`.
- Configura la relacion uno-a-muchos.
- Carga datos iniciales con `HasData`.

Relacion configurada:

```csharp
modelBuilder.Entity<Categoria>()
    .HasMany(categoria => categoria.Productos)
    .WithOne(producto => producto.Categoria)
    .HasForeignKey(producto => producto.CategoriaId)
    .OnDelete(DeleteBehavior.Restrict);
```

`DatabaseSeeder.cs`:

- Crea un scope de servicios.
- Obtiene `AppDbContext`.
- Verifica o crea la base con `EnsureCreatedAsync`.
- Escribe logs antes y despues de la verificacion.

### `Interfaces`

Contiene contratos, no implementaciones.

Para que sirve:

- Permite depender de abstracciones.
- Separa lo que se hace de como se hace.
- Facilita pruebas unitarias.
- Mejora el orden del proyecto.

Ejemplo:

```csharp
public interface IProductoService
```

La interfaz dice que operaciones existen. La clase `ProductoService` implementa como se ejecutan.

### `Repositories`

Contiene el acceso a datos.

Responsabilidades:

- Usar `AppDbContext`.
- Hacer consultas con EF Core.
- Usar `Include` para cargar relaciones.
- Agregar, actualizar o eliminar entidades.
- Ejecutar `SaveChangesAsync`.

Ejemplo:

```csharp
return await _context.Productos
    .Include(producto => producto.Categoria)
    .OrderBy(producto => producto.Nombre)
    .ToListAsync();
```

### `Services`

Contiene la logica de negocio.

Responsabilidades:

- Validar reglas del sistema.
- Coordinar repositorios.
- Convertir entidades a DTOs.
- Devolver errores controlados con `OperationResult`.

Reglas implementadas:

- No eliminar una categoria si tiene productos.
- No crear un producto si la categoria no existe.
- Convertir productos a `ProductoListDto` para la API.

`OperationResult.cs`:

```csharp
OperationResult.Success()
OperationResult.Failure("Mensaje de error")
```

Sirve para devolver exito o error sin lanzar excepciones en casos normales de negocio.

### `Dtos`

DTO significa **Data Transfer Object**.

Sirven para no exponer directamente las entidades de EF Core en la API.

Archivos:

- `CategoriaOptionDto`: usado para llenar el select de categorias.
- `ProductoCreateDto`: datos que Axios envia al crear un producto.
- `ProductoListDto`: datos que la API devuelve para listar productos.

### `Controllers`

Los controladores son delgados.

Hacen esto:

- Reciben la solicitud.
- Registran logs.
- Validan `ModelState`.
- Llaman servicios.
- Devuelven vista, redireccion o JSON.

No hacen esto:

- No consultan directamente `AppDbContext`.
- No contienen reglas de negocio.
- No hacen transformaciones complejas.

Controladores principales:

- `CategoriasController`: CRUD basico de categorias con Razor.
- `ProductosController`: listado y creacion de productos con Razor.
- `ApiProductosController`: endpoints JSON para Axios.
- `ProductosAxiosController`: muestra la vista Axios desde `Views`.

### `Views`

Contiene las pantallas del sistema.

`Views/Categorias`:

- `Index.cshtml`: lista categorias.
- `Details.cshtml`: muestra productos de una categoria.
- `Create.cshtml`: crea categoria.
- `Edit.cshtml`: edita categoria.
- `Delete.cshtml`: confirma eliminacion.

`Views/Productos`:

- `Index.cshtml`: lista productos con su categoria.
- `Create.cshtml`: crea productos.

`Views/ProductosAxios/Index.cshtml`:

- Vista con HTML, JavaScript y Axios.
- Esta ubicada en `Views` porque se entrega mediante un controlador MVC.
- Consume la API `/api/productos`.
- Consume la API `/api/productos/categorias`.
- Envia JSON con `axios.post`.

### `wwwroot`

Contiene archivos estaticos.

En este proyecto se usa para:

- Bootstrap.
- jQuery.
- CSS general.
- JavaScript base de la plantilla.

La vista Axios no esta en `wwwroot`; esta en `Views/ProductosAxios/Index.cshtml`, como parte del flujo MVC.

## 6. Logging

El proyecto usa `Microsoft.Extensions.Logging`.

Configuracion en `Program.cs`:

```csharp
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
```

Para que sirve:

- `AddConsole`: muestra logs en la terminal.
- `AddDebug`: muestra logs en herramientas de depuracion.
- `ILogger<T>` permite saber que clase escribio el log.

Hay logs en:

- Controladores.
- Servicios.
- Repositorios.
- `DatabaseSeeder`.

Niveles usados:

- `LogInformation`: pasos normales.
- `LogWarning`: casos esperados pero importantes, como una categoria inexistente.

## 7. Flujo Razor

Ejemplo: abrir `/Categorias`.

```text
Navegador
  -> CategoriasController.Index
  -> ICategoriaService
  -> CategoriaService.GetCategoriasWithProductsAsync
  -> ICategoriaRepository
  -> CategoriaRepository.GetAllWithProductsAsync
  -> AppDbContext
  -> SQL Server
  -> Views/Categorias/Index.cshtml
  -> HTML al navegador
```

## 8. Flujo API con Axios

Ejemplo: abrir `/ProductosAxios`.

```text
Navegador
  -> ProductosAxiosController.Index
  -> Views/ProductosAxios/Index.cshtml
  -> JavaScript ejecuta axios.get("/api/productos")
  -> ApiProductosController.GetProductos
  -> IProductoService
  -> ProductoService.GetProductosForApiAsync
  -> IProductoRepository
  -> ProductoRepository.GetAllWithCategoryAsync
  -> AppDbContext
  -> SQL Server
  -> JSON al navegador
  -> JavaScript pinta la tabla
```

Conceptos importantes dentro de `Views/ProductosAxios/Index.cshtml`:

- `event.preventDefault()`: cancela el envio normal del formulario para evitar que el navegador recargue la pagina. Se usa porque el formulario se envia manualmente con `axios.post`.
- `axios.get(...)`: consulta datos JSON desde la API.
- `axios.post(...)`: envia un objeto JavaScript como JSON hacia la API.
- `Number(...)`: convierte valores de inputs/selects, que llegan como texto, a numeros compatibles con `int` o `decimal` en C#.
- `form.reset()`: limpia los campos del formulario despues de guardar correctamente. No borra datos de la base; solo reinicia los controles HTML.
- `innerHTML`: reemplaza el contenido HTML de un elemento. En el proyecto se usa para limpiar tablas y construir filas.
- `appendChild(...)`: agrega un elemento creado con JavaScript dentro de otro elemento HTML.
- `try/catch`: separa el flujo correcto del flujo de error cuando una solicitud HTTP falla.

## 9. Configuracion de Base de Datos

La cadena de conexion esta en `appsettings.json`:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=GuiaEfRazorAxiosDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Partes importantes:

- `Server=(localdb)\\MSSQLLocalDB`: usa SQL Server LocalDB.
- `Database=GuiaEfRazorAxiosDb`: nombre de la base.
- `Trusted_Connection=True`: usa autenticacion de Windows.
- `MultipleActiveResultSets=true`: permite multiples resultados activos.
- `TrustServerCertificate=True`: evita problemas de certificado en desarrollo local.

## 10. Program.cs

`Program.cs` es el punto de entrada.

Registra logging:

```csharp
builder.Logging.AddConsole();
builder.Logging.AddDebug();
```

Registra MVC:

```csharp
builder.Services.AddControllersWithViews();
```

Registra EF Core:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Registra repositorios:

```csharp
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
```

Registra servicios:

```csharp
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
```

`AddScoped` significa:

```text
Una instancia por solicitud HTTP.
```

## 11. Rutas Utiles

```text
/                         Inicio
/Categorias               Razor: categorias
/Categorias/Details/1     Razor: detalle de categoria con productos
/Productos                Razor: productos
/Productos/Create         Razor: crear producto
/ProductosAxios           Vista MVC con HTML, JavaScript y Axios
/api/productos            API JSON de productos
/api/productos/categorias API JSON de categorias
```

## 12. Como Ejecutar

Desde la carpeta del proyecto:

```powershell
dotnet restore
dotnet build
dotnet run
```

Luego abre la URL que muestre la consola.

Ejemplo comun:

```text
https://localhost:xxxx
```

## 13. Como Verificar que Compila

Ejecuta:

```powershell
dotnet build
```

Resultado esperado:

```text
Compilacion correcta.
0 Advertencia(s)
0 Errores
```

## 14. Ideas para Practicar

1. Agregar editar productos.
2. Agregar eliminar productos.
3. Agregar detalles de producto.
4. Cambiar `EnsureCreatedAsync` por migraciones.
5. Agregar busqueda por nombre.
6. Crear una tercera tabla `Proveedor`.
7. Agregar pruebas unitarias para `ProductoService`.
8. Agregar logs `LogError` con `try/catch` en operaciones criticas.
9. Crear paginacion para productos.
10. Agregar filtros por categoria en la vista Axios.

## 15. Resumen de Buenas Practicas Aplicadas

- Controladores delgados.
- Interfaces separadas en `Interfaces`.
- Repositorios para acceso a datos.
- Servicios para reglas de negocio.
- DTOs para la API.
- Logging en capas importantes.
- Relacion uno-a-muchos configurada en EF Core.
- Vista Axios ubicada en `Views`, servida por controlador MVC.
- Comentarios educativos en clases principales para entender cada parte del codigo.

## 16. Referencias para Estudiar Mas

Estas URLs sirven para que el estudiante investigue cada tema con documentacion oficial o ampliamente usada.

### ASP.NET Core MVC

- Documentacion oficial MVC: https://learn.microsoft.com/en-us/aspnet/core/mvc/overview
- Vistas en ASP.NET Core MVC: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview
- Sintaxis Razor: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor
- Layouts en Razor Views: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/layout

### Formularios, Tag Helpers y Validacion

- Formularios con Tag Helpers: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms
- Tag Helpers en ASP.NET Core: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro
- Tag Helpers incluidos: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in
- Validacion de modelos y `ModelState`: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation

### Entity Framework Core y SQL Server

- Entity Framework Core: https://learn.microsoft.com/en-us/ef/core/
- Relaciones en EF Core: https://learn.microsoft.com/en-us/ef/core/modeling/relationships
- SQL Server Express LocalDB: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

### Inyeccion de Dependencias, Servicios e Interfaces

- Dependency Injection en ASP.NET Core: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
- Principio de inversion de dependencias: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles#dependency-inversion

### Logging

- Logging en .NET y ASP.NET Core: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/
- API `ILogger`: https://learn.microsoft.com/en-us/dotnet/core/extensions/logging

### APIs, JavaScript y Axios

- Crear Web APIs con ASP.NET Core: https://learn.microsoft.com/en-us/aspnet/core/web-api/
- Axios: https://axios-http.com/docs/intro
- Requests con Axios: https://axios-http.com/docs/api_intro
- JavaScript async/await: https://developer.mozilla.org/en-US/docs/Learn/JavaScript/Asynchronous/Promises

### Bootstrap

- Bootstrap Forms: https://getbootstrap.com/docs/5.3/forms/overview/
- Bootstrap Tables: https://getbootstrap.com/docs/5.3/content/tables/
- Bootstrap Buttons: https://getbootstrap.com/docs/5.3/components/buttons/
