using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;
using T2ProyectoInventariado.OCP;

namespace T2ProyectoInventariado.Services
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
