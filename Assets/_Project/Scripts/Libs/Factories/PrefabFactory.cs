using UnityEngine;

namespace _Project.Scripts.Libs.Factories
{
    public class PrefabFactory<T> : IFactory<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;

        public PrefabFactory(T prefab, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public T Create() => Object.Instantiate(_prefab, _parent);
    }
}