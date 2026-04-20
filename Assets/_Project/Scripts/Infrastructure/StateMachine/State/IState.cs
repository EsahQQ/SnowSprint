using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.StateMachine.State
{
    public interface IState
    {
        UniTask OnEnter();
        UniTask OnExit();
        void Update(float dt);
        void Reset();
    }
}