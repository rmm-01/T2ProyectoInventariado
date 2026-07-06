using Microsoft.Data.SqlClient;
using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Repositories
{
    /// <summary>
    /// Acceso a Proveedor contra SQL Server (antes era una List en memoria).
    /// </summary>
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly string _cs;

        public ProveedorRepository(string connectionString) => _cs = connectionString;

        public List<Proveedor> ObtenerTodos()
        {
            var lista = new List<Proveedor>();
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, RazonSocial, Pais, Contacto FROM dbo.Proveedor ORDER BY Id;";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(Map(r));
            return lista;
        }

        public Proveedor? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, RazonSocial, Pais, Contacto FROM dbo.Proveedor WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        public void Agregar(Proveedor proveedor)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO dbo.Proveedor (RazonSocial, Pais, Contacto)
                                VALUES (@RazonSocial, @Pais, @Contacto);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            Bind(cmd, proveedor);
            proveedor.Id = (int)cmd.ExecuteScalar()!;
        }

        public void Actualizar(Proveedor proveedor)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE dbo.Proveedor SET
                                RazonSocial=@RazonSocial, Pais=@Pais, Contacto=@Contacto
                                WHERE Id=@Id;";
            Bind(cmd, proveedor);
            cmd.Parameters.AddWithValue("@Id", proveedor.Id);
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conn = new SqlConnection(_cs);
            conn.Open();

            using (var check = conn.CreateCommand())
            {
                check.CommandText = "SELECT COUNT(1) FROM dbo.OrdenCompra WHERE ProveedorId = @id;";
                check.Parameters.AddWithValue("@id", id);
                var enUso = (int)check.ExecuteScalar()! > 0;
                if (enUso)
                    throw new InvalidOperationException("No se puede eliminar: el proveedor tiene órdenes de compra asociadas.");
            }

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM dbo.Proveedor WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private static void Bind(SqlCommand cmd, Proveedor p)
        {
            cmd.Parameters.AddWithValue("@RazonSocial", p.RazonSocial);
            cmd.Parameters.AddWithValue("@Pais", (object?)p.Pais ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Contacto", (object?)p.Contacto ?? DBNull.Value);
        }

        private static Proveedor Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            RazonSocial = r.GetString(1),
            Pais = r.IsDBNull(2) ? string.Empty : r.GetString(2),
            Contacto = r.IsDBNull(3) ? string.Empty : r.GetString(3)
        };
    }
}
