using System.Collections.ObjectModel;
namespace _Project.Scripts.Features.SceneConstants
{
    public static class SceneNames
    {
        public const string MainMenu = "MainMenu";
        public const string OnlineScene = "OnlineScene";

        public static readonly ReadOnlyCollection<string> All = new ReadOnlyCollection<string>(new[]
        {
            MainMenu,
            OnlineScene,
        });
    }
}
