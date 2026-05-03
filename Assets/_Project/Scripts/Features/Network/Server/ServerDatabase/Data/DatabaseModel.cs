using System;
using System.Collections.Generic;

namespace _Project.Scripts.Features.Network.Server.ServerDatabase.Data
{
    [Serializable]
    public class DatabaseModel
    {
        public List<UserData> Users = new List<UserData>();
    }
}