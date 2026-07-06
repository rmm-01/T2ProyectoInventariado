using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.Interfaces
{
    public interface IProductoRepository
    {
        List<Producto> ObtenerTodos();
        Producto? ObtenerPorId(int id);
        void Agregar(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(int id);
    }
}
