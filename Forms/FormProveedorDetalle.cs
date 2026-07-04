using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.Forms
{
    public class FormProveedorDetalle : Form
    {
        public Proveedor? Proveedor { get; private set; }

        private readonly TextBox _txtRazonSocial, _txtPais, _txtContacto;

        public FormProveedorDetalle(Proveedor? proveedor)
        {
            Text = proveedor != null ? "Editar Proveedor" : "Nuevo Proveedor";
            ClientSize = new Size(380, 220);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int y = 15;

            Controls.Add(new Label { Text = "Razón Social:", Location = new Point(15, y + 3), AutoSize = true });
            _txtRazonSocial = new TextBox { Location = new Point(130, y), Width = 220 };
            Controls.Add(_txtRazonSocial); y += 35;

            Controls.Add(new Label { Text = "País:", Location = new Point(15, y + 3), AutoSize = true });
            _txtPais = new TextBox { Location = new Point(130, y), Width = 220 };
            Controls.Add(_txtPais); y += 35;

            Controls.Add(new Label { Text = "Contacto:", Location = new Point(15, y + 3), AutoSize = true });
            _txtContacto = new TextBox { Location = new Point(130, y), Width = 220 };
            Controls.Add(_txtContacto); y += 45;

            var btnGuardar = new Button { Text = "Guardar", Location = new Point(130, y), Size = new Size(100, 35), DialogResult = DialogResult.OK };
            btnGuardar.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(_txtRazonSocial.Text))
                {
                    MessageBox.Show("La razón social es obligatoria.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }
                Proveedor ??= new Proveedor();
                Proveedor.RazonSocial = _txtRazonSocial.Text.Trim();
                Proveedor.Pais = _txtPais.Text.Trim();
                Proveedor.Contacto = _txtContacto.Text.Trim();
            };
            Controls.Add(btnGuardar);

            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(240, y), Size = new Size(100, 35), DialogResult = DialogResult.Cancel };
            Controls.Add(btnCancelar);

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            if (proveedor != null)
            {
                _txtRazonSocial.Text = proveedor.RazonSocial;
                _txtPais.Text = proveedor.Pais;
                _txtContacto.Text = proveedor.Contacto;
            }

            Proveedor = proveedor;
        }
    }
}
