using System.Net.WebSockets;
using System.Net;
using System.Text;

namespace MPP_BackEnd.Repositories
{
    public class RepositoryPhone : IRepositoryPhone
    {
        private static List<PhoneModel> phones=/*GenerateRandomPhones(15);*/
         new List<PhoneModel>
        {
            new PhoneModel
            {
                Id = 1,
                Name = "iPhone 15",
                Producer = "Apple",
                Year = 2023,
                Color = "Black",
                Memory = 128,
                Photo = "./assets/iphone13.jpg"
            },
            new PhoneModel
            {
                Id = 2,
                Name = "Galaxy S21",
                Producer = "Samsung",
                Year = 2021,
                Color = "Phantom Gray",
                Memory = 1000,
                Photo = "./assets/s21.jpg"
            },
            new PhoneModel
            {
                Id = 3,
                Name = "Huawei P50",
                Producer = "Huawei",
                Year = 2022,
                Color = "Blue",
                Memory = 128,
                Photo = "./assets/huaweip50.jpg"
            },
            new PhoneModel
            {
                Id = 4,
                Name = "OnePlus 9 Pro",
                Producer = "OnePlus",
                Year = 2020,
                Color = "Morning Mist",
                Memory = 128,
                Photo = "./assets/oneplus9.jpg"
            },
            new PhoneModel
            {
                Id = 5,
                Name = "Xp 1 III",
                Producer = "Sony",
                Year = 2021,
                Color = "Frosted Black",
                Memory = 256,
                Photo = "./assets/xperia1.jpg"
            },
            new PhoneModel
            {
            Id = 6,
            Name = "Pixel 7 Pro",
            Producer = "Google",
            Year = 2023,
            Color = "Obsidian Black",
            Memory = 256,
            Photo = "https://fdn2.gsmarena.com/vv/pics/google/google-pixel7-pro-2.jpg" // Search for image online and replace with URL
            },
            new PhoneModel
            {
            Id = 7,
            Name = "Galaxy Z Fold 4",
            Producer = "Samsung",
            Year = 2023,
            Color = "Beige",
            Memory = 512,
            Photo = "https://www.gsmarena.com/samsung_galaxy_z_fold4-pictures-11737.php" // Search for image online and replace with URL
            },
            new PhoneModel
            {
            Id = 8,
            Name = "ROG Phone 6",
            Producer = "ASUS",
            Year = 2022,
            Color = "Meteor Black",
            Memory = 512,
            Photo = "https://www.91mobiles.com/asus-rog-phone-6-price-in-india?ty=gallery" // Search for image online and replace with URL
            },
            new PhoneModel
            {
            Id = 9,
            Name = "iPhone SE (3rd generation)",
            Producer = "Apple",
            Year = 2022,
            Color = "Midnight",
            Memory = 128,
            Photo = "https://www.91mobiles.com/apple-iphone-se-2022-price-in-india?ty=gallery" // Search for image online and replace with URL
            },
            new PhoneModel
            {
            Id = 10,
            Name = "Reno7 Pro",
            Producer = "Oppo",
            Year = 2023,
            Color = "Starlight Black",
            Memory = 256,
            Photo = "https://www.gsmarena.com/oppo_reno7_pro_5g-pictures-11190.php" // Search for image online and replace with  with URL
            },

        };
        private static Thread addingThread;
        private static readonly object threadLock = new object();
        public static List<PhoneModel> GenerateRandomPhones(int n)
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
                    Photo = GenerateRandomString(rand, rand.Next(10, 20))
                };

                randomphones.Add(phone);
            }

            return randomphones;
        }

        // Method to generate a random string of given length
        private static string GenerateRandomString(Random rand, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
        public RepositoryPhone()
        {
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
            int maxid = 0;
            foreach(var a in phones)
            {
                if(a.Id>maxid)
                {
                    maxid = a.Id;
                }
            }
            return maxid+1;
        }

        public int AddPhone(PhoneModel phoneModel)
        {
            int newid = get_maxID();
            phoneModel.Id = newid;
            phones.Add(phoneModel);
            return phoneModel.Id;
        }

        public bool DeletePhone(int id)
        {
            int indexToRemove = phones.FindIndex(phone => phone.Id == id);
            if (indexToRemove != -1)
            {
                phones.RemoveAt(indexToRemove);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<PhoneModel> GetAllPhones()
        {
            return phones;
        }

        public PhoneModel? GetPhone(int id)
        {
            return phones.ElementAt(phones.FindIndex(phone => phone.Id == id));
        }

        public bool UpdatePhone(int id, PhoneModel phone)
        {
            int indexToUpdate = phones.FindIndex(phone => phone.Id == id);
            if (indexToUpdate != -1)
            {
                phones[indexToUpdate].Name = phone.Name;
                phones[indexToUpdate].Memory = phone.Memory;
                phones[indexToUpdate].Producer = phone.Producer;
                phones[indexToUpdate].Color = phone.Color;
                phones[indexToUpdate].Photo = phone.Photo;
                phones[indexToUpdate].Year = phone.Year;
                return true;
            }
            else
            {
                return false;
            }
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
                Thread.Sleep(TimeSpan.FromSeconds(20));
                PhoneModel newphone = new PhoneModel
                {
                    Id = this.get_maxID(),
                    Name = "name",
                    Producer = "producer",
                    Year = 2026,
                    Color = "color",
                    Memory = 128,
                    Photo = "photo"
                };
                phones.Add(newphone);
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
    }
}
