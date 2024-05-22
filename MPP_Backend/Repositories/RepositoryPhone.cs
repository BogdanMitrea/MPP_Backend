using System.Net.WebSockets;
using System.Net;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace MPP_BackEnd.Repositories
{
    public class RepositoryPhone : IRepositoryPhone
    {
        private readonly string _connectionString;

        private static Thread addingThread;
        private static readonly object threadLock = new object();

        public RepositoryPhone()
        {
            _connectionString = "server=viaduct.proxy.rlwy.net;port=34412;user=root;password=MpzwBuPsIHfhiudKFBpxeYrApLypiKMY;database=railway";
            //Console.WriteLine(_connectionString);
            //GenerateRandomPhones(40000000);
            StartWebSocketServerAsync();
            lock (threadLock)
            {
                if (addingThread == null)
                {
                    Console.WriteLine("Thread started");
                    addingThread = new Thread(generatenewPhone)
                    {
                        IsBackground = true
                    };
                    addingThread.Start();
                }
            }
        }

        public int GetMaxId()
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT MAX(id) FROM Phone";

            object result = command.ExecuteScalar();
            int maxId = (result == DBNull.Value) ? 0 : Convert.ToInt32(result);

            connection.Close();

            return maxId + 1;
        }

        public int AddPhone(PhoneModel phoneModel)
        {
            int newId = GetMaxId();

            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO Phone (id, phoneName, producer, color, yearOfRelease, phoneMemory, photo, store) " +
                                  "VALUES (@id, @phoneName, @producer, @color, @yearOfRelease, @phoneMemory, @photo, @store)";

            command.Parameters.AddWithValue("@id", newId);
            command.Parameters.AddWithValue("@phoneName", phoneModel.Name);
            command.Parameters.AddWithValue("@producer", phoneModel.Producer);
            command.Parameters.AddWithValue("@color", phoneModel.Color);
            command.Parameters.AddWithValue("@yearOfRelease", phoneModel.Year);
            command.Parameters.AddWithValue("@phoneMemory", phoneModel.Memory);
            command.Parameters.AddWithValue("@photo", phoneModel.Photo);
            command.Parameters.AddWithValue("@store", phoneModel.Store);

            command.ExecuteNonQuery();

            connection.Close();

            return newId;
        }

        public bool DeletePhone(int id)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM Phone WHERE id = @id";

            command.Parameters.AddWithValue("@id", id);

            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();
            return rowsAffected > 0;
        }

        public IEnumerable<PhoneModel> GetAllPhones()
        {
            List<PhoneModel> phones = new List<PhoneModel>();

            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Phone";

            using MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                PhoneModel phone = new PhoneModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["phoneName"].ToString(),
                    Producer = reader["producer"].ToString(),
                    Color = reader["color"].ToString(),
                    Year = Convert.ToInt32(reader["yearOfRelease"]),
                    Memory = Convert.ToInt32(reader["phoneMemory"]),
                    Photo = reader["photo"].ToString(),
                    Store = Convert.ToInt32(reader["store"])
                };
                phones.Add(phone);
            }

            connection.Close();

            return phones;
        }

        public IEnumerable<PhoneModel> GetPagedPhones(int page = 1, int pageSize = 5)
        {
            List<PhoneModel> phones = new List<PhoneModel>();

            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Phone ORDER BY id LIMIT @pageSize OFFSET @offset";
            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

            using MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                PhoneModel phone = new PhoneModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["phoneName"].ToString(),
                    Producer = reader["producer"].ToString(),
                    Color = reader["color"].ToString(),
                    Year = Convert.ToInt32(reader["yearOfRelease"]),
                    Memory = Convert.ToInt32(reader["phoneMemory"]),
                    Photo = reader["photo"].ToString(),
                    Store = Convert.ToInt32(reader["store"])
                };
                phones.Add(phone);
            }

            connection.Close();

            return phones;
        }

        public int getPhonesCount()
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT COUNT(*) FROM Phone";

            int count = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return count;
        }


        public PhoneModel? GetPhone(int id)
        {
            PhoneModel? phone = null;

            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Phone WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                phone = new PhoneModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["phoneName"].ToString(),
                    Producer = reader["producer"].ToString(),
                    Color = reader["color"].ToString(),
                    Year = Convert.ToInt32(reader["yearOfRelease"]),
                    Memory = Convert.ToInt32(reader["phoneMemory"]),
                    Photo = reader["photo"].ToString(),
                    Store = Convert.ToInt32(reader["store"])
                };
            }

            connection.Close();

            return phone;
        }

        public bool UpdatePhone(int id, PhoneModel phone)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE Phone SET phoneName = @name, producer = @producer, color = @color, yearOfRelease = @year, phoneMemory = @memory, photo = @photo WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", phone.Name);
            command.Parameters.AddWithValue("@producer", phone.Producer);
            command.Parameters.AddWithValue("@color", phone.Color);
            command.Parameters.AddWithValue("@year", phone.Year);
            command.Parameters.AddWithValue("@memory", phone.Memory);
            command.Parameters.AddWithValue("@photo", phone.Photo);

            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            return rowsAffected > 0;
        }


        private WebSocket webSocket;
        public async Task StartWebSocketServerAsync()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/ws/");
            listener.Start();
            Console.WriteLine("WebSocket server started");

            while (true)
            {
                var context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var webSocketContext = await context.AcceptWebSocketAsync(null);
                    webSocket = webSocketContext.WebSocket;
                    Console.WriteLine("Client connected");
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        public void generatenewPhone()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(60));
                PhoneModel newphone = new PhoneModel
                {
                    Id = this.GetMaxId(),
                    Name = "name",
                    Producer = "producer",
                    Year = 2026,
                    Color = "color",
                    Memory = 128,
                    Photo = "photo",
                    Store=1
                };
                this.AddPhone(newphone);
                Console.WriteLine("Phone created");
                SendWebSocketMessage("New phone generated");
            }
        }

        public async Task SendWebSocketMessage(string message)
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public void GenerateRandomPhones(int n)
        {
            List<PhoneModel> randomphones = new List<PhoneModel>();
            Random rand = new Random();
            for (int i = 0; i < n; i++)
            {
                PhoneModel phone = new PhoneModel
                {
                    Id = i + 1,
                    Name = GenerateRandomString(rand, rand.Next(5, 15)),
                    Producer = GenerateRandomString(rand, rand.Next(5, 15)),
                    Year = rand.Next(2000, 2025), // Random year between 2000 and 2024
                    Color = GenerateRandomString(rand, rand.Next(5, 15)),
                    Memory = rand.Next(32, 513),
                    Photo = GenerateRandomString(rand, rand.Next(10, 20)),
                    Store = rand.Next(1, 1038)
                };
                try
                {
                    this.AddPhone(phone);
                }
                catch(Exception) {}
            }
        }

        // Method to generate a random string of given length
        private static string GenerateRandomString(Random rand, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        public IEnumerable<PhoneModel> GetPhonesByStore(int storeid)
        {
            List<PhoneModel> phones = new List<PhoneModel>();

            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Phone WHERE store = @id";

            command.Parameters.AddWithValue("@id", storeid);

            using MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                PhoneModel phone = new PhoneModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["phoneName"].ToString(),
                    Producer = reader["producer"].ToString(),
                    Color = reader["color"].ToString(),
                    Year = Convert.ToInt32(reader["yearOfRelease"]),
                    Memory = Convert.ToInt32(reader["phoneMemory"]),
                    Photo = reader["photo"].ToString(),
                    Store = Convert.ToInt32(reader["store"])
                };
                phones.Add(phone);
            }

            connection.Close();

            return phones;
        }
    }
}
