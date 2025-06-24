using System.Text.Json;
using ClienteManager.Models;
using System.Security.Cryptography;
using System.Text;

namespace ClienteManager.Data
{
    public class JsonDataService
    {
        private readonly string _usersFilePath;
        private readonly string _clientsFilePath;
        private readonly string _citiesFilePath;
        private readonly IWebHostEnvironment _env;
        private static readonly object _userLock = new object();
        private static readonly object _clientLock = new object();
        private static readonly object _cityLock = new object();

        public JsonDataService(IWebHostEnvironment env)
        {
            _env = env;
            var dataPath = Path.Combine(_env.ContentRootPath, "Data");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            _usersFilePath = Path.Combine(dataPath, "users.json");
            _clientsFilePath = Path.Combine(dataPath, "clients.json");
            _citiesFilePath = Path.Combine(dataPath, "Cidades.json");
        }

        // --- USUÁRIOS ---
        public List<ApplicationUser> GetUsers()
        {
            lock (_userLock)
            {
                if (!File.Exists(_usersFilePath)) return new List<ApplicationUser>();
                var json = File.ReadAllText(_usersFilePath);
                return JsonSerializer.Deserialize<List<ApplicationUser>>(json) ?? new List<ApplicationUser>();
            }
        }

        public void SaveUsers(List<ApplicationUser> users)
        {
            lock (_userLock)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(users, options);
                Console.WriteLine(">>> Salvando usuários em: " + _usersFilePath);
                File.WriteAllText(_usersFilePath, json);
            }
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // --- CLIENTES ---
        public List<Cliente> GetClients()
        {
            lock (_clientLock)
            {
                if (!File.Exists(_clientsFilePath)) return new List<Cliente>();
                var json = File.ReadAllText(_clientsFilePath);
                return JsonSerializer.Deserialize<List<Cliente>>(json) ?? new List<Cliente>();
            }
        }

        public void SaveClients(List<Cliente> clients)
        {
            lock (_clientLock)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(clients, options);
                File.WriteAllText(_clientsFilePath, json);
            }
        }

        // --- CIDADES ---
        public List<string> GetCities()
        {
            lock (_cityLock)
            {
                if (!File.Exists(_citiesFilePath))
                {
                    return new List<string>();
                }

                var json = File.ReadAllText(_citiesFilePath);
                var cidades = JsonSerializer.Deserialize<List<Cidade>>(json);
                return cidades?.Select(c => c.Nome).OrderBy(nome => nome).ToList() ?? new List<string>();
            }
        }
    }
}
