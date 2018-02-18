using System.Collections.Generic;
using UnityEngine;

namespace PortableConsole
{
    public class GameObjectPool
    {
        public GameObject Prefab { get; set; }
        public int Capacity;

        private IList<GameObject> _pooledObjects;
        //------------------------------
        // public methods
        //------------------------------
        public GameObjectPool(GameObject prefab)
        {
            Prefab = prefab;
            _pooledObjects = new List<GameObject>();

            Init();
        }

        public GameObject GetGameObject()
        {
            foreach(var obj in _pooledObjects)
            {
                if(!obj.activeInHierarchy)
                {
                    return obj;
                }
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

        private void AddGameObjectToPool()
        {
            var newItem = Object.Instantiate(Prefab);
            newItem.SetActive(false);
            _pooledObjects.Add(newItem);
        }
    }
}