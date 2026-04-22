using UnityEngine;

namespace _Project.Scripts.Features.UI.HUD
{
    public class HudView : MonoBehaviour, IHudView
    {
        [SerializeField] private GameObject _hudPanel;
        
        public void Show() => _hudPanel.SetActive(true);
        public void Hide() => _hudPanel.SetActive(false);
    }
}