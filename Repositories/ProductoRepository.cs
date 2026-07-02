using Microsoft.Data.SqlClient;
using T3ProyectoInventariado.Entities;
using T3ProyectoInventariado.Interfaces;

namespace T3ProyectoInventariado.Repositories
{
    /// <summary>
    /// Acceso a Producto contra SQL Server (antes era una List en memoria).
    /// Cada operacion abre una conexion al servidor de BD usando la cadena inyectada;
    /// el pool de conexiones de ADO.NET reutiliza las conexiones de forma transparente.
    /// </summary>
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _cs;

        public ProductoRepository(string connectionString) => _cs = connectionString;

        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, Nombre, Descripcion, Categoria, UnidadMedida,
                                       PrecioCompra, PrecioVenta, StockActual, StockMinimo, StockMaximo
                                FROM dbo.Producto ORDER BY Id;";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(Map(r));
            return lista;
        }

        public Producto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, Nombre, Descripcion, Categoria, UnidadMedida,
                                       PrecioCompra, PrecioVenta, StockActual, StockMinimo, StockMaximo
                                FROM dbo.Producto WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        public void Agregar(Producto producto)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO dbo.Producto
                    (Nombre, Descripcion, Categoria, UnidadMedida, PrecioCompra, PrecioVenta, StockActual, StockMinimo, StockMaximo)
                    VALUES (@Nombre, @Descripcion, @Categoria, @UnidadMedida, @PrecioCompra, @PrecioVenta, @StockActual, @StockMinimo, @StockMaximo);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
            Bind(cmd, producto);
            producto.Id = (int)cmd.ExecuteScalar()!;
        }

        public void Actualizar(Producto producto)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE dbo.Producto SET
                    Nombre=@Nombre, Descripcion=@Descripcion, Categoria=@Categoria, UnidadMedida=@UnidadMedida,
                    PrecioCompra=@PrecioCompra, PrecioVenta=@PrecioVenta,
                    StockActual=@StockActual, StockMinimo=@StockMinimo, StockMaximo=@StockMaximo
                    WHERE Id=@Id;";
            Bind(cmd, producto);
            cmd.Parameters.AddWithValue("@Id", producto.Id);
            cmd.ExecuteNonQuery();
        }

        private static void Bind(SqlCommand cmd, Producto p)
        {
            cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)p.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Categoria", (object?)p.Categoria ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UnidadMedida", (object?)p.UnidadMedida ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PrecioCompra", p.PrecioCompra);
            cmd.Parameters.AddWithValue("@PrecioVenta", p.PrecioVenta);
            cmd.Parameters.AddWithValue("@StockActual", p.StockActual);
            cmd.Parameters.AddWithValue("@StockMinimo", p.StockMinimo);
            cmd.Parameters.AddWithValue("@StockMaximo", p.StockMaximo);
        }

        private static Producto Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            Nombre = r.GetString(1),
            Descripcion = r.IsDBNull(2) ? string.Empty : r.GetString(2),
            Categoria = r.IsDBNull(3) ? string.Empty : r.GetString(3),
            UnidadMedida = r.IsDBNull(4) ? string.Empty : r.GetString(4),
            PrecioCompra = r.GetDecimal(5),
            PrecioVenta = r.GetDecimal(6),
            StockActual = r.GetInt32(7),
            StockMinimo = r.GetInt32(8),
            StockMaximo = r.GetInt32(9)
        };
    }
}
