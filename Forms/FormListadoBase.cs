namespace T2ProyectoInventariado.Forms
{
    /// <summary>
    /// Base comun a las pantallas de listado (Productos, Proveedores, Ordenes de Compra):
    /// grilla + dos botones de accion, redimensionable, y carga de datos en background
    /// para no congelar la UI mientras se consulta SQL Server.
    /// </summary>
    public abstract class FormListadoBase : Form
    {
        protected readonly DataGridView Grid;

        protected FormListadoBase(string titulo, Size clientSize, string textoBoton1, string textoBoton2)
        {
            Text = titulo;
            ClientSize = clientSize;
            MinimumSize = new Size(500, 350) + (Size - ClientSize);
            StartPosition = FormStartPosition.CenterScreen;

            Grid = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(clientSize.Width - 20, clientSize.Height - 90),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var btn1 = new Button
            {
                Text = textoBoton1,
                Location = new Point(10, clientSize.Height - 60),
                Size = new Size(160, 40),
                Font = new Font("Segoe UI", 10),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btn1.Click += async (s, e) => await ConBloqueoDeUiAsync(OnBoton1Async);

            var btn2 = new Button
            {
                Text = textoBoton2,
                Location = new Point(180, clientSize.Height - 60),
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 10),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btn2.Click += async (s, e) => await ConBloqueoDeUiAsync(OnBoton2Async);

            Controls.AddRange(new Control[] { Grid, btn1, btn2 });
        }

        /// <summary>Se llama una vez los controles ya estan armados (constructor del form derivado).</summary>
        protected async void CargarInicial() => await RecargarAsync();

        protected async Task RecargarAsync()
        {
            await ConBloqueoDeUiAsync(async () =>
            {
                Grid.DataSource = await Task.Run(ObtenerFilas);
                OnDatosCargados();
            });
        }

        /// <summary>Hook opcional para ajustar la grilla (estilos de fila, etc.) luego de cargar datos.</summary>
        protected virtual void OnDatosCargados() { }

        /// <summary>Ejecuta una accion de larga duracion (I/O a BD) deshabilitando los controles y mostrando reloj de arena.</summary>
        protected async Task ConBloqueoDeUiAsync(Func<Task> accion)
        {
            Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                await accion();
            }
            finally
            {
                Cursor = Cursors.Default;
                Enabled = true;
            }
        }

        protected int? ObtenerIdSeleccionado()
        {
            if (Grid.SelectedRows.Count == 0) return null;
            return Convert.ToInt32(Grid.SelectedRows[0].Cells["Id"].Value);
        }

        /// <summary>Obtiene los datos a mostrar en la grilla (corre en background, no tocar controles aqui).</summary>
        protected abstract object ObtenerFilas();

        protected abstract Task OnBoton1Async();

        protected abstract Task OnBoton2Async();
    }
}
