using System;
using TMPro;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Menu.Panels
{
    [Serializable]
    public struct AuthVerifyPanel
    {
        public TMP_InputField CodeInput;
        public Button SubmitButton;
        public Button CancelButton;
    }
}