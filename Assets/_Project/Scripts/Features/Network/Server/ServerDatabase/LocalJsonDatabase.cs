using System.Collections.Generic;
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
            Debug.Log($"[LocalDB] БД загружена. Путь: {_dbPath}");
        }

        private void LoadDatabase()
        {
            if (File.Exists(_dbPath))
            {
                var json = File.ReadAllText(_dbPath);
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
            var json = JsonUtility.ToJson(_db, true);
            File.WriteAllText(_dbPath, json);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private UserData FindByEmail(string email) =>
            _db.Users.Find(u => u.Email == email.Trim().ToLower());
        

        public bool IsEmailExists(string email) =>
            _db.Users.Exists(u => u.Email == email.Trim().ToLower());

        public string CreateUserAndGetCode(string email, string username, string password)
        {
            email = email.Trim().ToLower();
            var code = Random.Range(100000, 999999).ToString();
            var existing = FindByEmail(email);

            if (existing != null)
            {
                existing.VerificationCode = code;
                existing.Username = username;
                existing.PasswordHash = HashPassword(password);
            }
            else
            {
                _db.Users.Add(new UserData
                {
                    Email = email,
                    Username = username,
                    PasswordHash = HashPassword(password),
                    VerificationCode = code,
                    IsActivated = false,
                    Coins = 100,
                    UnlockedUpgrades = new List<string>()
                });
            }

            SaveDatabase();
            Debug.Log($"[LocalDB] Код {code} создан для {email}");
            return code;
        }

        public bool TryVerify(string email, string code)
        {
            email = email.Trim().ToLower();
            code = code.Trim();

            var user = FindByEmail(email);
            if (user == null)
            {
                Debug.LogWarning($"[LocalDB] Пользователь {email} не найден для верификации");
                return false;
            }

            if (user.VerificationCode != code)
            {
                Debug.LogWarning($"[LocalDB] Код не совпал. Ожидался: {user.VerificationCode}, Пришёл: {code}");
                return false;
            }

            user.IsActivated = true;
            SaveDatabase();
            Debug.Log($"[LocalDB] Аккаунт {email} активирован");
            return true;
        }

        public UserData TryLogin(string email, string password)
        {
            var hash = HashPassword(password);
            return _db.Users.Find(u =>
                u.Email == email.Trim().ToLower() &&
                u.PasswordHash == hash);
        }

        public UserData GetUserByEmail(string email) => FindByEmail(email);

        public void UpdateCoins(string email, int newAmount)
        {
            var user = FindByEmail(email);
            if (user == null) return;
            user.Coins = newAmount;
            SaveDatabase();
        }

        public void AddUserUpgrade(string email, string upgradeId)
        {
            var user = FindByEmail(email);
            if (user == null) return;
            if (!user.UnlockedUpgrades.Contains(upgradeId))
                user.UnlockedUpgrades.Add(upgradeId);
            SaveDatabase();
        }

        public bool IsUpgradeOwned(string email, string upgradeId)
        {
            var user = FindByEmail(email);
            return user?.UnlockedUpgrades.Contains(upgradeId) ?? false;
        }
    }
}
