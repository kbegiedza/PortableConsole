using System.Collections.Generic;
using UnityEngine;

namespace PortableConsole
{
    /// <summary>
    /// Pool pattern for <see cref="GameObject"/>s without MonoBehaviour inheritance.
    /// </summary>
    public class GameObjectPool
    {
        /// <summary>
        /// Can pool grow?
        /// </summary>
        public bool Growable { get; private set; }

        /// <summary>
        /// Pooled prefab - <see cref="GameObject"/>
        /// </summary>
        public GameObject Prefab { get; set; }

        /// <summary>
        /// Capacity of this pool
        /// </summary>
        public int Capacity;

        private Transform _parent;
        private IList<GameObject> _pooledObjects;

        //------------------------------
        // public methods
        //------------------------------
        /// <summary>
        /// Creates new <see cref="GameObjectPool"/> instance with pre-created objects in pool.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="poolSize"></param>
        /// <param name="growable"></param>
        public GameObjectPool(GameObject prefab, Transform parent, int poolSize, bool growable = true)
        {
            Growable = growable;
            Capacity = poolSize;
            Prefab = prefab;

            _parent = parent;
            _pooledObjects = new List<GameObject>();

            Init();
        }

        /// <summary>
        /// Returns next not used, pooled <see cref="GameObject"/>. 
        /// If every GameObject is in use, and is not growable, returns null.
        /// </summary>
        /// <returns></returns>
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
        
        /// <summary>
        /// Pre-creates pooled <see cref="GameObject"/>s.
        /// </summary>
        private void Init()
        {
            for (int i = 0; i < Capacity; ++i)
            {
                AddGameObjectToPool();
            }
        }

        /// <summary>
        /// Resizes pool to newSize and fill it with new <see cref="GameObject"/>s.
        /// </summary>
        /// <param name="newSize"></param>
        private void ResizePool(int newSize)
        {
            for (int i = Capacity; i < newSize; ++i)
            {
                AddGameObjectToPool();
            }

            Capacity = newSize;
        }

        /// <summary>
        /// Adds new <see cref="GameObject"/> to pool.
        /// </summary>
        /// <returns></returns>
        private GameObject AddGameObjectToPool()
        {
            var newItem = Object.Instantiate(Prefab, _parent);
            newItem.SetActive(false);
            _pooledObjects.Add(newItem);

            return newItem;
        }
    }
}