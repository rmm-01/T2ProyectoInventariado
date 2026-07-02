using T3ProyectoInventariado.Entities;
using T3ProyectoInventariado.Interfaces;

namespace T3ProyectoInventariado.Data
{
    public static class DatosIniciales
    {
        public static void Cargar(IProductoRepository productoRepo, IProveedorRepository proveedorRepo, IOrdenCompraRepository ordenRepo)
        {
            productoRepo.Agregar(new Producto
            {
                Nombre = "Arroz Importado Premium",
                Descripcion = "Arroz de grano largo importado de Tailandia",
                Categoria = "Alimentos",
                UnidadMedida = "Saco 50kg",
                PrecioCompra = 85.00m,
                PrecioVenta = 110.00m,
                StockActual = 25,
                StockMinimo = 10,
                StockMaximo = 100
            });

            productoRepo.Agregar(new Producto
            {
                Nombre = "Aceite de Oliva Extra Virgen",
                Descripcion = "Aceite de oliva importado de España",
                Categoria = "Alimentos",
                UnidadMedida = "Caja 12 botellas",
                PrecioCompra = 120.00m,
                PrecioVenta = 160.00m,
                StockActual = 5,
                StockMinimo = 8,
                StockMaximo = 50
            });

            productoRepo.Agregar(new Producto
            {
                Nombre = "Fideos Italianos",
                Descripcion = "Pasta artesanal importada de Italia",
                Categoria = "Alimentos",
                UnidadMedida = "Caja 24 paquetes",
                PrecioCompra = 45.00m,
                PrecioVenta = 65.00m,
                StockActual = 40,
                StockMinimo = 15,
                StockMaximo = 80
            });

            proveedorRepo.Agregar(new Proveedor
            {
                RazonSocial = "Thai Export Co.",
                Pais = "Tailandia",
                Contacto = "import@thaiexport.com"
            });

            proveedorRepo.Agregar(new Proveedor
            {
                RazonSocial = "Mediterranean Foods S.L.",
                Pais = "España",
                Contacto = "+34 91 555 1234"
            });

            ordenRepo.Agregar(new OrdenCompra
            {
                Fecha = DateTime.Now.AddDays(-3),
                ProveedorId = 1,
                Estado = EstadoOrden.Pendiente,
                Detalles = new List<DetalleOrdenCompra>
                {
                    new() { ProductoId = 1, Cantidad = 20, PrecioUnitario = 85.00m },
                    new() { ProductoId = 2, Cantidad = 10, PrecioUnitario = 120.00m }
                }
            });
        }
    }
}
