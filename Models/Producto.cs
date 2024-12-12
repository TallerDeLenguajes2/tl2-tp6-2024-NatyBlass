public class Producto
{
    private int idProducto;
    private string descripcion;
    private float precio;

    public int IdProducto { get => idProducto; set => idProducto = value; }
    public string Descripcion { get => descripcion; set => descripcion = value; }
    public float Precio { get => precio; set => precio = value; }

    public Producto(int idProducto, string descripcion, float precio)
    {
        this.idProducto = idProducto;
        this.descripcion = descripcion;
        this.precio = precio;
    }
}