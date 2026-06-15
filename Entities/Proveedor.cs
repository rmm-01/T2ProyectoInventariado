namespace T2ProyectoInventariado.Entities
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Contacto { get; set; } = string.Empty;
    }
}
