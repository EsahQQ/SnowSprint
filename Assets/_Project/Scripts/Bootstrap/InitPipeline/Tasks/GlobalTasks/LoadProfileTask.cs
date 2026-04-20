using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Bootstrap.InitPipeline.Tasks.GlobalTasks
{
    public class LoadProfileTask : IInitTask
    {
        public int Priority => GlobalInitPriority.LoadProfile; 
        
        public UniTask Execute()
        {
            Debug.Log("Load Profile...");
            return UniTask.Delay(1000);
        }
    }
}