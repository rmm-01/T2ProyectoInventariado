using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.Interfaces
{
    public interface IProveedorRepository
    {
        List<Proveedor> ObtenerTodos();
        Proveedor? ObtenerPorId(int id);
        void Agregar(Proveedor proveedor);
        void Actualizar(Proveedor proveedor);
    }
}
