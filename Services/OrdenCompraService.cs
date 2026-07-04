using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.Interfaces;

namespace T2ProyectoInventariado.Services
{
    public class OrdenCompraService
    {
        private readonly IOrdenCompraRepository _ordenRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IStockService _stockService;

        public OrdenCompraService(IOrdenCompraRepository ordenRepository, IProductoRepository productoRepository, IStockService stockService)
        {
            _ordenRepository = ordenRepository;
            _productoRepository = productoRepository;
            _stockService = stockService;
        }

        public List<OrdenCompra> ObtenerTodos() => _ordenRepository.ObtenerTodos();

        public OrdenCompra? ObtenerPorId(int id) => _ordenRepository.ObtenerPorId(id);

        public void Agregar(OrdenCompra orden) => _ordenRepository.Agregar(orden);

        public void ConfirmarRecepcion(int ordenId)
        {
            var orden = _ordenRepository.ObtenerPorId(ordenId);
            if (orden == null || orden.Estado == EstadoOrden.Recibida) return;

            foreach (var detalle in orden.Detalles)
            {
                var producto = _productoRepository.ObtenerPorId(detalle.ProductoId);
                if (producto != null)
                {
                    _stockService.AplicarMovimiento(producto, detalle.Cantidad);
                    _productoRepository.Actualizar(producto);
                }
            }

            orden.Estado = EstadoOrden.Recibida;
            _ordenRepository.Actualizar(orden);
        }
    }
}
