using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.Forms
{
    public class FormClienteDetalle : Form
    {
        public Cliente? Cliente { get; private set; }
        private readonly TextBox _txtNombre, _txtRuc, _txtTelefono, _txtCorreo, _txtDireccion;

        public FormClienteDetalle(Cliente? cliente)
        {
            Text = cliente == null ? "Nuevo Cliente" : "Editar Cliente";
            ClientSize = new Size(380, 320);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int y = 15;
            const int labelX = 15, inputX = 140, inputW = 210;
            void AddLabel(string text, int top) =>
                Controls.Add(new Label { Text = text, Location = new Point(labelX, top + 3), AutoSize = true });

            AddLabel("Nombre:", y);
            _txtNombre = new TextBox { Location = new Point(inputX, y), Width = inputW };
            Controls.Add(_txtNombre); y += 35;

            AddLabel("RUC/DNI:", y);
            _txtRuc = new TextBox { Location = new Point(inputX, y), Width = inputW };
            Controls.Add(_txtRuc); y += 35;

            AddLabel("Teléfono:", y);
            _txtTelefono = new TextBox { Location = new Point(inputX, y), Width = inputW };
            Controls.Add(_txtTelefono); y += 35;

            AddLabel("Correo:", y);
            _txtCorreo = new TextBox { Location = new Point(inputX, y), Width = inputW };
            Controls.Add(_txtCorreo); y += 35;

            AddLabel("Dirección:", y);
            _txtDireccion = new TextBox { Location = new Point(inputX, y), Width = inputW };
            Controls.Add(_txtDireccion); y += 45;

            var btnGuardar = new Button { Text = "Guardar", Location = new Point(90, y), Size = new Size(90, 35), DialogResult = DialogResult.OK };
            btnGuardar.Click += BtnGuardar_Click;
            Controls.Add(btnGuardar);

            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(190, y), Size = new Size(90, 35), DialogResult = DialogResult.Cancel };
            Controls.Add(btnCancelar);

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            if (cliente != null)
            {
                _txtNombre.Text = cliente.Nombre;
                _txtRuc.Text = cliente.RucODni;
                _txtTelefono.Text = cliente.Telefono;
                _txtCorreo.Text = cliente.Correo;
                _txtDireccion.Text = cliente.Direccion;
            }

            Cliente = cliente;
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtNombre.Text) || string.IsNullOrWhiteSpace(_txtRuc.Text))
            {
                MessageBox.Show("Nombre y RUC/DNI son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Cliente ??= new Cliente();
            Cliente.Nombre = _txtNombre.Text.Trim();
            Cliente.RucODni = _txtRuc.Text.Trim();
            Cliente.Telefono = _txtTelefono.Text.Trim();
            Cliente.Correo = _txtCorreo.Text.Trim();
            Cliente.Direccion = _txtDireccion.Text.Trim();
        }
    }
}