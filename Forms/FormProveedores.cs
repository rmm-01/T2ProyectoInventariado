using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;
using T2ProyectoInventariado.Services;

namespace T2ProyectoInventariado.Forms
{
    public class FormProveedores : Form
    {
        private readonly ProveedorService _service;
        private readonly DataGridView _dgv;

        public FormProveedores(IProveedorRepository repository)
        {
            _service = new ProveedorService(repository);

            Text = "Gestión de Proveedores";
            ClientSize = new Size(700, 450);
            StartPosition = FormStartPosition.CenterScreen;

            _dgv = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(670, 340),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var btnNuevo = new Button
            {
                Text = "Nuevo Proveedor",
                Location = new Point(10, 370),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10)
            };
            btnNuevo.Click += BtnNuevo_Click;

            var btnEditar = new Button
            {
                Text = "Editar Proveedor",
                Location = new Point(170, 370),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10)
            };
            btnEditar.Click += BtnEditar_Click;

            Controls.AddRange(new Control[] { _dgv, btnNuevo, btnEditar });
            CargarDatos();
        }

        private void CargarDatos()
        {
            _dgv.DataSource = null;
            _dgv.DataSource = _service.ObtenerTodos();
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            var form = new FormProveedorDetalle(null);
            if (form.ShowDialog() == DialogResult.OK && form.Proveedor != null)
            {
                _service.Agregar(form.Proveedor);
                CargarDatos();
            }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(_dgv.SelectedRows[0].Cells["Id"].Value);
            var proveedor = _service.ObtenerPorId(id);
            if (proveedor == null) return;

            var form = new FormProveedorDetalle(proveedor);
            if (form.ShowDialog() == DialogResult.OK && form.Proveedor != null)
            {
                _service.Actualizar(form.Proveedor);
                CargarDatos();
            }
        }
    }

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
