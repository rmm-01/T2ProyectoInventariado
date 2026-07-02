using T3ProyectoInventariado.Entities;
using T3ProyectoInventariado.Interfaces;

namespace T3ProyectoInventariado.Services
{
    public class ProveedorService
    {
        private readonly IProveedorRepository _repository;

        public ProveedorService(IProveedorRepository repository)
        {
            _repository = repository;
        }

        public List<Proveedor> ObtenerTodos() => _repository.ObtenerTodos();

        public Proveedor? ObtenerPorId(int id) => _repository.ObtenerPorId(id);

        public void Agregar(Proveedor proveedor) => _repository.Agregar(proveedor);

        public void Actualizar(Proveedor proveedor) => _repository.Actualizar(proveedor);
    }
}
