using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Forms
{
    public class FormOrdenCompraDetalle : Form
    {
        public OrdenCompra? Orden { get; private set; }

        private readonly ComboBox _cmbProveedor;
        private readonly DataGridView _dgvDetalles;
        private readonly ComboBox _cmbProducto;
        private readonly NumericUpDown _numCantidad, _numPrecio;
        private readonly List<DetalleOrdenCompra> _detalles = new();
        private readonly IProductoRepository _productoRepo;

        public FormOrdenCompraDetalle(IProductoRepository productoRepo, IProveedorRepository proveedorRepo)
        {
            _productoRepo = productoRepo;

            Text = "Nueva Orden de Compra";
            ClientSize = new Size(600, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Controls.Add(new Label { Text = "Proveedor:", Location = new Point(15, 18), AutoSize = true });
            _cmbProveedor = new ComboBox
            {
                Location = new Point(100, 15),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            var proveedores = proveedorRepo.ObtenerTodos();
            _cmbProveedor.DataSource = proveedores;
            _cmbProveedor.DisplayMember = "RazonSocial";
            _cmbProveedor.ValueMember = "Id";
            Controls.Add(_cmbProveedor);

            var grpDetalle = new GroupBox
            {
                Text = "Agregar Producto",
                Location = new Point(10, 55),
                Size = new Size(575, 80)
            };

            grpDetalle.Controls.Add(new Label { Text = "Producto:", Location = new Point(10, 28), AutoSize = true });
            _cmbProducto = new ComboBox
            {
                Location = new Point(75, 25),
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            var productos = productoRepo.ObtenerTodos();
            _cmbProducto.DataSource = productos;
            _cmbProducto.DisplayMember = "Nombre";
            _cmbProducto.ValueMember = "Id";
            grpDetalle.Controls.Add(_cmbProducto);

            grpDetalle.Controls.Add(new Label { Text = "Cant:", Location = new Point(265, 28), AutoSize = true });
            _numCantidad = new NumericUpDown { Location = new Point(300, 25), Width = 60, Minimum = 1, Maximum = 99999, Value = 1 };
            grpDetalle.Controls.Add(_numCantidad);

            grpDetalle.Controls.Add(new Label { Text = "Precio:", Location = new Point(370, 28), AutoSize = true });
            _numPrecio = new NumericUpDown { Location = new Point(415, 25), Width = 70, DecimalPlaces = 2, Maximum = 999999 };
            grpDetalle.Controls.Add(_numPrecio);

            var btnAgregar = new Button { Text = "+", Location = new Point(495, 23), Size = new Size(35, 28) };
            btnAgregar.Click += BtnAgregar_Click;
            grpDetalle.Controls.Add(btnAgregar);

            Controls.Add(grpDetalle);

            _cmbProducto.SelectedIndexChanged += (s, e) =>
            {
                if (_cmbProducto.SelectedItem is Producto p)
                    _numPrecio.Value = p.PrecioCompra;
            };
            if (productos.Count > 0) _numPrecio.Value = productos[0].PrecioCompra;

            _dgvDetalles = new DataGridView
            {
                Location = new Point(10, 145),
                Size = new Size(575, 280),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Controls.Add(_dgvDetalles);

            var btnGuardar = new Button
            {
                Text = "Guardar Orden",
                Location = new Point(350, 440),
                Size = new Size(120, 35),
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnGuardar.Click += BtnGuardar_Click;
            Controls.Add(btnGuardar);

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(480, 440),
                Size = new Size(100, 35),
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            Controls.Add(btnCancelar);

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            RefrescarDetalles();
        }

        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            if (_cmbProducto.SelectedItem is not Producto producto) return;

            if (_numPrecio.Value <= 0)
            {
                MessageBox.Show("El precio unitario debe ser mayor que cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _detalles.Add(new DetalleOrdenCompra
            {
                ProductoId = producto.Id,
                Cantidad = (int)_numCantidad.Value,
                PrecioUnitario = _numPrecio.Value
            });
            RefrescarDetalles();
        }

        private void RefrescarDetalles()
        {
            _dgvDetalles.DataSource = null;
            _dgvDetalles.DataSource = _detalles.Select(d =>
            {
                var prod = _productoRepo.ObtenerPorId(d.ProductoId);
                return new
                {
                    Producto = prod?.Nombre ?? "N/A",
                    d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario.ToString("C2"),
                    Subtotal = (d.Cantidad * d.PrecioUnitario).ToString("C2")
                };
            }).ToList();
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (_detalles.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            if (_cmbProveedor.SelectedItem is not Proveedor proveedor)
            {
                MessageBox.Show("Seleccione un proveedor.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Orden = new OrdenCompra
            {
                Fecha = DateTime.Now,
                ProveedorId = proveedor.Id,
                Detalles = new List<DetalleOrdenCompra>(_detalles),
                Estado = EstadoOrden.Pendiente
            };
        }
    }
}
