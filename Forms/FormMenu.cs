using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Forms
{
    public class FormMenu : Form
    {
        private readonly IProductoRepository _productoRepo;
        private readonly IProveedorRepository _proveedorRepo;
        private readonly IOrdenCompraRepository _ordenRepo;
        private readonly IClienteRepository _clienteRepo;

        public FormMenu(IProductoRepository productoRepo, IProveedorRepository proveedorRepo, IOrdenCompraRepository ordenRepo, IClienteRepository clienteRepo)
        {
            _productoRepo = productoRepo;
            _proveedorRepo = proveedorRepo;
            _ordenRepo = ordenRepo;
            _clienteRepo = clienteRepo;

            Text = "Kusi Perú - Sistema de Inventariado";
            ClientSize = new Size(500, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Theme.Lienzo;

            var hero = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(500, 150),
                BackColor = Theme.Noche
            };

            var lblTitulo = new Label
            {
                Text = "IMPORTADORA KUSI PERÚ",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(60, 45)
            };

            var lblSubtitulo = new Label
            {
                Text = "Sistema de Inventariado",
                Font = new Font("Segoe UI", 11),
                ForeColor = ColorTranslator.FromHtml("#F1C48B"),
                AutoSize = true,
                Location = new Point(64, 90)
            };

            var filo = new Panel
            {
                Location = new Point(0, 146),
                Size = new Size(500, 4),
                BackColor = Theme.Adobe
            };

            hero.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo, filo });

            var btnProductos = new Button
            {
                Text = "Gestión de Productos",
                Size = new Size(300, 50),
                Location = new Point(100, 190),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Theme.EstilizarBotonPrimario(btnProductos);
            btnProductos.Click += (s, e) =>
            {
                var form = new FormProductos(_productoRepo);
                form.ShowDialog();
            };

            var btnProveedores = new Button
            {
                Text = "Gestión de Proveedores",
                Size = new Size(300, 50),
                Location = new Point(100, 260),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Theme.EstilizarBotonSecundario(btnProveedores);
            btnProveedores.Click += (s, e) =>
            {
                var form = new FormProveedores(_proveedorRepo);
                form.ShowDialog();
            };

            var btnOrdenes = new Button
            {
                Text = "Órdenes de Compra",
                Size = new Size(300, 50),
                Location = new Point(100, 330),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Theme.EstilizarBotonSecundario(btnOrdenes);
            btnOrdenes.Click += (s, e) =>
            {
                var form = new FormOrdenesCompra(_ordenRepo, _productoRepo, _proveedorRepo);
                form.ShowDialog();
            };

            Controls.AddRange(new Control[] { btnProductos, btnProveedores, btnOrdenes, hero });
            var btnClientes = new Button
            {
                Text = "Gestión de Clientes",
                Size = new Size(300, 50),
                Location = new Point(100, 340),
                Font = new Font("Segoe UI", 11)
            };
            btnClientes.Click += (s, e) =>
            {
                var form = new FormClientes(_clienteRepo);
                form.ShowDialog();
            };

            Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo, btnProductos, btnProveedores, btnOrdenes, btnClientes });
        }
    }
}
