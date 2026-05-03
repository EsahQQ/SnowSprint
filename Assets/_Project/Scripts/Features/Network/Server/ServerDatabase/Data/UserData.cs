using System;

namespace _Project.Scripts.Features.Network.Server.ServerDatabase.Data
{
    [Serializable]
    public class UserData
    {
        public string Email;
        public string Username;
        public string PasswordHash;
        public string VerificationCode;
        public bool IsActivated;
        public int Coins;
    }
}