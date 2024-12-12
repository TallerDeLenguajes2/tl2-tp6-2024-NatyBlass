using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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


    /*public void AgregarProductoAPresupuesto(int idPresupuesto, Producto producto, int cantidad)
    {
        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad);";
            var command = new SqliteCommand(consulta, connection);
            
        }
    }
    */
}