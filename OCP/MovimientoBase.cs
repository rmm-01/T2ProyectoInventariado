using T3ProyectoInventariado.Entities;

namespace T3ProyectoInventariado.OCP
{
    public abstract class MovimientoBase
    {
        public abstract void AplicarMovimiento(Producto producto, int cantidad);
    }
}
