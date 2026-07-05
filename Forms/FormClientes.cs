using T2ProyectoInventariado.Interfaces;
using T2ProyectoInventariado.Services;

namespace T2ProyectoInventariado.Forms
{
    public class FormClientes : Form
    {
        private readonly ClienteService _service;
        private readonly DataGridView _dgv;

        public FormClientes(IClienteRepository repository)
        {
            _service = new ClienteService(repository);

            Text = "Gestión de Clientes";
            ClientSize = new Size(800, 480);
            StartPosition = FormStartPosition.CenterScreen;

            _dgv = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(770, 360),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var btnNuevo = new Button { Text = "Nuevo Cliente", Location = new Point(10, 390), Size = new Size(150, 40), Font = new Font("Segoe UI", 10) };
            btnNuevo.Click += BtnNuevo_Click;

            var btnEditar = new Button { Text = "Editar Cliente", Location = new Point(170, 390), Size = new Size(150, 40), Font = new Font("Segoe UI", 10) };
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
            var form = new FormClienteDetalle(null);
            if (form.ShowDialog() == DialogResult.OK && form.Cliente != null)
            {
                _service.Agregar(form.Cliente);
                CargarDatos();
            }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) return;
            var id = (int)_dgv.SelectedRows[0].Cells["Id"].Value;
            var cliente = _service.ObtenerPorId(id);
            if (cliente == null) return;

            var form = new FormClienteDetalle(cliente);
            if (form.ShowDialog() == DialogResult.OK && form.Cliente != null)
            {
                _service.Actualizar(form.Cliente);
                CargarDatos();
            }
        }
    }
}