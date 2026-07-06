using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;
using T2ProyectoInventariado.OCP;
using T2ProyectoInventariado.Services;

namespace T2ProyectoInventariado.Forms
{
    public class FormOrdenesCompra : FormListadoBase
    {
        private readonly OrdenCompraService _service;
        private readonly IProductoRepository _productoRepo;
        private readonly IProveedorRepository _proveedorRepo;

        public FormOrdenesCompra(IOrdenCompraRepository ordenRepo, IProductoRepository productoRepo, IProveedorRepository proveedorRepo)
            : base("Órdenes de Compra", new Size(800, 500), "Nueva Orden", "Confirmar Recepción")
        {
            _productoRepo = productoRepo;
            _proveedorRepo = proveedorRepo;
            _service = new OrdenCompraService(ordenRepo, productoRepo, new StockService(new MovimientoEntrada()));
        }

        protected override object ObtenerFilas()
        {
            var ordenes = _service.ObtenerTodos();
            return ordenes.Select(o =>
            {
                var proveedor = _proveedorRepo.ObtenerPorId(o.ProveedorId);
                return new
                {
                    o.Id,
                    o.Fecha,
                    Proveedor = proveedor?.RazonSocial ?? "N/A",
                    CantidadItems = o.Detalles.Count,
                    Total = o.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario).ToString("C2"),
                    o.Estado
                };
            }).ToList();
        }

        protected override async Task OnBoton1Async()
        {
            var form = new FormOrdenCompraDetalle(_productoRepo, _proveedorRepo);
            if (form.ShowDialog() == DialogResult.OK && form.Orden != null)
            {
                await Task.Run(() => _service.Agregar(form.Orden));
                await RecargarAsync();
            }
        }

        protected override async Task OnBoton2Async()
        {
            var id = ObtenerIdSeleccionado();
            if (id == null) return;

            var orden = await Task.Run(() => _service.ObtenerPorId(id.Value));
            if (orden == null) return;

            if (orden.Estado == EstadoOrden.Recibida)
            {
                MessageBox.Show("Esta orden ya fue recibida.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                "¿Confirmar la recepción de esta orden?\nEl stock de los productos se actualizará automáticamente.",
                "Confirmar Recepción",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                await Task.Run(() => _service.ConfirmarRecepcion(id.Value));
                await RecargarAsync();
                MessageBox.Show("Orden recibida. Stock actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
