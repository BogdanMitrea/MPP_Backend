//using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;

namespace MPP_Backend.Repositories
{
    public class AuthRepository
    {
        private readonly string connectionString;
        private static Dictionary<string, string> _tokenStorage=new Dictionary<string, string>();
        String mypassword= "MpzwBuPsIHfhiudKFBpxeYrApLypiKMY";
        public AuthRepository()
        {
            //_connectionString = "Server=DESKTOP-DASUQ97\\SQLEXPRESS;Database=PhonesStore;Trusted_Connection=True;TrustServerCertificate=True";
            connectionString = "server=viaduct.proxy.rlwy.net;port=34412;user=root;password=" + mypassword+";database=railway";
            Console.WriteLine(connectionString);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connected to MySQL database.");
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        public bool Login(string username, string psswd)
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Appuser WHERE username=@username AND userpassword=@psswd";

            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@psswd", psswd);

            using MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                connection.Close();
                return true;
            }

            return false;
        }

        public int Register(string username, string psswd, string email)
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand getMaxIdCommand = connection.CreateCommand();
            getMaxIdCommand.CommandType = CommandType.Text;
            getMaxIdCommand.CommandText = "SELECT MAX(id) FROM Appuser";
            object maxIdResult = getMaxIdCommand.ExecuteScalar();
            int newId = (maxIdResult == DBNull.Value) ? 1 : Convert.ToInt32(maxIdResult) + 1;
            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO Appuser(id, username, userpassword, email) VALUES (@id, @username, @psswd, @email)";
            command.Parameters.AddWithValue("@id", newId);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@psswd", psswd);
            command.Parameters.AddWithValue("@email", email);
            command.ExecuteNonQuery();
            connection.Close();
            return newId;
        }

        public string GetTokenByUsername(string username)
        {
            if (_tokenStorage.ContainsKey(username))
            {
                return _tokenStorage[username];
            }
            return null;
        }

        public void SaveOrUpdateToken(string username, string token)
        {
            _tokenStorage[username] = token;
        }
    }
}
