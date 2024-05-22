//using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using MPP_BackEnd;
using System.Data;
using System.Threading;

namespace MPP_Backend.Repositories
{
    public class RepositoryStore : IRepositoryStore
    {
        private readonly string _connectionString;

        public RepositoryStore()
        {
            _connectionString = "server=viaduct.proxy.rlwy.net;port=34412;user=root;password=MpzwBuPsIHfhiudKFBpxeYrApLypiKMY;database=railway";
            //this.GenerateRandomStores(500);
        }

        public int AddStore(Store store)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();
            MySqlCommand getMaxIdCommand = connection.CreateCommand();
            getMaxIdCommand.CommandType = CommandType.Text;
            getMaxIdCommand.CommandText = "SELECT MAX(Id) FROM Store";
            int currentMaxId = Convert.ToInt32(getMaxIdCommand.ExecuteScalar());
            int newId = currentMaxId + 1;

            MySqlCommand insertCommand = connection.CreateCommand();
            insertCommand.CommandType = CommandType.Text;
            insertCommand.CommandText = "INSERT INTO Store (Id, storename) VALUES (@id, @storeName)";

            insertCommand.Parameters.AddWithValue("@id", newId);
            insertCommand.Parameters.AddWithValue("@storeName", store.Name);

            int rowsAffected = insertCommand.ExecuteNonQuery();
            connection.Close();

            return newId;
        }

        public bool DeleteStore(int id)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM Store WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);

            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();
            return rowsAffected > 0; // Return true if at least one row was deleted
        }

        public IEnumerable<Store> GetAllStores()
        {
            List<Store> stores = new List<Store>();

            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Store";

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["Id"]);
                string name = reader["storename"] as string;
                stores.Add(new Store { Id = id, Name = name }); // Assuming a Store class with Id and Name properties
            }

            connection.Close();

            return stores;
        }

        public Store? GetStore(int id)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Store WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                int storeId = Convert.ToInt32(reader["Id"]);
                string name = reader["storename"] as string;
                return new Store { Id = storeId, Name = name };
            }
            else
            {
                return null; // Store not found
            }
        }

        public bool UpdateStore(int id, Store store)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE Store SET storeName = @name WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", store.Name);

            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();
            return rowsAffected > 0; // Return true if at least one row was updated
        }



        public void GenerateRandomStores(int n)
        {
            List<Store> randomphones = new List<Store>();
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                Store store = new Store
                {
                    Id=-1,
                    Name= GenerateRandomString(rand, rand.Next(5, 15)),
                };
                this.AddStore(store);
            }
        }

        private static string GenerateRandomString(Random rand, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}
