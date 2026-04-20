using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Bootstrap.InitPipeline.Tasks.GlobalTasks
{
    public class LoadLevelsTask : IInitTask
    {
        public int Priority => GlobalInitPriority.LoadLevels; 
        
        public UniTask Execute()
        {
            Debug.Log("Load Levels...");
            return UniTask.Delay(1000);
        }
    }
}