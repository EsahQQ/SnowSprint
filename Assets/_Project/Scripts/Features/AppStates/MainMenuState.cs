using _Project.Scripts.Features.UI.Menu;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.AppStates
{
    public class MainMenuState : BaseState
    {
        private readonly IMainMenuView _view;

        public MainMenuState(IStateMachine stateMachine, IMainMenuView view) : base(stateMachine)
        {
            _view = view;
        }

        public override UniTask OnEnter()
        {
            return UniTask.CompletedTask;
        }

        public override UniTask OnExit()
        {
            return UniTask.CompletedTask;
        }
    }
}