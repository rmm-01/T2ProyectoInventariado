using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Services
{
    public class ProductoService
    {
        private readonly IProductoRepository _repository;

        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        public List<Producto> ObtenerTodos() => _repository.ObtenerTodos();

        public Producto? ObtenerPorId(int id) => _repository.ObtenerPorId(id);

        public void Agregar(Producto producto)
        {
            bool existe = _repository.ObtenerTodos()
                .Any(p => p.Nombre.Trim().Equals(producto.Nombre.Trim(), StringComparison.OrdinalIgnoreCase));

            if (existe)
                throw new InvalidOperationException("Ya existe un producto con ese nombre.");

            _repository.Agregar(producto);
        }

        public void Actualizar(Producto producto) => _repository.Actualizar(producto);

        public bool TieneStockBajo(Producto producto) => producto.StockActual < producto.StockMinimo;
    }
}
