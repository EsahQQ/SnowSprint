using UnityEngine;
using Zenject;

namespace _Project.Scripts.Libs.Factories
{
    public class ZenjectPrefabFactory<T> : IFactory<T> where T : Component
    {
        private readonly IInstantiator _instantiator;
        private readonly T _prefab;
        private readonly Transform _parent;
   
        public ZenjectPrefabFactory(IInstantiator instantiator, T prefab, Transform parent = null)
        {
            _instantiator = instantiator;
            _prefab = prefab;
            _parent = parent;
        }

        public T Create() => _instantiator.InstantiatePrefabForComponent<T>(_prefab, _parent);
    }
}