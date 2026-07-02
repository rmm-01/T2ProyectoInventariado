using T3ProyectoInventariado.Entities;

namespace T3ProyectoInventariado.Interfaces
{
    public interface IProveedorRepository
    {
        List<Proveedor> ObtenerTodos();
        Proveedor? ObtenerPorId(int id);
        void Agregar(Proveedor proveedor);
        void Actualizar(Proveedor proveedor);
    }
}
