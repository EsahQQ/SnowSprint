using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Features.UI.HUD;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.AppStates.Gameplay
{
    public class GameplayLoopState : BaseFixedUpdateableState
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly LevelProgressView _levelProgressView;
        private readonly FinishTrigger _finishTrigger;
        private readonly IHudView _hudView;

        public GameplayLoopState(IStateMachine stateMachine, IPlayerProvider playerProvider, FinishTrigger finishTrigger,
            IHudView hudView, LevelProgressView levelProgressView) : base(stateMachine)
        {
            _playerProvider = playerProvider;
            _finishTrigger = finishTrigger;
            _hudView = hudView;
            _levelProgressView = levelProgressView;
        }

        public override UniTask OnEnter()
        {
            _hudView.Show();
            _finishTrigger.OnPlayerFinished += OnLevelFinished;
            
            foreach (var player in _playerProvider.AllPlayers)
                InitializeAndActivatePlayer(player);
            
            _playerProvider.OnAnyPlayerRegistered += InitializeAndActivatePlayer;
            
            WaitAndInitProgress().Forget();
            
            return UniTask.CompletedTask;
        }
        private async UniTaskVoid WaitAndInitProgress()
        {
            await UniTask.WaitUntil(() => _playerProvider.LocalPlayer != null);
            _levelProgressView.Init();
        }

        private void InitializeAndActivatePlayer(PlayerController player)
        {
            player.Initialize();
            
            
            if (Unity.Netcode.NetworkManager.Singleton.IsServer)
                player.SetActive(true);
        }

        public override void Update(float dt)
        {
            foreach (var player in _playerProvider.AllPlayers)
                player.Tick();

            _levelProgressView.Update();
        }

        public override void FixedUpdate(float fixedDt)
        {
            foreach (var player in _playerProvider.AllPlayers)
                player.FixedTick(fixedDt);
        }

        public override UniTask OnExit()
        {
            _finishTrigger.OnPlayerFinished -= OnLevelFinished;
            _playerProvider.OnAnyPlayerRegistered -= InitializeAndActivatePlayer; 
            
            if (Unity.Netcode.NetworkManager.Singleton.IsServer)
                foreach (var player in _playerProvider.AllPlayers)
                    player.SetActive(false);

            _hudView.Hide();
            return UniTask.CompletedTask;
        }

        private void OnLevelFinished() => StateMachine.RequestSwitchState<LevelFinishedState>();
    }
}