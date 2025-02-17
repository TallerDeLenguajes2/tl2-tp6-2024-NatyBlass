using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_NatyBlass.Models.Presupuestos;
using tl2_tp6_2024_NatyBlass.Models.Productos;
using tl2_tp6_2024_NatyBlass.Models.PresupuestosDetalle;

public class ProductosController : Controller
{
    private readonly ProductoRepositorio repoProducto;

    public ProductosController(ProductoRepositorio productoRepositorio)
    {
        repoProducto = productoRepositorio;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ListarProductos()
    {
        var productos = repoProducto.ListarProductos();
        return View(productos);
    }

    [HttpGet]
    public IActionResult ObtenerProductoPorId(int id)
    {
        var producto = repoProducto.ObtenerProductoPorId(id);
        
        if (producto == null)
        {
            ViewData["ErrorMessage"] = "El producto con el ID ingresado no existe";
            return View("Error");
        }
        return View(producto);
    }

    [HttpGet("Crear")]
    public IActionResult Crear()
    {
        return View(new Producto()); //quiero un formulario para crear productos
    }

    [HttpPost("Crear")] // quiero crear el producto
    public IActionResult Crear(Producto nuevoProducto)
    {
            var producto = repoProducto.CrearProducto(nuevoProducto);
            return RedirectToAction("Listar");            
    }

    [HttpGet("Editar/{id}")] //quiero el formulario para editar el producto
    public IActionResult Editar(int id)
    {
        var producto = repoProducto.ObtenerProductoPorId(id);

        if (producto == null)
        {
            ViewData["ErrorMessage"] = "El producto con el ID ingresado no existe";
            return View("Error");
        }

        return View(producto);
    }

    [HttpPost("Editar/{id}")] //Aquí quiero ya modificar el producto
    public IActionResult Editar(int id, Producto productoAEditar)
    {
        var producto = repoProducto.ModificarProducto(id, productoAEditar);
        return RedirectToAction("Listar");
    }

    [HttpGet("Eliminar/{id}")] // quisiera eliminar un producto, por lo tanto quiero en formulario
    public IActionResult Eliminar(int id)
    {
        var producto = repoProducto.ObtenerProductoPorId(id);

        if (producto == null)
        {
            ViewData["ErrorMessage"] = "El producto con el ID ingresado no existe";
            return View("Error");
        }
        else
        {
            return View(producto);
        }
    }

    [HttpPost("Eliminar/{id}")] //acá ya eliminaría el producto
    public IActionResult Eliminar(Producto producto, int id)
    {
        repoProducto.EliminarProducto(id);
        return RedirectToAction("Listar");
    }


}