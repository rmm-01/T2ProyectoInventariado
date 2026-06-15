using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.Interfaces
{
    public interface IOrdenCompraRepository
    {
        List<OrdenCompra> ObtenerTodos();
        OrdenCompra? ObtenerPorId(int id);
        void Agregar(OrdenCompra orden);
        void Actualizar(OrdenCompra orden);
    }
}
