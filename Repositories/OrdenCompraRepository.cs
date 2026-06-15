using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Repositories
{
    public class OrdenCompraRepository : IOrdenCompraRepository
    {
        private readonly List<OrdenCompra> _ordenes = new();
        private int _nextId = 1;

        public List<OrdenCompra> ObtenerTodos() => new(_ordenes);

        public OrdenCompra? ObtenerPorId(int id) => _ordenes.FirstOrDefault(o => o.Id == id);

        public void Agregar(OrdenCompra orden)
        {
            orden.Id = _nextId++;
            _ordenes.Add(orden);
        }

        public void Actualizar(OrdenCompra orden)
        {
            var existente = ObtenerPorId(orden.Id);
            if (existente == null) return;

            existente.Estado = orden.Estado;
            existente.Detalles = orden.Detalles;
        }
    }
}
