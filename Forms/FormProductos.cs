using T2ProyectoInventariado.Interfaces;
using T2ProyectoInventariado.Services;

namespace T2ProyectoInventariado.Forms
{
    public class FormProductos : FormListadoBase
    {
        private readonly ProductoService _service;

        public FormProductos(IProductoRepository repository)
            : base("Gestión de Productos", new Size(900, 500), "Nuevo Producto", "Editar Producto")
        {
            _service = new ProductoService(repository);
            CargarInicial();
        }

        protected override object ObtenerFilas()
        {
            var productos = _service.ObtenerTodos();
            return productos.Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Categoria,
                p.UnidadMedida,
                PrecioCompra = p.PrecioCompra.ToString("C2"),
                PrecioVenta = p.PrecioVenta.ToString("C2"),
                p.StockActual,
                p.StockMinimo,
                p.StockMaximo,
                Estado = _service.TieneStockBajo(p) ? "⚠ STOCK BAJO" : "OK"
            }).ToList();
        }

        protected override void OnDatosCargados()
        {
            foreach (DataGridViewRow row in Grid.Rows)
            {

                if (row.Cells["Estado"].Value?.ToString() == "⚠ STOCK BAJO")
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
            }
            int productosStockBajo = 0;

            foreach (DataGridViewRow row in Grid.Rows)
            {
                if (row.Cells["Estado"].Value?.ToString() == "⚠ STOCK BAJO")
                {
                    productosStockBajo++;
                }
            }

            if (productosStockBajo > 0)
            {
                MessageBox.Show(
                    $"Hay {productosStockBajo} productos con stock bajo.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        protected override async Task OnBoton1Async()
        {
            var form = new FormProductoDetalle(null);
            if (form.ShowDialog() == DialogResult.OK && form.Producto != null)
                try
                {
                    await Task.Run(() => _service.Agregar(form.Producto));
                    await RecargarAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
        }

        protected override async Task OnBoton2Async()
        {
            var id = ObtenerIdSeleccionado();
            if (id == null) return;

            var producto = await Task.Run(() => _service.ObtenerPorId(id.Value));
            if (producto == null) return;
            DialogResult respuesta = MessageBox.Show(
                "¿Desea editar este producto?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.No)
                return;
            var form = new FormProductoDetalle(producto);
            if (form.ShowDialog() == DialogResult.OK && form.Producto != null)
            {
                await Task.Run(() => _service.Actualizar(form.Producto));
                await RecargarAsync();
            }
        }
    }
}
