using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using tl2_tp6_2024_NatyBlass.Models.Presupuestos;
using tl2_tp6_2024_NatyBlass.Models.Productos;
using tl2_tp6_2024_NatyBlass.Models.PresupuestosDetalle;
using Microsoft.AspNetCore.Mvc;

public class PresupuestoRepositorio
{
    private readonly string cadenaDeConexion = "Data Source=Tienda.db;Cache=Shared";

    private int ObtenerId(Presupuesto presupuesto)
    {
        var idABuscar = -999;
        string consulta = "SELECT idPresupuesto FROM Presupuestos WHERE NombreDestinatario = @NombreDestinatario;";

        using(var connection = new SqliteConnection(cadenaDeConexion))
        {
            var command = new SqliteCommand(consulta, connection);
            command.Parameters.Add(new SqliteParameter("@NombreDestinatario", presupuesto.NombreDestinatario));
            connection.Open();
            using(SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    idABuscar = Convert.ToInt32(reader["idPresupuesto"]);
                }
            };
            connection.Close();
        }

        return idABuscar;
        
    }

    private void auxId(Presupuesto presupuesto)
    {
        int idCorrespondiente = this.ObtenerId(presupuesto);
        if (idCorrespondiente != -999)
        { 
            presupuesto.insId(idCorrespondiente);
        }
    }

    public Presupuesto CrearPresupuesto(Presupuesto presup)
    {
        string consulta = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion);";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            connection.Open();

            var command = new SqliteCommand(consulta, connection);
            using (command)
            {
                command.Parameters.AddWithValue("@NombreDestinatario", presup.NombreDestinatario);
                command.Parameters.AddWithValue("@FechaCreacion", presup.FechaCreacion);
                presup.insId(Convert.ToInt32(command.ExecuteScalar()));
                int filasAfectadas = command.ExecuteNonQuery();
                
                Console.WriteLine($"Filas insertadas: {filasAfectadas}");//más para debugg

                if(filasAfectadas == 0) // Estoy tratando de hacer debug ya que no se guarda en la base de datos
                {
                    throw new Exception("No se pudo insertar en presupuesto en la base de datos");
                }

            }
            connection.Close();
        }

        return presup;
    }

    public List<Presupuesto> ListarPresupuesto()
    {
        List<Presupuesto> presupuestos = new List<Presupuesto>();

        string consultaPresupuestos = "SELECT idPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos;";
        string consultaDetallePresupuestos = @"SELECT presupDet.idProducto, presupDet.Cantidad, prod.Descripcion, prod.Precio
                                               FROM PresupuestosDetalle presupDet
                                               INNER JOIN Productos prod USING(idProducto)
                                               WHERE presupDet.idPresupuesto = @idPresupuesto;";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            connection.Open();

            var command = new SqliteCommand(consultaPresupuestos, connection);
            
            using (command)
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idPresupuesto = reader.GetInt32(0);
                        string nombreDestinatario = reader.GetString(1);
                        DateTime fechaCreacion = reader.GetDateTime(2);

                        List<PresupuestoDetalle> detalles = new List<PresupuestoDetalle>();
                        using(var detalleConnection = new SqliteConnection(cadenaDeConexion))
                        {
                            detalleConnection.Open();
                            var detCommand = new SqliteCommand(consultaDetallePresupuestos, detalleConnection);

                            using(detCommand)
                            {
                                detCommand.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                                using(SqliteDataReader detalleReader = detCommand.ExecuteReader())
                                {
                                    while (detalleReader.Read())
                                    {
                                        int idProducto = detalleReader.GetInt32(0);
                                        int cantidad = detalleReader.GetInt32(1);
                                        string descripcion = detalleReader.GetString(2);
                                        int precio = detalleReader.GetInt32(3);

                                        Producto producto = new Producto(idProducto, descripcion, precio);
                                        PresupuestoDetalle detalle = new PresupuestoDetalle(producto, cantidad);

                                        detalles.Add(detalle);
                                    }
                                }
                            }

                            detalleConnection.Close();
                        }

                        presupuestos.Add(new Presupuesto(idPresupuesto, nombreDestinatario, fechaCreacion, detalles));
                    }
                }
            }
            
            connection.Close();
        }
            
        return presupuestos;
    }

    public Presupuesto ObtenerPresupuestoPorId(int id)
    {
        Presupuesto presup = null;

        string consulta = "SELECT idPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos WHERE idPresupuesto = @Id;";
        string consultaDetallePresupuestos = @"SELECT presupDet.idProducto, presupDet.Cantidad, prod.Descripcion, prod.Precio
                                               FROM PresupuestosDetalle presupDet
                                               INNER JOIN Productos prod USING(idProducto)
                                               WHERE presupDetp.idPresupuesto = @idPresupuesto;";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            connection.Open();
            
            var command = new SqliteCommand(consulta, connection);
            using (command)
            {
                command.Parameters.AddWithValue("@Id", id);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idPresupuesto = reader.GetInt32(0);
                        string nombreDestinatario = reader.GetString(1);
                        DateTime fechaCreacion = reader.GetDateTime(2);
    
                        reader.Close(); //esto es para cerrar el reader antes de ejecutar la consulta de detalles

                        List<PresupuestoDetalle> detalles = new List<PresupuestoDetalle>();
                        var detCommand = new SqliteCommand(consultaDetallePresupuestos, connection);
                        using (detCommand)
                        {
                            detCommand.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                            
                            using(SqliteDataReader detReader = detCommand.ExecuteReader())
                            {
                                while (detReader.Read())
                                {
                                    int idProducto = detReader.GetInt32(0);
                                    int cantidad = detReader.GetInt32(1);
                                    string descripcion = detReader.GetString(2);
                                    int precio = detReader.GetInt32(3);

                                        Producto producto = new Producto(idProducto, descripcion, precio);
                                        PresupuestoDetalle detalle = new PresupuestoDetalle(producto, cantidad);

                                        detalles.Add(detalle);
                                }
                            }

                            presup = new Presupuesto(idPresupuesto, nombreDestinatario, fechaCreacion, detalles);
                        }
                    }
                }
            }

            connection.Close();
        }

        return presup;
    }

    public Presupuesto AgregarProductoAlPresupuesto(int idPresupuesto, int idProducto, int cant)
    {
        ProductoRepositorio prodRepositorio = new ProductoRepositorio();

        Presupuesto nuevoPresupuesto = new Presupuesto();

        var prodAñadir = prodRepositorio.ObtenerProductoPorId(idProducto);

        if (prodAñadir != null)
        {
            string consultarExistencia = "SELECT COUNT(*) FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto;";
            bool existe = false;

            using(var connection = new SqliteConnection(cadenaDeConexion))
            {
                connection.Open();
                var command = new SqliteCommand(consultarExistencia, connection);

                using (command)
                {
                    command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                    command.Parameters.AddWithValue("@idProducto", idProducto);

                    var count = Convert.ToInt32(command.ExecuteScalar());

                    existe = count > 0; //acá será true si al menos hay una tupla
                }

                connection.Close();
            }
            //Decidí seguir la lógica de que si EXISTE hago un UPDATE y sino, un INSERT

            if(existe)
            {
                string consultaUpdate = "UPDATE PresupuestosDetalles SET Cantidad = @cantidad WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto;";
                using(var connection = new SqliteConnection(cadenaDeConexion))
                {
                    connection.Open();
                    var command = new SqliteCommand(consultaUpdate, connection);

                    using (command)
                    {
                        command.Parameters.AddWithValue("@cantidad", cant);
                        command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                        command.Parameters.AddWithValue("@idProducto", idProducto);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            else
            {
                string consultaInsertar = "INSERT INTO PresupuestosDeatlles (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @cantidad);";
                using(var connection = new SqliteConnection(cadenaDeConexion))
                {
                    connection.Open();
                    var command = new SqliteCommand(consultaInsertar, connection);

                    using(command)
                    {
                        command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                        command.Parameters.AddWithValue("@idProducto", idProducto);
                        command.Parameters.AddWithValue("@cantidad", cant);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }

            nuevoPresupuesto = this.ObtenerPresupuestoPorId(idPresupuesto);
        }
    
        return nuevoPresupuesto;
    }

    public bool EliminarPresupuestoPorId(int idPresupuesto)
    {
        //Este DELETE debe ser en parte ya que primero tengo que boorar los detalles del presupuesto
        string consultaPresupuesto = "DELETE FROM Presupuestos WHERE idPresupuesto = @IdPresupuesto;";
        string consultaDetPresupuesto = "DELETE FROM PresupuestoDetalle WHERE idPresupuesto = @IdPresupuesto;";

        using(var connection = new SqliteConnection(cadenaDeConexion))
        {
            connection.Open();

            var commandDetalle = new SqliteCommand(consultaDetPresupuesto, connection);
            
            using(commandDetalle)
            {
                commandDetalle.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);
                commandDetalle.ExecuteNonQuery();
            }

             //Una vez que borré el detalle, borro el presupuesto

            var commandPresup = new SqliteCommand(consultaPresupuesto, connection);

            using (commandPresup)
            {
                commandPresup.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);
                commandPresup.ExecuteNonQuery();
                return true;
            }

            connection.Close();

        }

        return false;
    }
    
    public bool EliminarProducto(int idPresupuesto, int idProducto)
    {
        string consulta = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto;";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            connection.Open();
            var command = new SqliteCommand(consulta, connection);

            using (command)
            {
                command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                command.Parameters.AddWithValue("@idProducto", idProducto);

                int filasAfectadas = command.ExecuteNonQuery();
                
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
    
    public bool ActualizarCantidadProducto(int idPresupuesto, int idProducto, int nuevaCant)
    {
        string consulta = "UPDATE PresupuestosDetalle SET Cantidad = @nuevaCant WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto;";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            connection.Open();
            var command = new SqliteCommand(consulta, connection);

            using (command)
            {
                command.Parameters.AddWithValue("@nuevaCant", nuevaCant);
                command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                command.Parameters.AddWithValue("@idProducto", idProducto);

                int filasAfectadas = command.ExecuteNonQuery();

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


}