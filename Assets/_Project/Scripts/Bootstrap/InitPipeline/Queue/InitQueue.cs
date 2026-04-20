using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Bootstrap.InitPipeline.Tasks;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Project.Scripts.Bootstrap.InitPipeline.Queue
{
    public class InitQueue : IInitQueue
    {
        [Inject(Source = InjectSources.Local)]
        private readonly List<IInitTask> _tasks;

        public InitQueue(List<IInitTask> tasks)
        {
            _tasks = tasks;
        }

        public async UniTask RunAsync()
        {
            var groupedTasks = _tasks
                .GroupBy(t => t.Priority)
                .OrderBy(g => g.Key);

            foreach (var group in groupedTasks)
            {
                var tasksInGroup = group.Select(t => t.Execute());
                await UniTask.WhenAll(tasksInGroup);
            }
        }
    }
}