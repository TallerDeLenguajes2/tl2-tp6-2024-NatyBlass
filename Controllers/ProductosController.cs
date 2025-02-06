using Microsoft.AspNetCore.Mvc;

public class ProductosController : Controller
{
    private readonly ProductoRepositorio repoProducto;

    public ProductosController(ProductoRepositorio productoRepositorio)
    {
        repoProducto = productoRepositorio;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var productos = repoProducto.ListarProductos();
        return View(productos);
    }

    [HttpGet("Crear")]
    public IActionResult Crear()
    {
        return View(); //quiero un formulario para crear productos
    }

    [HttpPost("Crear")] // quiero crear el producto
    public IActionResult Crear(Producto producto)
    {
        if (ModelState.IsValid)
        {
            repoProducto.CrearProducto(producto);
            return RedirectToAction("Listar");            
        }

        return View(producto);
    }

    [HttpGet("Editar/{id}")] //quiero el formulario para editar el producto
    public IActionResult Editar(int id)
    {
        var producto = repoProducto.ObtenerProductoPorId(id);

        if (producto == null)
        {
            return NotFound("Producto no encontrado.");
        }
        else
        {
            return View(producto);
        }
    }

    [HttpPost("Editar/{id}")] //Aquí quiero ya modificar el producto
    public IActionResult Editar(int id, Producto producto)
    {
        if (ModelState.IsValid)
        {
            producto.IdProducto = id;
            repoProducto.ModificarProducto(id, producto);
            return RedirectToAction("Listar");
        }

        return View(producto);
    }

    [HttpGet("Eliminar/{id}")] // quisiera eliminar un producto, por lo tanto quiero en formulario
    public IActionResult Eliminar(int id)
    {
        var producto = repoProducto.ObtenerProductoPorId(id);

        if (producto == null)
        {
            return NotFound("Producto no encontrado.");
        }
        else
        {
            return View(producto);
        }
    }

    [HttpPost("Eliminar/{id}")] //acá ya eliminaría el producto
    public IActionResult ConfirmarEliminar(int id)
    {
        repoProducto.EliminarProducto(id);
        return RedirectToAction("Listar");
    }


}