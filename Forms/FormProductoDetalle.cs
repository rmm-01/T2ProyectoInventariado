using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.Forms
{
    public class FormProductoDetalle : Form
    {
        public Producto? Producto { get; private set; }

        private readonly TextBox _txtNombre, _txtDescripcion, _txtCategoria, _txtUnidad;
        private readonly NumericUpDown _numPrecioCompra, _numPrecioVenta, _numStock, _numStockMin, _numStockMax;

        public FormProductoDetalle(Producto? producto)
        {
            Text = producto != null ? "Editar Producto" : "Nuevo Producto";
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
            if (!Validar(out var mensaje))
            {
                MessageBox.Show(mensaje, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private bool Validar(out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(_txtNombre.Text))
            {
                mensaje = "El nombre es obligatorio.";
                return false;
            }

            if (_numStockMin.Value > _numStockMax.Value)
            {
                mensaje = "El stock mínimo no puede ser mayor que el stock máximo.";
                return false;
            }

            if (_numPrecioVenta.Value < _numPrecioCompra.Value)
            {
                mensaje = "El precio de venta no puede ser menor que el precio de compra.";
                return false;
            }

            mensaje = string.Empty;
            return true;
        }
    }
}
