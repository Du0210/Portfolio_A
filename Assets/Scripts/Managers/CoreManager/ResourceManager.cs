namespace HDU.Managers
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using Cysharp.Threading.Tasks;
    
    public class ResourceManager : HDU.Interface.IManager
    {
        public void Clear()
        {

        }

        public void Init()
        {

        }

        public async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            if(typeof(T) == typeof(GameObject))
            {
                string name = key;
                int index = name.LastIndexOf('/');
                if (index >= 0)
                    name = name.Substring(index + 1);

                GameObject go = Managers.Pool.GetOriginal(name);
                if (go != null)
                    return go as T;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
                return handle.Result;

            Debug.LogError($"[ResourceManager] Failed to load asset : {key}");
            return null;
        }

        /// <summary>
        /// 동기 방식 로드
        /// </summary>
        public T Load<T>(string key) where T : Object
        {
            return LoadAsync<T>(key).GetAwaiter().GetResult();
        }

        public async UniTask<GameObject> InstantiateAsync(string key, Transform parent = null)
        {
            GameObject original = await LoadAsync<GameObject>(key);

            if (original == null)
            {
                Debug.LogError($"[ResourceManager] Failed to load prefab : {key}");
                return null;
            }

            if (original.GetComponent<Poolable>() != null)
                return Managers.Pool.Pop(original, parent).gameObject;

            GameObject go = Object.Instantiate(original, parent);
            go.name = original.name;
            return go;
        }

        /// <summary>
        /// 동기 방식 인스턴스화
        /// </summary>
        public GameObject Instantiate(string key, Transform parent = null)
        {
            GameObject original = Load<GameObject>(key);

            if(original == null)
            {
                Debug.LogError($"[ResourceManager] Prefab load failed : {key}");
                return null;
            }

            if (original.TryGetComponent<Poolable>(out var poolable))
                return Managers.Pool.Pop(original, parent).gameObject;

            GameObject go = Object.Instantiate(original, parent);
            go.name = original.name;
            return go;
        }

        public GameObject Instantiate(GameObject obj, Transform parent = null)
        {
            if(obj == null)
            {
                Debug.LogError("[ResourceManager] Instantiate failed : obj is null");
                return null;
            }

            if (obj.TryGetComponent<Poolable>(out var poolable))
                return Managers.Pool.Pop(obj, parent).gameObject;

            GameObject go =Object.Instantiate(obj, parent);
            go.name= obj.name;
            return go;
        }

        public void Destroy(GameObject go)
        {
            if (go == null)
                return;

            Poolable poolable = go.GetComponent<Poolable>();
            if (poolable != null)
            {
                Managers.Pool.Push(poolable);
                return;
            }

            Object.Destroy(go);
        }
    }
}   