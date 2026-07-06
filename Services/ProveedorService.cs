using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Services
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

        public void Eliminar(int id) => _repository.Eliminar(id);
    }
}
