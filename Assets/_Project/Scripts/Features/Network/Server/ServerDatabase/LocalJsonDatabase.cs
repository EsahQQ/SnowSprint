using System.IO;
using System.Security.Cryptography;
using System.Text;
using _Project.Scripts.Features.Network.Server.ServerDatabase.Data;
using UnityEngine;

namespace _Project.Scripts.Features.Network.Server.ServerDatabase
{
    public class LocalJsonDatabase : IServerDatabase
    {
        private readonly string _dbPath;
        private DatabaseModel _db;

        public LocalJsonDatabase()
        {
            _dbPath = Path.Combine(Application.persistentDataPath, "ServerUsers.json");
            LoadDatabase();
            Debug.Log($"[Server] БД загружена. Путь: {_dbPath}");
        }

        private void LoadDatabase()
        {
            if (File.Exists(_dbPath))
            {
                string json = File.ReadAllText(_dbPath);
                _db = JsonUtility.FromJson<DatabaseModel>(json) ?? new DatabaseModel();
            }
            else
            {
                _db = new DatabaseModel();
                SaveDatabase();
            }
        }

        private void SaveDatabase()
        {
            string json = JsonUtility.ToJson(_db, true);
            File.WriteAllText(_dbPath, json);
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public bool IsEmailExists(string email) => _db.Users.Exists(u => u.Email == email);

        public string CreateUserAndGetCode(string email, string username, string password)
        {
            email = email.Trim().ToLower(); 
            string code = Random.Range(100000, 999999).ToString();

            var existingUser = _db.Users.Find(u => u.Email == email);
            if (existingUser != null)
            {
                existingUser.VerificationCode = code;
                existingUser.Username = username; 
                existingUser.PasswordHash = HashPassword(password);
                SaveDatabase();
                Debug.Log($"[DB] Код обновлен для существующего {email}: {code}");
                return code;
            }

            _db.Users.Add(new UserData
            {
                Email = email,
                Username = username,
                PasswordHash = HashPassword(password),
                VerificationCode = code,
                IsActivated = false,
                Coins = 100
            });
    
            SaveDatabase();
            Debug.Log($"[DB] Новый юзер создан {email}: {code}");
            return code;
        }

        public bool TryVerify(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                Debug.LogWarning("[DB] Попытка верификации с пустым Email или Кодом");
                return false;
            }

            email = email.Trim().ToLower();
            code = code.Trim();

            if (_db == null || _db.Users == null)
            {
                Debug.LogError("[DB] База данных не инициализирована!");
                return false;
            }

            var user = _db.Users.Find(u => u.Email == email);

            if (user == null)
            {
                Debug.LogWarning($"[DB] Юзер с почтой {email} не найден.");
                return false;
            }

            if (user.VerificationCode == code)
            {
                user.IsActivated = true;
                SaveDatabase();
                Debug.Log($"[DB] Успешная активация для {email}");
                return true;
            }

            Debug.LogWarning($"[DB] Код не совпал! Ожидался: {user.VerificationCode}, Пришел: {code}");
            return false;
        }

        public UserData TryLogin(string email, string password)
        {
            string hash = HashPassword(password);
            return _db.Users.Find(u => u.Email == email && u.PasswordHash == hash);
        }
    }
}