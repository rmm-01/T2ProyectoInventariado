using System.Drawing.Drawing2D;

namespace T2ProyectoInventariado.Forms
{
    /// <summary>
    /// Identidad visual compartida por todas las pantallas ("Kusi Perú"): paleta,
    /// tipografía y helpers de estilo para no repetir setup de controles en cada form.
    /// </summary>
    internal static class Theme
    {
        public static readonly Color Noche = ColorTranslator.FromHtml("#1F3B4D");
        public static readonly Color Adobe = ColorTranslator.FromHtml("#D9782D");
        public static readonly Color Jade = ColorTranslator.FromHtml("#3E7C74");
        public static readonly Color Lienzo = ColorTranslator.FromHtml("#F7F4EF");
        public static readonly Color Tinta = ColorTranslator.FromHtml("#24313A");
        public static readonly Color Alerta = ColorTranslator.FromHtml("#C0392B");
        public static readonly Color FilaAlterna = ColorTranslator.FromHtml("#ECE5D6");

        public static readonly Font FontTitulo = new("Segoe UI", 15F, FontStyle.Bold);
        public static readonly Font FontBoton = new("Segoe UI", 10F, FontStyle.Bold);

        /// <summary>Barra superior con título, un badge de estado a la derecha y un filo de acento debajo.</summary>
        public static Panel CrearHeader(string titulo, int ancho, int alto, out Label badge)
        {
            var header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(ancho, alto),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Noche
            };

            var lbl = new Label
            {
                Text = titulo,
                Font = FontTitulo,
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(16, 0),
                Size = new Size(ancho - 220, alto - 4)
            };

            badge = new Label
            {
                Text = string.Empty,
                Font = new Font("Segoe UI", 9F),
                ForeColor = ColorTranslator.FromHtml("#F1C48B"),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(ancho - 210, 0),
                Size = new Size(194, alto - 4),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var filo = new Panel
            {
                Location = new Point(0, alto - 4),
                Size = new Size(ancho, 4),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Adobe
            };

            header.Controls.Add(lbl);
            header.Controls.Add(badge);
            header.Controls.Add(filo);
            return header;
        }

        public static void EstilizarBotonPrimario(Button b) => EstilizarBoton(b, Adobe);

        public static void EstilizarBotonSecundario(Button b) => EstilizarBoton(b, Jade);

        public static void EstilizarBotonPeligro(Button b) => EstilizarBoton(b, Alerta);

        private static void EstilizarBoton(Button b, Color color)
        {
            b.Font = FontBoton;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = color;
            b.ForeColor = Color.White;
            b.Cursor = Cursors.Hand;
            Redondear(b, 10);
        }

        public static void Redondear(Control c, int radio)
        {
            var rect = new Rectangle(0, 0, c.Width, c.Height);
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radio, radio, 180, 90);
            path.AddArc(rect.Right - radio, rect.Y, radio, radio, 270, 90);
            path.AddArc(rect.Right - radio, rect.Bottom - radio, radio, radio, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radio, radio, radio, 90, 90);
            path.CloseFigure();
            c.Region = new Region(path);
        }

        public static void EstilizarGrid(DataGridView g)
        {
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor = ColorTranslator.FromHtml("#DDD5C4");
            g.RowHeadersVisible = false;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersHeight = 34;
            g.ColumnHeadersDefaultCellStyle.BackColor = Noche;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = FontBoton;
            g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            g.DefaultCellStyle.SelectionBackColor = Adobe;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            g.AlternatingRowsDefaultCellStyle.BackColor = FilaAlterna;
            g.RowTemplate.Height = 28;
        }
    }
}
