using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Infrastructure.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        public async UniTask Load(string sceneName, CancellationToken cancellationToken = default)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);

            await using var registration = cancellationToken.Register(() =>
            {
                if (asyncOperation == null) return;
                asyncOperation.allowSceneActivation = false;
                Debug.LogWarning($"Scene {sceneName} loading was canceled.");
            });
            
            await asyncOperation.ToUniTask(cancellationToken: cancellationToken);
        }
    }
}