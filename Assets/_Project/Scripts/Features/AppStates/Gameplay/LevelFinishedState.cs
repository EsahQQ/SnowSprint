using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.Services;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Features.AppStates.Gameplay
{
    public class LevelFinishedState : BaseState
    {
        private readonly IPlayerDataService _playerData;
        private readonly IShopView _shopView;
        private const int LevelReward = 100; 

        public LevelFinishedState(IStateMachine stateMachine, IPlayerDataService playerData, IShopView shopView) 
            : base(stateMachine)
        {
            _playerData = playerData;
            _shopView = shopView;
        }

        public override async UniTask OnEnter()
        {
            Debug.Log("LevelFinishedState Enter");
            
            _playerData.AddCoins(LevelReward);
            
            await _shopView.ProcessShopAsync();
            
            StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.MainMenu);
        }
    }
}