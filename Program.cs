using Microsoft.Data.SqlClient;
using T2ProyectoInventariado.Data;
using T2ProyectoInventariado.Forms;
using T2ProyectoInventariado.Repositories;

namespace T2ProyectoInventariado
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Red de seguridad para errores de BD que ocurren despues del arranque
            // (ej. se cae el servidor SQL mientras la app ya esta abierta): en vez de
            // crashear, se muestra un mensaje y la app sigue viva.
            Application.ThreadException += (_, e) => MostrarErrorNoManejado(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                MostrarErrorNoManejado(e.ExceptionObject as Exception ?? new Exception("Error desconocido."));

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

        private static void MostrarErrorNoManejado(Exception ex)
        {
            var mensaje = ex is SqlException
                ? "Se perdio la conexion con la base de datos.\n\nDetalle: " + ex.Message
                : "Ocurrio un error inesperado.\n\nDetalle: " + ex.Message;

            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
