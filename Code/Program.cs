using MySqlConnector;
using static MariaDB.Code.MariaDB;

class Program {
    static void Main() {
        // Opening and getting the connection
        MySqlConnection connection = ConnectDB();

        // Example how to run a command
        using(MySqlCommand cmd = new()) {
            cmd.CommandText = "INSERT INTO Table (Column1, Column2) VALUES ('@value1name', '@value2name');";
            cmd.Parameters.AddWithValue("@value1name", "value1");
            cmd.Parameters.AddWithValue("@value2name", "value2");
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
        }

        // Closing the connection - Important! Close the connection after every use!!!
        connection.Close();
        Console.ReadKey();
    }
}
