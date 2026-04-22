using System.Collections.ObjectModel;
namespace _Project.Scripts.Features.SceneConstants
{
    public static class SceneNames
    {
        public const string MainMenu = "MainMenu";
        public const string Game = "Game";
        public const string LobbyMenu = "LobbyMenu";

        public static readonly ReadOnlyCollection<string> All = new ReadOnlyCollection<string>(new[]
        {
            MainMenu,
            Game,
            LobbyMenu,
        });
    }
}
