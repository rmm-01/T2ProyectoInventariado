using Microsoft.Data.SqlClient;
using T3ProyectoInventariado.Entities;
using T3ProyectoInventariado.Interfaces;

namespace T3ProyectoInventariado.Repositories
{
    /// <summary>
    /// Acceso a OrdenCompra (con sus Detalles) contra SQL Server.
    /// La cabecera y el detalle se guardan en una transaccion para mantener consistencia.
    /// </summary>
    public class OrdenCompraRepository : IOrdenCompraRepository
    {
        private readonly string _cs;

        public OrdenCompraRepository(string connectionString) => _cs = connectionString;

        public List<OrdenCompra> ObtenerTodos()
        {
            var ordenes = new List<OrdenCompra>();
            using var conn = new SqlConnection(_cs);
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Fecha, ProveedorId, Estado FROM dbo.OrdenCompra ORDER BY Id;";
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    ordenes.Add(MapOrden(r));
            }

            // Cargar todos los detalles y agruparlos por orden (evita N consultas).
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, OrdenCompraId, ProductoId, Cantidad, PrecioUnitario FROM dbo.DetalleOrdenCompra ORDER BY Id;";
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    var det = MapDetalle(r);
                    ordenes.FirstOrDefault(o => o.Id == det.OrdenCompraId)?.Detalles.Add(det);
                }
            }

            return ordenes;
        }

        public OrdenCompra? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();

            OrdenCompra? orden = null;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Fecha, ProveedorId, Estado FROM dbo.OrdenCompra WHERE Id = @id;";
                cmd.Parameters.AddWithValue("@id", id);
                using var r = cmd.ExecuteReader();
                if (r.Read()) orden = MapOrden(r);
            }
            if (orden == null) return null;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, OrdenCompraId, ProductoId, Cantidad, PrecioUnitario FROM dbo.DetalleOrdenCompra WHERE OrdenCompraId = @id ORDER BY Id;";
                cmd.Parameters.AddWithValue("@id", id);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    orden.Detalles.Add(MapDetalle(r));
            }

            return orden;
        }

        public void Agregar(OrdenCompra orden)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO dbo.OrdenCompra (Fecha, ProveedorId, Estado)
                                    VALUES (@Fecha, @ProveedorId, @Estado);
                                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                cmd.Parameters.AddWithValue("@Fecha", orden.Fecha);
                cmd.Parameters.AddWithValue("@ProveedorId", orden.ProveedorId);
                cmd.Parameters.AddWithValue("@Estado", (int)orden.Estado);
                orden.Id = (int)cmd.ExecuteScalar()!;
            }

            InsertarDetalles(conn, tx, orden);
            tx.Commit();
        }

        public void Actualizar(OrdenCompra orden)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"UPDATE dbo.OrdenCompra SET
                                    Fecha=@Fecha, ProveedorId=@ProveedorId, Estado=@Estado
                                    WHERE Id=@Id;";
                cmd.Parameters.AddWithValue("@Fecha", orden.Fecha);
                cmd.Parameters.AddWithValue("@ProveedorId", orden.ProveedorId);
                cmd.Parameters.AddWithValue("@Estado", (int)orden.Estado);
                cmd.Parameters.AddWithValue("@Id", orden.Id);
                cmd.ExecuteNonQuery();
            }

            // Reemplazar los detalles (misma semantica que la version en memoria).
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = "DELETE FROM dbo.DetalleOrdenCompra WHERE OrdenCompraId=@Id;";
                cmd.Parameters.AddWithValue("@Id", orden.Id);
                cmd.ExecuteNonQuery();
            }

            InsertarDetalles(conn, tx, orden);
            tx.Commit();
        }

        private static void InsertarDetalles(SqlConnection conn, SqlTransaction tx, OrdenCompra orden)
        {
            foreach (var d in orden.Detalles)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO dbo.DetalleOrdenCompra (OrdenCompraId, ProductoId, Cantidad, PrecioUnitario)
                                    VALUES (@OrdenCompraId, @ProductoId, @Cantidad, @PrecioUnitario);
                                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                cmd.Parameters.AddWithValue("@OrdenCompraId", orden.Id);
                cmd.Parameters.AddWithValue("@ProductoId", d.ProductoId);
                cmd.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                cmd.Parameters.AddWithValue("@PrecioUnitario", d.PrecioUnitario);
                d.OrdenCompraId = orden.Id;
                d.Id = (int)cmd.ExecuteScalar()!;
            }
        }

        private static OrdenCompra MapOrden(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            Fecha = r.GetDateTime(1),
            ProveedorId = r.GetInt32(2),
            Estado = (EstadoOrden)r.GetInt32(3),
            Detalles = new List<DetalleOrdenCompra>()
        };

        private static DetalleOrdenCompra MapDetalle(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            OrdenCompraId = r.GetInt32(1),
            ProductoId = r.GetInt32(2),
            Cantidad = r.GetInt32(3),
            PrecioUnitario = r.GetDecimal(4)
        };
    }
}
