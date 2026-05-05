using _Project.Scripts.Features.Network.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.UI.Menu
{
    public class RoomListItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _roomNameText;
        [SerializeField] private TextMeshProUGUI _playerCountText;
        [SerializeField] private Button _joinButton;

        private System.Guid _roomId;
        private System.Action<System.Guid> _joinCallback;

        private void Start()
        {
            _joinButton.onClick.AddListener(OnJoinClicked);
        }

        public void Setup(RoomInfo roomInfo, System.Action<System.Guid> joinCallback)
        {
            _roomId = roomInfo.RoomId;
            _joinCallback = joinCallback;

            _roomNameText.text = roomInfo.RoomName;
            _playerCountText.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";

            // Блокируем кнопку, если комната полная
            _joinButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers;
        }

        private void OnJoinClicked()
        {
            _joinCallback?.Invoke(_roomId);
        }
    }
}