# Sistema de Inventariado — Kusi Perú

Aplicación de escritorio (WinForms) para llevar el inventario de una importadora:
productos, proveedores y órdenes de compra. Cuando llega una orden de compra y se
confirma su recepción, el stock del producto se actualiza solo.

## Qué hace la app

- **Productos**: alta, edición y listado. Marca en rojo los productos con stock
  por debajo del mínimo definido (`StockMinimo`).
- **Proveedores**: alta, edición y listado (razón social, país, contacto).
- **Órdenes de Compra**: armás una orden eligiendo proveedor y agregando productos
  con cantidad y precio. Al "Confirmar Recepción" de una orden pendiente, el stock
  de cada producto de la orden se incrementa automáticamente.

Los datos viven en **SQL Server** (antes eran una lista en memoria que se perdía
al cerrar la app — ver `README_EJECUCION.md` para el detalle de esa migración).

## Cómo está armado por dentro

La idea central del proyecto es que cada capa dependa de **abstracciones**
(interfaces), no de implementaciones concretas, para poder cambiar piezas sin
romper el resto:

```
Forms (UI)  →  Services (reglas de negocio)  →  Interfaces  ←  Repositories (SQL Server)
```

- **Entities/**: clases de datos puras (`Producto`, `Proveedor`, `OrdenCompra`, `DetalleOrdenCompra`).
- **Interfaces/**: contratos (`IProductoRepository`, `IProveedorRepository`,
  `IOrdenCompraRepository`, `IStockService`). Los `Forms` y `Services` programan
  contra estas interfaces, nunca contra la clase SQL concreta.
- **Repositories/**: implementación real contra SQL Server (ADO.NET / `Microsoft.Data.SqlClient`).
  Si mañana quisieras guardar en otro motor de BD, solo se reemplaza esta carpeta.
- **Services/**: reglas de negocio (ej. `ProductoService.TieneStockBajo`,
  `OrdenCompraService.ConfirmarRecepcion`).
- **OCP/**: acá está el ejemplo más claro de **Open/Closed Principle**. Hay una
  clase abstracta `MovimientoBase` con dos implementaciones, `MovimientoEntrada`
  (suma stock) y `MovimientoSalida` (resta stock). Si mañana se necesita un
  `MovimientoAjuste` (por inventario físico), se agrega una clase nueva sin tocar
  ninguna existente.
- **Forms/**: la UI. `FormListadoBase` es una clase base que evita repetir código:
  las tres pantallas de listado (Productos, Proveedores, Órdenes) comparten la
  grilla, los dos botones de acción y la carga de datos en segundo plano — cada
  una solo define qué datos mostrar y qué hacer al presionar cada botón.
- **Data/**: `AppConfig` lee la cadena de conexión desde `appsettings.json` y
  `DatabaseInitializer` crea las tablas solas la primera vez que corre la app
  (no hace falta correr un script `.sql` a mano).

## Cosas para mencionar en la sustentación (SOLID)

- **S**: cada clase tiene un trabajo (repos = datos, services = reglas, forms = UI).
- **O**: `OCP/MovimientoBase` + subclases — agregar movimientos nuevos sin tocar código viejo.
- **L**: `MovimientoEntrada`/`MovimientoSalida` son intercambiables donde se espera un `MovimientoBase`.
- **I**: interfaces chicas y puntuales, ninguna obliga a implementar métodos que no se usan.
- **D**: `Services` y `Forms` reciben interfaces por constructor (inyección de
  dependencias manual, armada en `Program.cs`), no instancian `ProductoRepository`
  directo.

## Cómo levantarlo

### 1. Requisitos
- .NET 10 SDK.
- Un SQL Server accesible. La forma más simple para probar en tu propia PC es
  **SQL Server LocalDB** (viene con Visual Studio).

### 2. Configurar la conexión
Editá `appsettings.json`, clave `ConnectionStrings:InventarioDB`. Por defecto ya
apunta a LocalDB:

```json
"InventarioDB": "Server=(localdb)\\MSSQLLocalDB;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;"
```

Si la BD está en otra máquina, solo cambiás el `Server` (no se toca ni recompila código).

### 3. Correr
```
dotnet restore
dotnet build
dotnet run
```

En el primer arranque la app crea sola la base de datos, las tablas, y siembra
unos datos de ejemplo (para no arrancar con las pantallas vacías). Si algo falla
al conectar (SQL Server apagado, cadena mal escrita), la app muestra un mensaje
claro en vez de crashear.

### 4. Correr los tests
Hay un proyecto separado `T2ProyectoInventariado.Tests` (xUnit) con pruebas sobre
la lógica de movimiento de stock (no dependen de la base de datos):

```
dotnet test
```

## Estructura del repo (resumen rápido)

```
Entities/        clases de datos
Interfaces/       contratos que implementan Repositories y usan Services/Forms
Repositories/     acceso a SQL Server
Services/         reglas de negocio
OCP/              patrón Open/Closed (movimientos de stock)
Data/             configuración + creación de esquema de BD
Forms/            interfaz de usuario (WinForms)
T2ProyectoInventariado.Tests/   tests unitarios (xUnit)
```
