using T2ProyectoInventariado.Entities;

namespace T2ProyectoInventariado.Interfaces
{
    public interface IClienteRepository
    {
        List<Cliente> ObtenerTodos();
        Cliente? ObtenerPorId(int id);
        void Agregar(Cliente cliente);
        void Actualizar(Cliente cliente);
    }
}