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

            var productoRepo = new ProductoRepository();
            var proveedorRepo = new ProveedorRepository();
            var ordenRepo = new OrdenCompraRepository();
            var clienteRepo = new ClienteRepository();

            DatosIniciales.Cargar(productoRepo, proveedorRepo, ordenRepo);

            Application.Run(new FormMenu(productoRepo, proveedorRepo, ordenRepo, clienteRepo));
        }
    }
}
