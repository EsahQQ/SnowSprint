using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.SceneManagement
{
    public interface ISceneLoader
    {
        UniTask Load(string sceneName, CancellationToken cancellationToken = default);
    }
}