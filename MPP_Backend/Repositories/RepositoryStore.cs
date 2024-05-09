
using Microsoft.Data.SqlClient;
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
            _connectionString = "Server=DESKTOP-DASUQ97\\SQLEXPRESS;Database=PhonesStore;Trusted_Connection=True;TrustServerCertificate=True";
            //this.GenerateRandomStores(500);
        }

        public int AddStore(Store store)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO Store (storename) VALUES (@storeName); SELECT SCOPE_IDENTITY()";

            command.Parameters.AddWithValue("@storeName", store.Name);

            int newStoreId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return newStoreId;
        }

        public bool DeleteStore(int id)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "DELETE FROM Store WHERE Id = @id";

                command.Parameters.AddWithValue("@id", id);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0; // Return true if at least one row was deleted
            }
        }

        public IEnumerable<Store> GetAllStores()
        {
            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM Store";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["Id"]);
                    string name = reader["storename"] as string;
                    yield return new Store { Id = id, Name = name }; // Assuming a Store class with Id and Name properties
                }

                reader.Close();
            }
        }

        public Store? GetStore(int id)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM Store WHERE Id = @id";

                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int storeId = Convert.ToInt32(reader["Id"]);
                    string name = reader["storename"] as string;
                    return new Store { Id = storeId, Name = name };
                }
                else
                {
                    reader.Close();
                    return null; // Store not found
                }
            }
        }

        public bool UpdateStore(int id, Store store)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "UPDATE Store SET storeName = @name WHERE Id = @id";

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", store.Name);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0; // Return true if at least one row was updated
            }
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
