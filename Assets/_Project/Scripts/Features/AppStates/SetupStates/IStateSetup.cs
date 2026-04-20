using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.AppStates.SetupStates
{
    public interface IStateSetup
    {
        public UniTask Setup();
    }
}