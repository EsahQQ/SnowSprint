using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Editor
{
    public class CreateScriptableObjectWindow : EditorWindow
    {
        const string MenuPath = "Assets/Create Scriptable Object";
        const int MenuPriority = -9999;
        string _targetFolderPath;
        List<Type> _scriptableObjectTypes;
        int _selectedIndex = -1;
        string _searchFilter = "";
        private bool _isFilterAssetsObject = true;
        TreeViewState _treeViewState;
        ScriptableObjectTypeTreeView _treeView;
        string _lastSearchFilter;
        int _lastTypesCount = -1;
        public static void Open(string targetFolderPath)
        {
            var window = GetWindow<CreateScriptableObjectWindow>(true, "Create Scriptable Object", true);
            window._targetFolderPath = targetFolderPath ?? "Assets";
            window._searchFilter = "";
            window.minSize = new Vector2(320, 400);
            window.UpdateListTypes();
            window._isFilterAssetsObject = true;
            window._selectedIndex = window._scriptableObjectTypes.Count > 0 ? 0 : -1;
            window._lastSearchFilter = null;
            window._lastTypesCount = -1;
            window.EnsureTreeView();
        }
        [MenuItem(MenuPath, false, MenuPriority)]
        static void MenuCreateScriptableObject()
        {
            string folder = GetSelectedFolderAssetPath();
            Open(folder);
        }
        static string GetSelectedFolderAssetPath()
        {
            var selected = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if (selected == null || selected.Length == 0)
                return "Assets";
            foreach (var obj in selected)
            {
                string relativePath = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(relativePath)) continue;
                string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", relativePath));
                if (Directory.Exists(fullPath))
                    return relativePath;
                string dir = Path.GetDirectoryName(relativePath).Replace("\\", "/");
                if (!string.IsNullOrEmpty(dir))
                    return dir;
            }
            return "Assets";
        }
        static List<Type> CollectScriptableObjectTypes()
        {
            var result = new List<Type>();
            var ignoreDerived = new List<Type>
            {
                typeof(EditorWindow),
                typeof(UnityEditor.Editor)
            };
#if UNITY_2020_1_OR_NEWER
            var types = TypeCache.GetTypesDerivedFrom<ScriptableObject>();
            
            
            foreach (var t in types)
            {
                bool isFromScip = ignoreDerived.Any(x => x.IsAssignableFrom(t));
                if (t != null && t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition && isFromScip is false)
                    result.Add(t);
            }
#else
            var scriptableType = typeof(ScriptableObject);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string name = assembly.GetName().Name;
                if (name.StartsWith("Unity.") || name.StartsWith("UnityEngine") ||
                    name.StartsWith("UnityEditor") || name.StartsWith("System") ||
                    name.StartsWith("mscorlib") || name.StartsWith("netstandard"))
                    continue;
                try
                {
                    foreach (var t in assembly.GetExportedTypes())
                    {
                        if (t != null && t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition &&
                            scriptableType.IsAssignableFrom(t))
                            result.Add(t);
                    }
                }
                catch (ReflectionTypeLoadException) { }
            }
#endif
            result.Sort((a, b) => string.CompareOrdinal(a.FullName, b.FullName));
            return result;
        }
        
        void OnGUI()
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Folder: " + _targetFolderPath, EditorStyles.miniLabel);
            EditorGUILayout.Space(4);
            _searchFilter = EditorGUILayout.TextField("Search", _searchFilter);
            EditorGUILayout.Space(2);
            var nextFilter =  EditorGUILayout.Toggle("filter assets", _isFilterAssetsObject);
            bool changed = _isFilterAssetsObject != nextFilter;
            _isFilterAssetsObject = nextFilter;
            
            if (changed)
            {
                UpdateListTypes();
            }
            EditorGUILayout.Space(2);
     
            
            EnsureTreeView();
            bool dataChanged = _searchFilter != _lastSearchFilter ||
                (_scriptableObjectTypes != null && _scriptableObjectTypes.Count != _lastTypesCount);
            if (dataChanged)
            {
                _lastSearchFilter = _searchFilter;
                _lastTypesCount = _scriptableObjectTypes?.Count ?? 0;
                _treeView.SetData(_scriptableObjectTypes, _searchFilter);
                _treeView.SetSelectedType(_selectedIndex >= 0 && _selectedIndex < _scriptableObjectTypes.Count
                    ? _scriptableObjectTypes[_selectedIndex] : null);
                _treeView.Reload();
            }
            else
            {
                _treeView.SetSelectedType(_selectedIndex >= 0 && _selectedIndex < _scriptableObjectTypes.Count
                    ? _scriptableObjectTypes[_selectedIndex] : null);
            }
            var treeRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandHeight(true));
            if (treeRect.height > 0)
                _treeView.OnGUI(treeRect);
            var treeSelectedType = _treeView.GetSelectedType();
            _selectedIndex = treeSelectedType != null ? _scriptableObjectTypes.IndexOf(treeSelectedType) : -1;
            EditorGUILayout.Space(8);
            GUI.enabled = _selectedIndex >= 0 && _selectedIndex < _scriptableObjectTypes.Count;
            if (GUILayout.Button("Create", GUILayout.Height(28)))
                CreateSelected();
            GUI.enabled = true;
        }
        void EnsureTreeView()
        {
            if (_treeViewState == null)
                _treeViewState = new TreeViewState();
            if (_treeView == null)
                _treeView = new ScriptableObjectTypeTreeView(_treeViewState);
        }
        private void UpdateListTypes()
        {
            _scriptableObjectTypes = CollectScriptableObjectTypes();
            if (_isFilterAssetsObject)
            {
                _scriptableObjectTypes = FilterAssets(_scriptableObjectTypes);
            }
        }
        private List<Type> FilterAssets(List<Type> scriptableObjectTypes)
        {
            string projectPath = Application.dataPath.Replace("/Assets", "");
            Debug.Log($"scan for {projectPath}");
            foreach (Type scriptableObjectType in scriptableObjectTypes)
            {
                Debug.Log(scriptableObjectType.Assembly.CodeBase);
            }
            
            return scriptableObjectTypes.Where(x => x.Assembly.CodeBase.Contains(projectPath)).ToList();
        }
        static string GetTypeDisplayName(Type t)
        {
            if (string.IsNullOrEmpty(t.FullName)) return t.Name;
            return t.FullName;
        }
        void CreateSelected()
        {
            if (_selectedIndex < 0 || _selectedIndex >= _scriptableObjectTypes.Count) return;
            Type type = _scriptableObjectTypes[_selectedIndex];
            ScriptableObject instance = CreateInstance(type);
            if (instance == null)
            {
                EditorUtility.DisplayDialog("Error", "Could not create instance of " + type.Name, "OK");
                return;
            }
            string folder = _targetFolderPath ?? "Assets";
            string fileName = type.Name + ".asset";
            string path = folder.EndsWith("/") ? folder + fileName : folder + "/" + fileName;
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = instance;
            EditorGUIUtility.PingObject(instance);
            Close();
        }
    }
    sealed class ScriptableObjectTypeTreeView : TreeView
    {
        List<Type> _allTypes;
        List<Type> _filteredTypes;
        Type _selectedType;
        int _nextNamespaceId = -2;
        Dictionary<string, int> _namespacePathToId;
        public ScriptableObjectTypeTreeView(TreeViewState state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;
        }
        public void SetData(List<Type> allTypes, string searchFilter)
        {
            _allTypes = allTypes;
            _filteredTypes = string.IsNullOrWhiteSpace(searchFilter)
                ? new List<Type>(allTypes)
                : allTypes.Where(t =>
                    t.Name.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (t.FullName != null && t.FullName.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
        }
        public void SetSelectedType(Type t) => _selectedType = t;
        public Type GetSelectedType() => _selectedType;
        protected override TreeViewItem BuildRoot()
        {
            _nextNamespaceId = -2;
            _namespacePathToId = new Dictionary<string, int>(StringComparer.Ordinal);
            var root = new TreeViewItem(-1, -1, "Root");
            var nodeRoot = new NsNode("");
            foreach (Type type in _filteredTypes)
            {
                int typeId = _allTypes.IndexOf(type);
                if (typeId < 0) continue;
                string fullName = type.FullName ?? type.Name;
                string[] parts = fullName.Split('.');
                string typeName = parts[parts.Length - 1];
                NsNode current = nodeRoot;
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    string segment = parts[i];
                    if (!current.Children.TryGetValue(segment, out var next))
                    {
                        next = new NsNode(segment);
                        current.Children[segment] = next;
                    }
                    current = next;
                }
                if (!current.Types.ContainsKey(typeName))
                    current.Types[typeName] = typeId;
            }
            root.children = BuildTreeViewChildren(nodeRoot, "", 0);
            ExpandPathToSelected();
            if (_selectedType != null)
            {
                int idx = _allTypes.IndexOf(_selectedType);
                if (idx >= 0)
                    state.selectedIDs = new List<int> { idx };
            }
            return root;
        }
        List<TreeViewItem> BuildTreeViewChildren(NsNode node, string currentPath, int depth)
        {
            var list = new List<TreeViewItem>();
            foreach (var kv in node.Children.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            {
                int id = _nextNamespaceId--;
                string path = string.IsNullOrEmpty(currentPath) ? kv.Key : currentPath + "." + kv.Key;
                _namespacePathToId[path] = id;
                var item = new TreeViewItem(id, depth, kv.Key) { displayName = kv.Key };
                item.children = BuildTreeViewChildren(kv.Value, path, depth + 1);
                if (item.children != null && item.children.Count == 0)
                    item.children = null;
                list.Add(item);
            }
            foreach (var kv in node.Types.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
                list.Add(new TreeViewItem(kv.Value, depth, kv.Key) { displayName = kv.Key });
            return list.Count > 0 ? list : null;
        }
        void ExpandPathToSelected()
        {
            if (_selectedType == null || _namespacePathToId == null) return;
            string fullName = _selectedType.FullName ?? _selectedType.Name;
            int lastDot = fullName.LastIndexOf('.');
            if (lastDot <= 0) return;
            string nsPath = fullName.Substring(0, lastDot);
            string[] parts = nsPath.Split('.');
            string prefix = "";
            foreach (string part in parts)
            {
                prefix = string.IsNullOrEmpty(prefix) ? part : prefix + "." + part;
                if (_namespacePathToId.TryGetValue(prefix, out int id))
                    state.expandedIDs.Add(id);
            }
        }
        sealed class NsNode
        {
            public string Name;
            public Dictionary<string, NsNode> Children = new Dictionary<string, NsNode>(StringComparer.Ordinal);
            public Dictionary<string, int> Types = new Dictionary<string, int>(StringComparer.Ordinal);
            public NsNode(string name) { Name = name; }
        }
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            if (selectedIds == null || selectedIds.Count == 0)
            {
                _selectedType = null;
                return;
            }
            int id = selectedIds[0];
            if (id >= 0 && id < _allTypes.Count)
                _selectedType = _allTypes[id];
            else
                _selectedType = null;
        }
        protected override bool CanMultiSelect(TreeViewItem item) => false;
    }
}