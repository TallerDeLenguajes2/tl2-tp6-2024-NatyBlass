using tl2_tp6_2024_NatyBlass.Models.PresupuestosDetalle;
namespace tl2_tp6_2024_NatyBlass.Models.Presupuestos
{
    public class Presupuesto
    {
        private int idPresupuesto;
        private string nombreDestinatario;
        private DateTime fechaCreacion;
        private List<PresupuestoDetalle> detalle;

        public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
        public string NombreDestinatario { get => nombreDestinatario; set => nombreDestinatario = value; }
        public DateTime FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
        public List<PresupuestoDetalle> Detalle { get => detalle; set => detalle = value; }

        public Presupuesto(int idPresupuesto, string nombreDestinatario, DateTime fechaCreacion)
        {
            this.idPresupuesto = idPresupuesto;
            this.nombreDestinatario = nombreDestinatario;
            this.fechaCreacion = fechaCreacion;
            this.detalle = new List<PresupuestoDetalle>();
        }


        public float MontoPresupuesto()
        {
            float total = 0;
            foreach (var item in detalle)
            {
                total = total + item.Prod.Precio * item.Cantidad;
            }

            return total;
        }

        public float MontoPresupuestoConIVA()
        {
            float totalConIVA = MontoPresupuesto() + (MontoPresupuesto() * 21)/ 100;
            return totalConIVA;
        }

        public int CantidadProductos()
        {
            int cantTotal = 0;
            foreach (var item in detalle)
            {
                cantTotal = cantTotal + item.Cantidad;
            }

            return cantTotal;
        }

    }
}