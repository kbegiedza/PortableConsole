using System.Collections.Generic;
using UnityEngine;

namespace PortableConsole
{
    public class GameObjectPool
    {
        public bool Growable { get; private set; }
        public GameObject Prefab { get; set; }
        public int Capacity;

        private Transform _parent;

        private IList<GameObject> _pooledObjects;
        //------------------------------
        // public methods
        //------------------------------
        public GameObjectPool(GameObject prefab, Transform parent, int poolSize, bool growable = true)
        {
            Growable = growable;
            Capacity = poolSize;
            Prefab = prefab;

            _parent = parent;
            _pooledObjects = new List<GameObject>();

            Init();
        }

        public GameObject GetGameObject()
        {
            foreach (var obj in _pooledObjects)
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }

            if (Growable)
            {
                return AddGameObjectToPool();
            }

            return null;
        }
        //------------------------------
        // private methods
        //------------------------------
        private void Init()
        {
            for (int i = 0; i < Capacity; ++i)
            {
                AddGameObjectToPool();
            }
        }

        private void ResizePool(int newSize)
        {
            for (int i = Capacity; i < newSize; ++i)
            {
                AddGameObjectToPool();
            }

            Capacity = newSize;
        }

        private GameObject AddGameObjectToPool()
        {
            var newItem = Object.Instantiate(Prefab, _parent);
            newItem.SetActive(false);
            _pooledObjects.Add(newItem);

            return newItem;
        }
    }
}