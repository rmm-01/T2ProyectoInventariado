using System.Text.Json;

namespace T3ProyectoInventariado.Data
{
    /// <summary>
    /// Lee la configuracion de la aplicacion (cadena de conexion) desde appsettings.json.
    /// Asi el desacople es solo un cambio de configuracion: la "maquina de la base de datos"
    /// se define cambiando el Server de la cadena, sin tocar ni recompilar el codigo.
    /// </summary>
    public static class AppConfig
    {
        public static string GetConnectionString()
        {
            var ruta = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            if (!File.Exists(ruta))
                throw new FileNotFoundException(
                    "No se encontro appsettings.json junto al ejecutable. " +
                    "Verifique que el archivo se copio a la carpeta de salida.", ruta);

            using var doc = JsonDocument.Parse(File.ReadAllText(ruta));

            if (!doc.RootElement.TryGetProperty("ConnectionStrings", out var cs) ||
                !cs.TryGetProperty("InventarioDB", out var valor) ||
                string.IsNullOrWhiteSpace(valor.GetString()))
            {
                throw new InvalidOperationException(
                    "Falta la cadena de conexion 'ConnectionStrings:InventarioDB' en appsettings.json.");
            }

            return valor.GetString()!;
        }
    }
}
