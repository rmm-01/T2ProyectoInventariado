using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;
using T2ProyectoInventariado.Services;

namespace T2ProyectoInventariado.Forms
{
    public class FormOrdenesCompra : Form
    {
        private readonly OrdenCompraService _service;
        private readonly IProductoRepository _productoRepo;
        private readonly IProveedorRepository _proveedorRepo;
        private readonly DataGridView _dgv;

        public FormOrdenesCompra(IOrdenCompraRepository ordenRepo, IProductoRepository productoRepo, IProveedorRepository proveedorRepo)
        {
            _productoRepo = productoRepo;
            _proveedorRepo = proveedorRepo;
            _service = new OrdenCompraService(ordenRepo, productoRepo);

            Text = "Órdenes de Compra";
            ClientSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterScreen;

            _dgv = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(770, 380),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var btnNueva = new Button
            {
                Text = "Nueva Orden",
                Location = new Point(10, 410),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10)
            };
            btnNueva.Click += BtnNueva_Click;

            var btnConfirmar = new Button
            {
                Text = "Confirmar Recepción",
                Location = new Point(170, 410),
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 10)
            };
            btnConfirmar.Click += BtnConfirmar_Click;

            Controls.AddRange(new Control[] { _dgv, btnNueva, btnConfirmar });
            CargarDatos();
        }

        private void CargarDatos()
        {
            var ordenes = _service.ObtenerTodos();
            _dgv.DataSource = null;
            _dgv.DataSource = ordenes.Select(o =>
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

        private void BtnNueva_Click(object? sender, EventArgs e)
        {
            var form = new FormOrdenCompraDetalle(_productoRepo, _proveedorRepo);
            if (form.ShowDialog() == DialogResult.OK && form.Orden != null)
            {
                _service.Agregar(form.Orden);
                CargarDatos();
            }
        }

        private void BtnConfirmar_Click(object? sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(_dgv.SelectedRows[0].Cells["Id"].Value);
            var orden = _service.ObtenerPorId(id);
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
                _service.ConfirmarRecepcion(id);
                CargarDatos();
                MessageBox.Show("Orden recibida. Stock actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

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
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Controls.Add(_dgvDetalles);

            var btnGuardar = new Button { Text = "Guardar Orden", Location = new Point(350, 440), Size = new Size(120, 35), DialogResult = DialogResult.OK };
            btnGuardar.Click += BtnGuardar_Click;
            Controls.Add(btnGuardar);

            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(480, 440), Size = new Size(100, 35), DialogResult = DialogResult.Cancel };
            Controls.Add(btnCancelar);

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            RefrescarDetalles();
        }

        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            if (_cmbProducto.SelectedItem is not Producto producto) return;

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
