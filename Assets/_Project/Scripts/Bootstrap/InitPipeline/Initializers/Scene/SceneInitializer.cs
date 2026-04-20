using _Project.Scripts.Bootstrap.InitPipeline.Queue;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Bootstrap.InitPipeline.Initializers.Scene
{
    public class SceneInitializer : ISceneInitializer
    {
        private readonly IInitQueue _initQueue;
        private bool _isInitialized;

        public SceneInitializer(IInitQueue initQueue)
        {
            _initQueue =  initQueue;
        }
        
        public async UniTask<bool> EnsureInitializedAsync()
        {
            if (_isInitialized) return false;
            
            _isInitialized = true;

            await _initQueue.RunAsync();
            return true;
        }
    }
}