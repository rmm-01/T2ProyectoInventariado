# T3 — Sistema de Inventariado desacoplado (Cliente ↔ Servidor de BD)

## Qué cambió respecto de la T2
La T2 corría TODO en una sola máquina y guardaba los datos en memoria (`List<>`).
La T3 **desacopla la capa de datos**: ahora la información vive en una **base de datos
SQL Server** que puede estar en **otra máquina**, y la aplicación de escritorio se
conecta a ella por red/Internet (TCP). El resto del sistema (Entities, Interfaces,
Services, OCP, Forms) **no cambió**: solo se reescribieron los 3 repositorios.

## Cómo se hace la conexión
- Driver: **Microsoft.Data.SqlClient** (ADO.NET).
- La cadena de conexión está en **`appsettings.json`** (se copia junto al .exe).
- Cambiar de "una sola máquina" a "BD en otra máquina" = cambiar **solo** el `Server`
  de la cadena. No se recompila ni se toca el código.

## Requisitos
1. .NET 10 SDK (ya lo usabas en la T2).
2. Un SQL Server accesible. Opciones rápidas:
   - **SQL Server Express** (gratis) — acepta conexiones remotas por TCP 1433.
   - **SQL Server LocalDB** (para probar en una sola PC): `Server=(localdb)\MSSQLLocalDB`.

## Pasos para correr
1. Edita `appsettings.json` → `ConnectionStrings:InventarioDB`.
   - Una sola máquina (demo):
     `Server=localhost;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;`
   - BD en otra máquina (desacoplado):
     `Server=192.168.1.50,1433;Database=InventarioDB;User Id=appinventario;Password=...;TrustServerCertificate=True;Encrypt=False;`
2. `dotnet restore` (baja el paquete Microsoft.Data.SqlClient).
3. `dotnet run` (o F5 en Visual Studio).
4. En el **primer arranque** la app crea sola la base `InventarioDB`, las tablas y
   carga datos de ejemplo. En los siguientes arranques solo se conecta.

## Para demostrar el desacople en la sustentación
- Corre la app contra `localhost` → funciona.
- Cambia el `Server` por la IP de otra PC con SQL Server → mismo .exe, ahora los datos
  viven en otra máquina. Eso es el desacople pedido.

## Mapa de cambios (cada cambio = 1 Change Request / RFC)
- **RFC NORMAL**: Migración de persistencia en memoria → SQL Server
  (csproj + Program.cs + 3 repositorios + DatabaseInitializer + AppConfig + appsettings.json).
- **RFC ESTÁNDAR**: Cambio de la cadena de conexión para apuntar la BD a un servidor
  dedicado / nueva IP (cambio rutinario, pre-aprobado, solo `appsettings.json`).
- **RFC EMERGENCIA**: Restablecer servicio ante caída del servidor de BD
  (apuntar a servidor de respaldo / hotfix de conexión).
