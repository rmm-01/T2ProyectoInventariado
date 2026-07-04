using T2ProyectoInventariado.Entities;
using T2ProyectoInventariado.OCP;
using T2ProyectoInventariado.Services;

namespace T2ProyectoInventariado.Tests;

public class MovimientoStockTests
{
    private static Producto NuevoProducto(int stockInicial) => new()
    {
        Id = 1,
        Nombre = "Producto de prueba",
        StockActual = stockInicial
    };

    [Fact]
    public void MovimientoEntrada_Suma_Cantidad_Al_Stock()
    {
        var producto = NuevoProducto(stockInicial: 10);
        var service = new StockService(new MovimientoEntrada());

        service.AplicarMovimiento(producto, 5);

        Assert.Equal(15, producto.StockActual);
    }

    [Fact]
    public void MovimientoSalida_Resta_Cantidad_Al_Stock()
    {
        var producto = NuevoProducto(stockInicial: 10);
        var service = new StockService(new MovimientoSalida());

        service.AplicarMovimiento(producto, 4);

        Assert.Equal(6, producto.StockActual);
    }

    [Fact]
    public void MovimientoSalida_Permite_Stock_Negativo_Si_No_Se_Valida_Antes()
    {
        // Documenta el comportamiento actual: MovimientoSalida no valida stock
        // suficiente, la validacion de negocio queda a cargo de quien la invoque.
        var producto = NuevoProducto(stockInicial: 2);
        var service = new StockService(new MovimientoSalida());

        service.AplicarMovimiento(producto, 5);

        Assert.Equal(-3, producto.StockActual);
    }
}
