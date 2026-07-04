using T2ProyectoInventariado.Interfaces;
using T2ProyectoInventariado.Services;

namespace T2ProyectoInventariado.Forms
{
    public class FormProveedores : FormListadoBase
    {
        private readonly ProveedorService _service;

        public FormProveedores(IProveedorRepository repository)
            : base("Gestión de Proveedores", new Size(700, 450), "Nuevo Proveedor", "Editar Proveedor")
        {
            _service = new ProveedorService(repository);
            CargarInicial();
        }

        protected override object ObtenerFilas() => _service.ObtenerTodos();

        protected override async Task OnBoton1Async()
        {
            var form = new FormProveedorDetalle(null);
            if (form.ShowDialog() == DialogResult.OK && form.Proveedor != null)
            {
                await Task.Run(() => _service.Agregar(form.Proveedor));
                await RecargarAsync();
            }
        }

        protected override async Task OnBoton2Async()
        {
            var id = ObtenerIdSeleccionado();
            if (id == null) return;

            var proveedor = await Task.Run(() => _service.ObtenerPorId(id.Value));
            if (proveedor == null) return;

            var form = new FormProveedorDetalle(proveedor);
            if (form.ShowDialog() == DialogResult.OK && form.Proveedor != null)
            {
                await Task.Run(() => _service.Actualizar(form.Proveedor));
                await RecargarAsync();
            }
        }
    }
}
