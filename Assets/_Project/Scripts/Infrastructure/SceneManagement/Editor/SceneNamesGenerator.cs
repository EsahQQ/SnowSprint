using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Infrastructure.CodeGeneration.Editor;
using UnityEditor;

namespace _Project.Scripts.Infrastructure.SceneManagement.Editor
{
    [InitializeOnLoad]
    public static class SceneNamesGenerator
    {
        private const string Path = "Assets/_Project/Scripts/Features/SceneConstants/SceneNames.cs";

        static SceneNamesGenerator() => EditorBuildSettings.sceneListChanged += Generate;

        [MenuItem("Tools/Generate Scene Names")]
        public static void Generate()
        {
            CodeGenerationUtility.GenerateClass(Path, EditorBuildSettings.scenes.Select(scene => System.IO.Path.GetFileNameWithoutExtension(scene.path)).ToList(),
                "_Project.Scripts.Features.SceneConstants", "SceneNames");
        }
    }
}