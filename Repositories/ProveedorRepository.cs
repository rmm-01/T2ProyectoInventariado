using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly List<Proveedor> _proveedores = new();
        private int _nextId = 1;

        public List<Proveedor> ObtenerTodos() => new(_proveedores);

        public Proveedor? ObtenerPorId(int id) => _proveedores.FirstOrDefault(p => p.Id == id);

        public void Agregar(Proveedor proveedor)
        {
            proveedor.Id = _nextId++;
            _proveedores.Add(proveedor);
        }

        public void Actualizar(Proveedor proveedor)
        {
            var existente = ObtenerPorId(proveedor.Id);
            if (existente == null) return;

            existente.RazonSocial = proveedor.RazonSocial;
            existente.Pais = proveedor.Pais;
            existente.Contacto = proveedor.Contacto;
        }
    }
}
