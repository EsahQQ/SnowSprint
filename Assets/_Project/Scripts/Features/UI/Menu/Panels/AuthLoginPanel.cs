using System;
using TMPro;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Menu.Panels
{
    [Serializable]
    public struct AuthLoginPanel
    {
        public TMP_InputField UsernameInput;
        public TMP_InputField PasswordInput;
        public Button SubmitButton;
        public Button GoToRegisterButton;
        public Button CloseButton;
    }
}