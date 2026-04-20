using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates
{
    public class MainMenuState : BaseState
    {
        private readonly IMainMenuView _mainMenuView;

        public MainMenuState(IStateMachine stateMachine, IMainMenuView mainMenuView) : base(stateMachine)
        {
            _mainMenuView = mainMenuView;
        }
        
        public override async UniTask OnEnter()
        {
            Debug.Log("MainMenuState Enter");
            
            await _mainMenuView.ProcessMenuAsync();
            
            StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.Game);
        }
    }
}