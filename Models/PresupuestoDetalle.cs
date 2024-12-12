public class PresupuestoDetalle
{
    private Producto prod;
    private int cantidad;

    public Producto Prod { get => prod; set => prod = value; }
    public int Cantidad { get => cantidad; set => cantidad = value; }

    public PresupuestoDetalle(Producto prod, int cantidad)
    {
        this.prod = prod;
        this.cantidad = cantidad;
    }
}