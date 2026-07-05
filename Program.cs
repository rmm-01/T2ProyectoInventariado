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

            // Manejo de errores no controlados
            Application.ThreadException += (_, e) =>
                MostrarErrorNoManejado(e.Exception);

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                MostrarErrorNoManejado(
                    e.ExceptionObject as Exception ??
                    new Exception("Error desconocido.")
                );

            // 1) Leer cadena de conexión
            string cs;

            try
            {
                cs = AppConfig.GetConnectionString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error de configuración",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // 2) Inicializar base de datos
            try
            {
                DatabaseInitializer.Inicializar(cs);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "No se pudo conectar al servidor de base de datos.\n\n" +
                    "Detalle: " + ex.Message + "\n\n" +
                    "Revise la cadena de conexión en appsettings.json.",
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            // 3) Crear repositorios
            var productoRepo = new ProductoRepository(cs);
            var proveedorRepo = new ProveedorRepository(cs);
            var ordenRepo = new OrdenCompraRepository(cs);

            // ClienteRepository está implementado sin conexión SQL
            var clienteRepo = new ClienteRepository();


            // 4) Cargar datos iniciales si no existen productos
            if (productoRepo.ObtenerTodos().Count == 0)
            {
                DatosIniciales.Cargar(
                    productoRepo,
                    proveedorRepo,
                    ordenRepo
                );
            }


            // 5) Abrir menú principal
            Application.Run(
                new FormMenu(
                    productoRepo,
                    proveedorRepo,
                    ordenRepo,
                    clienteRepo
                )
            );
        }


        private static void MostrarErrorNoManejado(Exception ex)
        {
            var mensaje = ex is SqlException
                ? "Se perdió la conexión con la base de datos.\n\nDetalle: "
                    + ex.Message
                : "Ocurrió un error inesperado.\n\nDetalle: "
                    + ex.Message;


            MessageBox.Show(
                mensaje,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}