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

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Crear(Presupuesto presupuesto)
    {
        if (ModelState.IsValid) //ModelState me permite validar los datos enviados al servidor a través del formulario o cuerpo de la solicitud
        {
            
            repoPresupuesto.CrearPresupuesto(presupuesto);
            
            return RedirectToAction("Index"); //En este caso, me redirige a la lista de presupuestos
        }
        else
        {
            return View(presupuesto); //Si el modelo no es válido nos muestra  el formulario con los errores
        }
    }
    
    public IActionResult ModificarPresupuesto(int id)
    {
        var presupuesto = repoPresupuesto.ObtenerPresupuestoPorId(id);
        if(presupuesto == null)
        {
            return NotFound("No se encontró el Presupuesto1");
        }
        return View(presupuesto); //Muestra el formulario para modificar el presupuesto
    }

    [HttpPost]
    public IActionResult Modificar(int id, Presupuesto presupuesto)
    {
        if (ModelState.IsValid)
        {
            var presupExistente = repoPresupuesto.ObtenerPresupuestoPorId(id);
            if (presupExistente == null)
            {
                return NotFound("No se encontró el Presupuesto2");
            }
            else
            {
                presupExistente.NombreDestinatario = presupuesto.NombreDestinatario;
                presupExistente.FechaCreacion = presupuesto.FechaCreacion;
                repoPresupuesto.CrearPresupuesto(presupExistente); // Acá lo actualizaría al presupuesto
                return RedirectToAction("Index");
            }
        }
        return View(presupuesto);
    }

    //Ahora quiero eliminar un presupuesto
    public IActionResult Eliminar(int id)
    {
        var presupuesto = repoPresupuesto.ObtenerPresupuestoPorId(id);
        if (presupuesto == null)
        {
            return NotFound("No se encontró el Presupuesto");
        }
        else
        {
            repoPresupuesto.EliminarPresupuestoPorId(id);
            return RedirectToAction("Index");
        }
    }

    //Voy a agregar productos a un presupuesto
    [HttpPost]
    public IActionResult AgregarProducto(int id, int productoId, int cantidad)
    {
        var presupuesto = repoPresupuesto.ObtenerPresupuestoPorId(id);
        if (presupuesto == null)
        {
            return NotFound("No se encontró el Presupuesto");
        }
        else
        {
            var producto = repoProducto.ObtenerProductoPorId(productoId);
            if (producto == null)
            {
                return NotFound("Producto no Encontrado");
            }
            else
            {
                var presupuestoDetalle = new PresupuestoDetalle(producto, cantidad);
                repoPresupuesto.AgregarProductoAlresupuesto(id, presupuestoDetalle.Prod, presupuestoDetalle.Cantidad);

                return RedirectToAction("Ver", new{id = id});
            }
        }
    }

    //Ahora quisiera ver un presupuesto con Productos
    public IActionResult Ver(int id)
    {
        var presupuesto = repoPresupuesto.ObtenerPresupuestoPorId(id);
        if (presupuesto == null)
        {
            return NotFound("Presupuesto no encontrado");
        }  

        return View(presupuesto);
    }
}