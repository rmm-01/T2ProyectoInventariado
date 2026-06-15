using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Forms
{
    public class FormMenu : Form
    {
        private readonly IProductoRepository _productoRepo;
        private readonly IProveedorRepository _proveedorRepo;
        private readonly IOrdenCompraRepository _ordenRepo;

        public FormMenu(IProductoRepository productoRepo, IProveedorRepository proveedorRepo, IOrdenCompraRepository ordenRepo)
        {
            _productoRepo = productoRepo;
            _proveedorRepo = proveedorRepo;
            _ordenRepo = ordenRepo;

            Text = "Kusi Perú - Sistema de Inventariado";
            ClientSize = new Size(500, 400);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            var lblTitulo = new Label
            {
                Text = "IMPORTADORA KUSI PERÚ",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(80, 30)
            };

            var lblSubtitulo = new Label
            {
                Text = "Sistema de Inventariado",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(150, 70)
            };

            var btnProductos = new Button
            {
                Text = "Gestión de Productos",
                Size = new Size(300, 50),
                Location = new Point(100, 130),
                Font = new Font("Segoe UI", 11)
            };
            btnProductos.Click += (s, e) =>
            {
                var form = new FormProductos(_productoRepo);
                form.ShowDialog();
            };

            var btnProveedores = new Button
            {
                Text = "Gestión de Proveedores",
                Size = new Size(300, 50),
                Location = new Point(100, 200),
                Font = new Font("Segoe UI", 11)
            };
            btnProveedores.Click += (s, e) =>
            {
                var form = new FormProveedores(_proveedorRepo);
                form.ShowDialog();
            };

            var btnOrdenes = new Button
            {
                Text = "Órdenes de Compra",
                Size = new Size(300, 50),
                Location = new Point(100, 270),
                Font = new Font("Segoe UI", 11)
            };
            btnOrdenes.Click += (s, e) =>
            {
                var form = new FormOrdenesCompra(_ordenRepo, _productoRepo, _proveedorRepo);
                form.ShowDialog();
            };

            Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo, btnProductos, btnProveedores, btnOrdenes });
        }
    }
}
