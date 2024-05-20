using System.Net.WebSockets;
using System.Net;
using System.Text;
using Microsoft.Data.SqlClient;
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
            _connectionString = "Server=DESKTOP-DASUQ97\\SQLEXPRESS;Database=PhonesStore;Trusted_Connection=True;TrustServerCertificate=True";
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

        public int get_maxID()
        {
            //int maxid = 0;
            //foreach(var a in phones)
            //{
            //    if(a.Id>maxid)
            //    {
            //        maxid = a.Id;
            //    }
            //}
            //return maxid+1;
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "Select max(Phone.id) from Phone";
            int maxid = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return maxid + 1;
        }

        public int AddPhone(PhoneModel phoneModel)
        {
            //int newid = get_maxID();
            //phoneModel.Id = newid;
            //phones.Add(phoneModel);
            //return phoneModel.Id;
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO Phone (phoneName, producer,color,yearOfRelease,phoneMemory,photo,store) VALUES (@phoneName, @producer,@color,@yearOfRelease,@phoneMemory,@photo,@store); SELECT SCOPE_IDENTITY()";

            command.Parameters.AddWithValue("@phoneName", phoneModel.Name);
            command.Parameters.AddWithValue("@producer", phoneModel.Producer);
            command.Parameters.AddWithValue("@color", phoneModel.Color);
            command.Parameters.AddWithValue("@yearOfRelease", phoneModel.Year);
            command.Parameters.AddWithValue("@phoneMemory", phoneModel.Memory);
            command.Parameters.AddWithValue("@photo", phoneModel.Photo);
            command.Parameters.AddWithValue("@store", phoneModel.Store);

            int newPhoneId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return newPhoneId;
        }

        public bool DeletePhone(int id)
        {
            //int indexToRemove = phones.FindIndex(phone => phone.Id == id);
            //if (indexToRemove != -1)
            //{
            //    phones.RemoveAt(indexToRemove);
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
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

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Phone";

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                PhoneModel phone = new PhoneModel
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["phoneName"],
                    Producer = (string)reader["producer"],
                    Color = (string)reader["color"],
                    Year = (int)reader["yearOfRelease"],
                    Memory = (int)reader["phoneMemory"],
                    Photo = (string)reader["photo"],
                    Store = (int)reader["store"]
                };
                phones.Add(phone);
            }

            reader.Close();
            connection.Close();

            return phones;
        }

        public IEnumerable<PhoneModel> GetPagedPhones(int page = 1, int pageSize = 5)
        {
            List<PhoneModel> phones = new List<PhoneModel>();

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT *\r\nFROM Phone\r\norder by id\r\nOFFSET "+(page-1)*pageSize+" ROWS\r\nFETCH NEXT "+pageSize+" ROWS ONLY;";

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                PhoneModel phone = new PhoneModel
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["phoneName"],
                    Producer = (string)reader["producer"],
                    Color = (string)reader["color"],
                    Year = (int)reader["yearOfRelease"],
                    Memory = (int)reader["phoneMemory"],
                    Photo = (string)reader["photo"],
                    Store = (int)reader["store"]
                };
                phones.Add(phone);
            }

            reader.Close();
            connection.Close();

            return phones;
        }

        public int getPhonesCount()
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "select count(*) from Phone";

            object result = command.ExecuteScalar();
            int.TryParse(result.ToString(),out int count);
            return count;
        }

        public PhoneModel? GetPhone(int id)
        {
            //return phones.ElementAt(phones.FindIndex(phone => phone.Id == id));
            PhoneModel? phone = null;

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Phone WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                phone = new PhoneModel
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["phoneName"],
                    Producer = (string)reader["producer"],
                    Color = (string)reader["color"],
                    Year = (int)reader["yearOfRelease"],
                    Memory = (int)reader["phoneMemory"],
                    Photo = (string)reader["photo"],
                    Store = (int)reader["store"]
                };
            }

            reader.Close();
            connection.Close();

            return phone;
        }

        public bool UpdatePhone(int id, PhoneModel phone)
        {
            //int indexToUpdate = phones.FindIndex(phone => phone.Id == id);
            //if (indexToUpdate != -1)
            //{
            //    phones[indexToUpdate].Name = phone.Name;
            //    phones[indexToUpdate].Memory = phone.Memory;
            //    phones[indexToUpdate].Producer = phone.Producer;
            //    phones[indexToUpdate].Color = phone.Color;
            //    phones[indexToUpdate].Photo = phone.Photo;
            //    phones[indexToUpdate].Year = phone.Year;
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE Phone SET phoneName = @name, producer = @producer, color = @color, yearOfRelease = @year, phoneMemory = @memory, photo = @photo  WHERE Id = @id";

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
                    Id = this.get_maxID(),
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

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "Select * FROM Phone WHERE store = @id";

            command.Parameters.AddWithValue("@id", storeid);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                PhoneModel phone = new PhoneModel
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["phoneName"],
                    Producer = (string)reader["producer"],
                    Color = (string)reader["color"],
                    Year = (int)reader["yearOfRelease"],
                    Memory = (int)reader["phoneMemory"],
                    Photo = (string)reader["photo"],
                    Store = (int)reader["store"]
                };
                phones.Add(phone);
            }

            reader.Close();
            connection.Close();

            return phones;
        }
    }
}
