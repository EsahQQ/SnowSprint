using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Menu.Panels
{
    [Serializable]
    public struct ProfilePanel
    {
        public GameObject Root;
        public TextMeshProUGUI UsernameText;
        public Button LogoutButton;
    }
}