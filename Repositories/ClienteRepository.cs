using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly List<Cliente> _clientes = new();
        private int _nextId = 1;

        public List<Cliente> ObtenerTodos() => new(_clientes);

        public Cliente? ObtenerPorId(int id) => _clientes.FirstOrDefault(c => c.Id == id);

        public void Agregar(Cliente cliente)
        {
            cliente.Id = _nextId++;
            _clientes.Add(cliente);
        }

        public void Actualizar(Cliente cliente)
        {
            var existente = ObtenerPorId(cliente.Id);
            if (existente == null) return;

            existente.Nombre = cliente.Nombre;
            existente.RucODni = cliente.RucODni;
            existente.Telefono = cliente.Telefono;
            existente.Correo = cliente.Correo;
            existente.Direccion = cliente.Direccion;
        }
    }
}