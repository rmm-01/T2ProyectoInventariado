using T3ProyectoInventariado.Entities;

namespace T3ProyectoInventariado.Interfaces
{
    public interface IProductoRepository
    {
        List<Producto> ObtenerTodos();
        Producto? ObtenerPorId(int id);
        void Agregar(Producto producto);
        void Actualizar(Producto producto);
    }
}
