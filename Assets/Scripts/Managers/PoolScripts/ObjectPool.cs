using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public GameObject Original { get; private set; }
    public Transform Root { get; set; }

    Queue<Poolable> _poolQueue = new Queue<Poolable>();

    public void Init(GameObject original, int count = 2)
    {
        Original = original;
        Root = new GameObject().transform;
        Root.name = $"{original.name}_Root";

        for (int i = 0; i < count; i++)
            Enqueue(Create());
    }

    Poolable Create()
    {
        GameObject go = Object.Instantiate<GameObject>(Original);
        go.name = Original.name;
        
        return go.GetOrAddComponent<Poolable>();
    }

    public void Enqueue(Poolable poolable)
    {
        if (poolable == null || _poolQueue.Contains(poolable))
            return;

        poolable.transform.SetParent(Root);

        poolable.gameObject.SetActive(false);
        poolable.IsUsing = false;

        _poolQueue.Enqueue(poolable);

    }

    public Poolable Dequeue(Transform parent)
    {
        Poolable poolable;

        if (_poolQueue.Count > 0)
        {
            poolable = _poolQueue.Dequeue();
        }
        else
            poolable = Create();

        poolable.gameObject.SetActive(true);

        if (parent == null)
            poolable.transform.SetParent(null);
        else
            poolable.transform.SetParent(parent);

        poolable.IsUsing = true;
        return poolable;
    }
}
