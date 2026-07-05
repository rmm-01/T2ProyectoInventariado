using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _repository;

        public ClienteService(IClienteRepository repository)
        {
            _repository = repository;
        }

        public List<Cliente> ObtenerTodos() => _repository.ObtenerTodos();

        public Cliente? ObtenerPorId(int id) => _repository.ObtenerPorId(id);

        public void Agregar(Cliente cliente) => _repository.Agregar(cliente);

        public void Actualizar(Cliente cliente) => _repository.Actualizar(cliente);

        public bool DatosCompletos(Cliente cliente) =>
            !string.IsNullOrWhiteSpace(cliente.Nombre) && !string.IsNullOrWhiteSpace(cliente.RucODni);
    }
}