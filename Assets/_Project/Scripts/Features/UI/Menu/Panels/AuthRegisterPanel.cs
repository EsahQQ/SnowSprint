using System;
using TMPro;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Menu.Panels
{
    [Serializable]
    public struct AuthRegisterPanel
    {
        public TMP_InputField UsernameInput;
        public TMP_InputField EmailInput;
        public TMP_InputField PasswordInput;
        public Button SubmitButton;
        public Button GoToLoginButton;
        public Button CloseButton;
    }
}