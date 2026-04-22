using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Network
{
    public class NetworkZenjectInjector : MonoBehaviour
    {
        private void Awake()
        {
            var sceneContext = FindObjectOfType<SceneContext>();
            if (sceneContext != null)
                sceneContext.Container.InjectGameObject(gameObject);
        }
    }
}