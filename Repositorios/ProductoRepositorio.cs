using Microsoft.Data.Sqlite;


public class ProductoRepositorio
{
    private string cadenaDeConexion = "Data Source=Tienda.db;Cache=Shared";

    public void CrearProducto (Producto prod)
    {
        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio);";
            var command = new SqliteCommand(consulta, connection);
            command.Parameters.AddWithValue("@Descripcion", prod.Descripcion);
            command.Parameters.AddWithValue("@Precio", prod.Precio);
            
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public void ModificarProducto(int id, Producto prod)
    {
        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @Id;";
            var command = new SqliteCommand(consulta, connection);
            command.Parameters.AddWithValue("@Descripcion", prod.Descripcion);
            command.Parameters.AddWithValue("@Precio", prod.Precio);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public List<Producto> ListarProductos()
    {
        var productos = new List<Producto>();

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "SELECT idProducto, Descripcion, Precio FROM Productos;";
            var command = new SqliteCommand(consulta, connection);

            connection.Open();
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    var producto = new Producto(
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

        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "SELECT idProducto, Descripcion, Precio FROM Productos WHERE idProducto = @Id;";
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
                        reader.GetInt32(2)
                    );
                }
            }

            connection.Close();
        }

        return prod;
    }

    public void EliminarProducto(int id)
    {
        using (var connection = new SqliteConnection(cadenaDeConexion))
        {
            string consulta = "DELETE FROM Productos WHERE idProducto = @Id;";
            var command = new SqliteCommand(consulta, connection);
            command.Parameters.AddWithValue("@Id", id);
            
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

}