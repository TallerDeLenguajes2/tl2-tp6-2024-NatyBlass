using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using tl2_tp6_2024_NatyBlass.Models.Presupuestos;
using tl2_tp6_2024_NatyBlass.Models.Productos;
using tl2_tp6_2024_NatyBlass.Models.PresupuestosDetalle;

public class PresupuestosController : Controller
{
    private readonly PresupuestoRepositorio repoPresupuesto;
    private readonly ProductoRepositorio repoProducto;

    public PresupuestosController()
    {
        repoPresupuesto = new PresupuestoRepositorio();
        repoProducto = new ProductoRepositorio();
    }

    public IActionResult Index()
    {
        var presupuestos = repoPresupuesto.ListarPresupuesto();
        return View("Index", presupuestos); //Así es como devuelvo la visa con la lista de presupuestos (en este caso)
    }

    [HttpGet]
    public IActionResult ObtenerPresupuestoPorId(int id)
    {
        var presupuestos = repoPresupuesto.ObtenerPresupuestoPorId(id);
        
        if (presupuestos == null)
        {
            ViewData["ErrorMessage"] = "No existe un presupuesto con el ID ingresado.";
            return View("Error");
        }

        return View(presupuestos);
    }

    [HttpGet]
    public IActionResult CrearPresupuesto()
    {
        return View(new Presupuesto());
    }

    [HttpPost]
    public IActionResult CrearPresupuesto(Presupuesto nuevoPresupuesto)
    {
        var producto = repoPresupuesto.CrearPresupuesto(nuevoPresupuesto);
        return RedirectToAction("ObtenerPresupuestoPorId");
    }

    [HttpGet]
    public IActionResult ModificarPresupuesto(int id)
    {
        var presupuesto = repoPresupuesto.ObtenerPresupuestoPorId(id);

        if (presupuesto == null)
        {
            ViewData["ErrorMessage"] = "No existe un presupuesto con el ID ingresado.";
            return View("Error");
        }

        return View(presupuesto);
    }

    [HttpPost]
    public IActionResult EliminarProductoPresupuesto(int idPresupuesto, int idProducto)
    {
        bool resultado = repoPresupuesto.EliminarProducto(idPresupuesto, idProducto);

        if (!resultado)
        {
            ViewData["ErrorMessage"] = "El producto no pudo ser eliminado";
            return View("Error");
        }

        return RedirectToAction("ModificarPresupuesto", new{id = idPresupuesto});
    }

    [HttpPost]
    public IActionResult ModificarCantidadProducto(int idPresupuesto, int idProducto, int nuevaCant)
    {
        bool resultado = repoPresupuesto.ActualizarCantidadProducto(idPresupuesto, idProducto, nuevaCant);

        if (!resultado)
        {
            ViewData["ErrorMessage"] = "No pudo actualizarse la cantidad del producto deseado.";
            return View("Error");
        }
        
        return RedirectToAction("ModificarPresupuesto", new{id = idPresupuesto});
    }

    [HttpGet]
    public IActionResult EliminarPresupuesto(int id)
    {
        var presupuesto = repoPresupuesto.ObtenerPresupuestoPorId(id);
        if (presupuesto == null)
        {
            ViewData["ErrorMessage"] = "El presupuesto con el ID ingresado no existe";
            return View("Error");
        }
        
        return View(presupuesto);
    }

    [HttpPost]
    public IActionResult EliminarPresupuesto(Presupuesto presupuesto, int id)
    {
        repoPresupuesto.EliminarPresupuestoPorId(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult AñadirProductoAlPresupuesto(int id)
    {
        var presupuesto = repoPresupuesto.ObtenerPresupuestoPorId(id);
        if (presupuesto == null)
        {
            ViewData["ErrorMessage"] = "El presupuesto con el ID ingresado no existe";
            return View("Error");
        }

        return View(presupuesto);
    }

    [HttpPost]
    public IActionResult AñadirProductoAlPresupuesto(int idPresupuesto, int idProducto, int cantidad)
    {
        Presupuesto auxiliar = repoPresupuesto.AgregarProductoAlPresupuesto(idPresupuesto, idProducto, cantidad);

        return RedirectToAction("AñadirProductoAlPresupuesto", new {id = idPresupuesto});
    }


}