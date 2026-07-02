using T3ProyectoInventariado.Entities;

namespace T3ProyectoInventariado.Interfaces
{
    public interface IStockService
    {
        void AplicarMovimiento(Producto producto, int cantidad);
    }
}
