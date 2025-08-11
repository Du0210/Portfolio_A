namespace HDU.Managers
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PoolManager : IManager
    {
        Dictionary<string, ObjectPool> _pool = new Dictionary<string, ObjectPool>();
        Transform _root;

        public void Init()
        {
            if (_root == null)
            {
                _root = new GameObject { name = "@Pool_Root" }.transform;
                Object.DontDestroyOnLoad(_root);
            }
        }

        public void Clear()
        {
            foreach (Transform child in _root)
                GameObject.Destroy(child.gameObject);

            _pool.Clear();
        }

        public void CreatePool(GameObject original, int count = 2)
        {
            ObjectPool pool = new ObjectPool();
            pool.Init(original, count);
            pool.Root.SetParent(_root);

            _pool.Add(original.name, pool);
        }

        public void Push(Poolable poolable)
        {
            string name = poolable.gameObject.name;

            if (_pool.ContainsKey(name) == false)
            {
                GameObject.Destroy(poolable.gameObject);
                return;
            }

            _pool[name].Enqueue(poolable);
        }

        public Poolable Pop(GameObject original, Transform parent = null)
        {
            string name = original.name;

            if (_pool.ContainsKey(name) == false)
            {
                if (original.layer == LayerMask.NameToLayer("UI"))
                    CreatePool(original, 1);
                else
                    CreatePool(original);
            }

            return _pool[name].Dequeue(parent);
        }


        public GameObject GetOriginal(string name)
        {
            if (_pool.ContainsKey(name) == false)
                return null;

            return _pool[name].Original;
        }
    }
}