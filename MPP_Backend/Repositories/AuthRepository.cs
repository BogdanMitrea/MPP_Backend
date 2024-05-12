using Microsoft.Data.SqlClient;
using System.Data;

namespace MPP_Backend.Repositories
{
    public class AuthRepository
    {
        private readonly string _connectionString;
        private Dictionary<string, string> _tokenStorage;

        public AuthRepository()
        {
            _connectionString = "Server=DESKTOP-DASUQ97\\SQLEXPRESS;Database=PhonesStore;Trusted_Connection=True;TrustServerCertificate=True";
            _tokenStorage = new Dictionary<string, string>();
        }

        public bool Login(string username, string psswd)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "Select * from Appuser where username=@username and userpassword=@psswd";

            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@psswd", psswd);

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                connection.Close();
                return true;
            }
            connection.Close();
            return false;
        }

        public int Register(string username, string psswd, string email)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "Insert into Appuser(username,userpassword,email) values(@username,@psswd,@email);  SELECT SCOPE_IDENTITY()";

            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@psswd", psswd);
            command.Parameters.AddWithValue("@email", email);

            int newPhoneId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return newPhoneId;
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
