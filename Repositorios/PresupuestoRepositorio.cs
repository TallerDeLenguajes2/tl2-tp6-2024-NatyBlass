using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using tl2_tp6_2024_NatyBlass.Models.Presupuestos;
using tl2_tp6_2024_NatyBlass.Models.Productos;
using tl2_tp6_2024_NatyBlass.Models.PresupuestosDetalle;
using Microsoft.AspNetCore.Mvc;

public class PresupuestoRepositorio
{
    private string cadenaDeConexion = "Data Source=Tienda.db;Cache=Shared";

    public void CrearPresupuesto(Presupuesto presup)
    {
        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion);";
            var command = new SqliteCommand(consulta, connection);
            command.Parameters.AddWithValue("@NombreDestinatario", presup.NombreDestinatario);
            command.Parameters.AddWithValue("@FechaCreacion", presup.FechaCreacion);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public List<Presupuesto> ListarPresupuesto()
    {
        var presupuestos = new List<Presupuesto>();
        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "SELECT IdPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos;";
            var command = new SqliteCommand(consulta, connection);
            
            connection.Open();
            
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var presupuesto = new Presupuesto(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetDateTime(2)
                    );

                    presupuestos.Add(presupuesto);
                }
            }
            
            connection.Close();
        }
            
        return presupuestos;
    }

    public Presupuesto ObtenerPresupuestoPorId(int id)
    {
        Presupuesto presup = null;

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "SELECT IdPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos WHERE IdPresupuesto = @Id;";
            var command = new SqliteCommand(consulta, connection);
            command.Parameters.AddWithValue("@Id", id);
            
            connection.Open();
            
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    presup = new Presupuesto(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetDateTime(2));

                }
            }

            connection.Close();
        }

        return presup;
    }

    private List<PresupuestoDetalle> ObtenerDetallesPorIdPresupuesto(int idPresupuesto)
    {
        var detalles = new List<PresupuestoDetalle>();
        var productoRepositorio = new ProductoRepositorio();

        using(var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "SELECT IdProducto, Cantidad FROM PresupuestoDetalle WHERE IdPresupuesto = @IdPresupuesto;";
            var command = new SqliteCommand(consulta, connection);
            command.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);
            
            connection.Open();

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    var producto = productoRepositorio.ObtenerProductoPorId(reader.GetInt32(0));
                    var detalle = new PresupuestoDetalle(producto, reader.GetInt32(1));
                    detalles.Add(detalle);
                }
            }

            connection.Close();
        }

        return detalles;
    }

    public bool AgregarProductoAlresupuesto(int idPresupuesto, Producto producto, int cantidad)
    {
        var presupuesto = ObtenerPresupuestoPorId(idPresupuesto);
        
        if (presupuesto != null)
        {
            using (var connection = new SqliteConnection(cadenaDeConexion))
            {
                string consulta = "INSERT INTO PresupuestoDetalle (IdPresupuesto, IdProducto, Cantidad) VALUES (@IdPresupuesto, @IdProducto, @Cantidad);";
                var command = new SqliteCommand(consulta, connection);
                command.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);
                command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                command.Parameters.AddWithValue("@Cantidad", cantidad);

                connection.Open();

                //Voy a utilizar el método de SqliteCommand que me devuelve el número de filas afectadas en una consulta SQL que se haya ejecutado para verificar que se realizó exitosamente la consulta

                int filasAfectadas = command.ExecuteNonQuery(); // acá tengo la cant de filas afectadas
                connection.Close();

                if(filasAfectadas > 0)
                {
                    return true; // si se insertó al menos 1 fila, funcionó la consulta.
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }

    public bool EliminarPresupuestoPorId(int idPresupuesto)
    {
        //Este DELETE debe ser en parte ya que primero tengo que boorar los detalles del presupuesto

        using(var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consultaDetPresupuesto = "DELETE FROM PresupuestoDetalle WHERE IdPresupuesto = @IdPresupuesto;";
            var commandDetPresupuesto =  new SqliteCommand(consultaDetPresupuesto, connection);
            commandDetPresupuesto.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);

            connection.Open();
            commandDetPresupuesto.ExecuteNonQuery();
            connection.Close();
        }

        //Una vez que borré el detalle, borro el presupuesto

        using(var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consultaPresup = "DELETE FROM Presupuestos WHERE IdPresupuesto = @IdPresupuesto;";
            var commandPresup = new SqliteCommand(consultaPresup, connection);
            commandPresup.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);

            connection.Open();
            //Reviso si hay filas afectadas
            int filasAfectadas = commandPresup.ExecuteNonQuery();
            connection.Close();

            if (filasAfectadas > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
    
}