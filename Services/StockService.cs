using T3ProyectoInventariado.Entities;
using T3ProyectoInventariado.Interfaces;
using T3ProyectoInventariado.OCP;

namespace T3ProyectoInventariado.Services
{
    public class StockService : IStockService
    {
        private readonly MovimientoBase _movimiento;

        public StockService(MovimientoBase movimiento)
        {
            _movimiento = movimiento;
        }

        public void AplicarMovimiento(Producto producto, int cantidad)
        {
            _movimiento.AplicarMovimiento(producto, cantidad);
        }
    }
}
