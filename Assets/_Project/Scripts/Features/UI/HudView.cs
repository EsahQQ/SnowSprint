using UnityEngine;

namespace _Project.Scripts.Features.UI
{
    public class HudView : MonoBehaviour, IHudView
    {
        [SerializeField] private GameObject hudPanel;
        
        public void Show()
        {
            hudPanel.SetActive(true);
        }

        public void Hide()
        {
            hudPanel.SetActive(false);
        }
    }
}