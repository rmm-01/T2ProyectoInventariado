using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.OCP
{
    public class MovimientoSalida : MovimientoBase
    {
        public override void AplicarMovimiento(Producto producto, int cantidad)
        {
            producto.StockActual -= cantidad;
        }
    }
}
