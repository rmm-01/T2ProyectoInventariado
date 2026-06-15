using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;
using T2ProyectoInventariado.Services;

namespace T2ProyectoInventariado.Forms
{
    public class FormProductos : Form
    {
        private readonly ProductoService _service;
        private readonly DataGridView _dgv;

        public FormProductos(IProductoRepository repository)
        {
            _service = new ProductoService(repository);

            Text = "Gestión de Productos";
            ClientSize = new Size(900, 500);
            StartPosition = FormStartPosition.CenterScreen;

            _dgv = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(870, 380),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var btnNuevo = new Button
            {
                Text = "Nuevo Producto",
                Location = new Point(10, 410),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10)
            };
            btnNuevo.Click += BtnNuevo_Click;

            var btnEditar = new Button
            {
                Text = "Editar Producto",
                Location = new Point(170, 410),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10)
            };
            btnEditar.Click += BtnEditar_Click;

            Controls.AddRange(new Control[] { _dgv, btnNuevo, btnEditar });
            CargarDatos();
        }

        private void CargarDatos()
        {
            var productos = _service.ObtenerTodos();
            _dgv.DataSource = null;
            _dgv.DataSource = productos.Select(p => new
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

            foreach (DataGridViewRow row in _dgv.Rows)
            {
                if (row.Cells["Estado"].Value?.ToString() == "⚠ STOCK BAJO")
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
            }
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            var form = new FormProductoDetalle(null);
            if (form.ShowDialog() == DialogResult.OK && form.Producto != null)
            {
                _service.Agregar(form.Producto);
                CargarDatos();
            }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) return;
            var id = (int)_dgv.SelectedRows[0].Cells["Id"].Value;
            var producto = _service.ObtenerPorId(id);
            if (producto == null) return;

            var form = new FormProductoDetalle(producto);
            if (form.ShowDialog() == DialogResult.OK && form.Producto != null)
            {
                _service.Actualizar(form.Producto);
                CargarDatos();
            }
        }
    }

    public class FormProductoDetalle : Form
    {
        public Producto? Producto { get; private set; }

        private readonly TextBox _txtNombre, _txtDescripcion, _txtCategoria, _txtUnidad;
        private readonly NumericUpDown _numPrecioCompra, _numPrecioVenta, _numStock, _numStockMin, _numStockMax;
        private readonly bool _esEdicion;

        public FormProductoDetalle(Producto? producto)
        {
            _esEdicion = producto != null;
            Text = _esEdicion ? "Editar Producto" : "Nuevo Producto";
            ClientSize = new Size(400, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int y = 15;
            const int labelX = 15, inputX = 150, inputW = 220;

            void AddLabel(string text, int top) =>
                Controls.Add(new Label { Text = text, Location = new Point(labelX, top + 3), AutoSize = true });

            AddLabel("Nombre:", y);
            _txtNombre = new TextBox { Location = new Point(inputX, y), Width = inputW };
            Controls.Add(_txtNombre); y += 35;

            AddLabel("Descripción:", y);
            _txtDescripcion = new TextBox { Location = new Point(inputX, y), Width = inputW };
            Controls.Add(_txtDescripcion); y += 35;

            AddLabel("Categoría:", y);
            _txtCategoria = new TextBox { Location = new Point(inputX, y), Width = inputW };
            Controls.Add(_txtCategoria); y += 35;

            AddLabel("Unidad Medida:", y);
            _txtUnidad = new TextBox { Location = new Point(inputX, y), Width = inputW };
            Controls.Add(_txtUnidad); y += 35;

            AddLabel("Precio Compra:", y);
            _numPrecioCompra = new NumericUpDown { Location = new Point(inputX, y), Width = inputW, DecimalPlaces = 2, Maximum = 999999 };
            Controls.Add(_numPrecioCompra); y += 35;

            AddLabel("Precio Venta:", y);
            _numPrecioVenta = new NumericUpDown { Location = new Point(inputX, y), Width = inputW, DecimalPlaces = 2, Maximum = 999999 };
            Controls.Add(_numPrecioVenta); y += 35;

            AddLabel("Stock Actual:", y);
            _numStock = new NumericUpDown { Location = new Point(inputX, y), Width = inputW, Maximum = 999999 };
            Controls.Add(_numStock); y += 35;

            AddLabel("Stock Mínimo:", y);
            _numStockMin = new NumericUpDown { Location = new Point(inputX, y), Width = inputW, Maximum = 999999 };
            Controls.Add(_numStockMin); y += 35;

            AddLabel("Stock Máximo:", y);
            _numStockMax = new NumericUpDown { Location = new Point(inputX, y), Width = inputW, Maximum = 999999 };
            Controls.Add(_numStockMax); y += 45;

            var btnGuardar = new Button { Text = "Guardar", Location = new Point(150, y), Size = new Size(100, 35), DialogResult = DialogResult.OK };
            btnGuardar.Click += BtnGuardar_Click;
            Controls.Add(btnGuardar);

            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(260, y), Size = new Size(100, 35), DialogResult = DialogResult.Cancel };
            Controls.Add(btnCancelar);

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            if (producto != null)
            {
                _txtNombre.Text = producto.Nombre;
                _txtDescripcion.Text = producto.Descripcion;
                _txtCategoria.Text = producto.Categoria;
                _txtUnidad.Text = producto.UnidadMedida;
                _numPrecioCompra.Value = producto.PrecioCompra;
                _numPrecioVenta.Value = producto.PrecioVenta;
                _numStock.Value = producto.StockActual;
                _numStockMin.Value = producto.StockMinimo;
                _numStockMax.Value = producto.StockMaximo;
            }

            Producto = producto;
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Producto ??= new Producto();
            Producto.Nombre = _txtNombre.Text.Trim();
            Producto.Descripcion = _txtDescripcion.Text.Trim();
            Producto.Categoria = _txtCategoria.Text.Trim();
            Producto.UnidadMedida = _txtUnidad.Text.Trim();
            Producto.PrecioCompra = _numPrecioCompra.Value;
            Producto.PrecioVenta = _numPrecioVenta.Value;
            Producto.StockActual = (int)_numStock.Value;
            Producto.StockMinimo = (int)_numStockMin.Value;
            Producto.StockMaximo = (int)_numStockMax.Value;
        }
    }
}
