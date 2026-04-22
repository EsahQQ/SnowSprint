using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Bootstrap.InitPipeline.Tasks.GlobalTasks
{
    public class GoodFpsTask : IInitTask
    {
        public int Priority => GlobalInitPriority.GoodFps;
        public UniTask Execute()
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
            return UniTask.CompletedTask;
        }
    }
}