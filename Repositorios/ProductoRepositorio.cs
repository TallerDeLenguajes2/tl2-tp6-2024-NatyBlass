using Microsoft.Data.Sqlite;
using tl2_tp6_2024_NatyBlass.Models.Presupuestos;
using tl2_tp6_2024_NatyBlass.Models.Productos;
using tl2_tp6_2024_NatyBlass.Models.PresupuestosDetalle;

public class ProductoRepositorio
{
    private string cadenaDeConexion = "Data Source=Tienda.db;Cache=Shared";

    private int ObtenerIdProducto(Producto producto)
    {
        var idABuscar = -999;
        string consulta = "SELECT idProducto FROM Productos WHERE Descripcion = @Descripcion AND Precio = @Precio;";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            var command = new SqliteCommand(consulta, connection);
            command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
            command.Parameters.AddWithValue("@Precio", producto.Precio);

            connection.Open();
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    idABuscar = Convert.ToInt32(reader["idProducto"]);
                }
            }

            connection.Close();
        }

        return idABuscar;
    }

    private void auxId(Producto producto)
    {
        int idCorrespondiente = this.ObtenerIdProducto(producto);
        if (idCorrespondiente != -999)
        { 
            producto.insertarId(idCorrespondiente);
        }
    }

    public Producto CrearProducto (Producto prod)
    {
        Producto nuevoProducto = prod;
        string consulta = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio);";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            connection.Open();

            var command = new SqliteCommand(consulta, connection);
            using (command)
            {
                command.Parameters.AddWithValue("@Descripcion", prod.Descripcion);
                command.Parameters.AddWithValue("@Precio", prod.Precio);
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        this.auxId(nuevoProducto);
        return nuevoProducto;
    }

    public Producto ModificarProducto(int id, Producto prod)
    {
        Producto producto = this.ObtenerProductoPorId(id);

        if (producto == null)
        {
            return producto;
        }

        string consulta = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @Id;";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            connection.Open();
            var command = new SqliteCommand(consulta, connection);

            using (command)
            {
                command.Parameters.AddWithValue("@Descripcion", prod.Descripcion);
                command.Parameters.AddWithValue("@Precio", prod.Precio);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        producto = this.ObtenerProductoPorId(id);
        this.auxId(producto);

        return producto;
    }

    public List<Producto> ListarProductos()
    {
        List<Producto> productos = new List<Producto>();
        string consulta = "SELECT * FROM Productos;";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            var command = new SqliteCommand(consulta, connection);

            connection.Open();
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    Producto producto = new Producto(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetInt32(2)
                    );
                    
                    productos.Add(producto);
                }
            }

            connection.Close();
        }

        return productos;
    }

    public Producto ObtenerProductoPorId(int id)
    {
        Producto prod = null;
        string consulta = "SELECT idProducto, Descripcion, Precio FROM Productos WHERE idProducto = @Id;"; // podría poner SELECT * FROM... (el * significa que selecciona todo)

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            var command = new SqliteCommand(consulta, connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();

            using(SqliteDataReader reader = command.ExecuteReader())
            {
                if(reader.Read())
                {
                    prod = new Producto(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetFloat(2) //Correccion porque en la clase es un FLOAT y no INT
                    );
                }
            }

            connection.Close();
        }

        return prod;
    }

    public bool EliminarProducto(int id)
    {
        Producto producto = this.ObtenerProductoPorId(id);

        if (producto == null)
        {
            return false;
        }

        string consultaProducto = "DELETE FROM Productos WHERE idProducto = @Id;";
        string consultaProductoEnPresupuesto = "DELETE FROM PresupuestosDetalle WHERE idProducto = @Id;";

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            connection.Open();

            using (var command = new SqliteCommand(consultaProductoEnPresupuesto, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand(consultaProducto, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        return true;
    }

}