using T3ProyectoInventariado.Entities;

namespace T3ProyectoInventariado.OCP
{
    public class MovimientoSalida : MovimientoBase
    {
        public override void AplicarMovimiento(Producto producto, int cantidad)
        {
            producto.StockActual -= cantidad;
        }
    }
}
