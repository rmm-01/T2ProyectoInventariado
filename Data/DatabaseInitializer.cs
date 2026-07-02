using Microsoft.Data.SqlClient;

namespace T3ProyectoInventariado.Data
{
    /// <summary>
    /// Prepara el servidor de base de datos para que la aplicacion corra sin instalacion manual:
    /// crea la base de datos y las tablas si no existen. Se conecta a la maquina indicada en la
    /// cadena de conexion (local o remota). Es idempotente: si ya existe, no hace nada.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Inicializar(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            string nombreBd = string.IsNullOrWhiteSpace(builder.InitialCatalog)
                ? "InventarioDB"
                : builder.InitialCatalog;

            // 1) Conectar a 'master' para crear la base de datos si no existe.
            var maestro = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
            using (var conn = new SqlConnection(maestro.ConnectionString))
            {
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText =
                    $"IF DB_ID(@nombre) IS NULL EXEC('CREATE DATABASE [{nombreBd}]');";
                cmd.Parameters.AddWithValue("@nombre", nombreBd);
                cmd.ExecuteNonQuery();
            }

            // 2) Conectar a la base ya creada y asegurar el esquema (tablas).
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
IF OBJECT_ID('dbo.Producto', 'U') IS NULL
CREATE TABLE dbo.Producto (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        NVARCHAR(200)  NOT NULL,
    Descripcion   NVARCHAR(500)  NULL,
    Categoria     NVARCHAR(100)  NULL,
    UnidadMedida  NVARCHAR(100)  NULL,
    PrecioCompra  DECIMAL(18,2)  NOT NULL DEFAULT 0,
    PrecioVenta   DECIMAL(18,2)  NOT NULL DEFAULT 0,
    StockActual   INT            NOT NULL DEFAULT 0,
    StockMinimo   INT            NOT NULL DEFAULT 0,
    StockMaximo   INT            NOT NULL DEFAULT 0
);

IF OBJECT_ID('dbo.Proveedor', 'U') IS NULL
CREATE TABLE dbo.Proveedor (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    RazonSocial NVARCHAR(200) NOT NULL,
    Pais        NVARCHAR(100) NULL,
    Contacto    NVARCHAR(200) NULL
);

IF OBJECT_ID('dbo.OrdenCompra', 'U') IS NULL
CREATE TABLE dbo.OrdenCompra (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Fecha       DATETIME2 NOT NULL,
    ProveedorId INT       NOT NULL,
    Estado      INT       NOT NULL DEFAULT 0
);

IF OBJECT_ID('dbo.DetalleOrdenCompra', 'U') IS NULL
CREATE TABLE dbo.DetalleOrdenCompra (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    OrdenCompraId  INT           NOT NULL,
    ProductoId     INT           NOT NULL,
    Cantidad       INT           NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL
);";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
