using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.OCP
{
    public abstract class MovimientoBase
    {
        public abstract void AplicarMovimiento(Producto producto, int cantidad);
    }
}
