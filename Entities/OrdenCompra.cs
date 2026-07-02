namespace T3ProyectoInventariado.Entities
{
    public enum EstadoOrden
    {
        Pendiente,
        Recibida
    }

    public class OrdenCompra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ProveedorId { get; set; }
        public List<DetalleOrdenCompra> Detalles { get; set; } = new();
        public EstadoOrden Estado { get; set; } = EstadoOrden.Pendiente;
    }
}
