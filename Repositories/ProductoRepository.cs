using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly List<Producto> _productos = new();
        private int _nextId = 1;

        public List<Producto> ObtenerTodos() => new(_productos);

        public Producto? ObtenerPorId(int id) => _productos.FirstOrDefault(p => p.Id == id);

        public void Agregar(Producto producto)
        {
            producto.Id = _nextId++;
            _productos.Add(producto);
        }

        public void Actualizar(Producto producto)
        {
            var existente = ObtenerPorId(producto.Id);
            if (existente == null) return;

            existente.Nombre = producto.Nombre;
            existente.Descripcion = producto.Descripcion;
            existente.Categoria = producto.Categoria;
            existente.UnidadMedida = producto.UnidadMedida;
            existente.PrecioCompra = producto.PrecioCompra;
            existente.PrecioVenta = producto.PrecioVenta;
            existente.StockActual = producto.StockActual;
            existente.StockMinimo = producto.StockMinimo;
            existente.StockMaximo = producto.StockMaximo;
        }
    }
}
