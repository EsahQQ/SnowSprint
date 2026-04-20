using _Project.Scripts.Bootstrap.InitPipeline.Queue;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Bootstrap.InitPipeline.Initializers.Global
{
    public class GlobalInitializer : IGlobalInitializer
    {
        private readonly IInitQueue _initQueue;
        private bool _isInitialized;

        public GlobalInitializer(IInitQueue initQueue)
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