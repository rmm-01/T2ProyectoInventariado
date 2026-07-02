using Microsoft.Data.SqlClient;
using T3ProyectoInventariado.Data;
using T3ProyectoInventariado.Forms;
using T3ProyectoInventariado.Repositories;

namespace T3ProyectoInventariado
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1) Leer la cadena de conexion (define en QUE maquina vive la base de datos).
            string cs;
            try
            {
                cs = AppConfig.GetConnectionString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de configuracion",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2) Conectar al servidor de BD y preparar el esquema (idempotente).
            try
            {
                DatabaseInitializer.Inicializar(cs);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "No se pudo conectar al servidor de base de datos.\n\n" +
                    "Detalle: " + ex.Message + "\n\n" +
                    "Revise la cadena de conexion en appsettings.json (Server / usuario / clave) " +
                    "y que el servidor SQL acepte conexiones remotas.",
                    "Error de conexion a la base de datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3) Inyectar los repositorios SQL (mismas interfaces que la version en memoria).
            var productoRepo = new ProductoRepository(cs);
            var proveedorRepo = new ProveedorRepository(cs);
            var ordenRepo = new OrdenCompraRepository(cs);

            // 4) Sembrar datos de ejemplo solo si la base esta vacia (primera ejecucion).
            if (productoRepo.ObtenerTodos().Count == 0)
                DatosIniciales.Cargar(productoRepo, proveedorRepo, ordenRepo);

            Application.Run(new FormMenu(productoRepo, proveedorRepo, ordenRepo));
        }
    }
}
