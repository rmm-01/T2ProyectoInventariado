using T3ProyectoInventariado.Entities;

namespace T3ProyectoInventariado.Interfaces
{
    public interface IOrdenCompraRepository
    {
        List<OrdenCompra> ObtenerTodos();
        OrdenCompra? ObtenerPorId(int id);
        void Agregar(OrdenCompra orden);
        void Actualizar(OrdenCompra orden);
    }
}
