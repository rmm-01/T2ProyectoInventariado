using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.Interfaces
{
    public interface IStockService
    {
        void AplicarMovimiento(Producto producto, int cantidad);
    }
}
