using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace _Project.Scripts.Infrastructure.CodeGeneration.Editor
{
    public static class CodeGenerationUtility
    {
        public static void GenerateClass(string outputPath, List<string> keys, string @namespace, string className, string inheritance = "", params string[] usings)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.ObjectModel;");
            
            foreach (var @using in usings)
                sb.AppendLine($"using {@using};");
            
            if (usings.Length > 0) sb.AppendLine();

            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            
            var classDeclaration = $"    public static class {className}";
            if (!string.IsNullOrEmpty(inheritance)) classDeclaration += $" : {inheritance}";
            
            sb.AppendLine(classDeclaration);
            sb.AppendLine("    {");

            foreach (var key in keys)
                sb.AppendLine($"        public const string {key} = \"{key}\";");
            
            sb.AppendLine("\n        public static readonly ReadOnlyCollection<string> All = new ReadOnlyCollection<string>(new[]");
            sb.AppendLine("        {");
            foreach (var key in keys) sb.AppendLine($"            {key},");
            sb.AppendLine("        });");

            sb.AppendLine("    }");
            sb.AppendLine("}");

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        public static T FindAsset<T>() where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length == 0) return null;
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}