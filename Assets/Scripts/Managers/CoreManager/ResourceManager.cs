namespace HDU.Managers
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using Cysharp.Threading.Tasks;
    using System.Collections.Generic;
    using static HDU.Define.CoreDefine;
    using UnityEngine.U2D;
    using HDU.Define;

    public class ResourceManager : HDU.Interface.IManager
    {
        public Dictionary<EUnitPrefabKey, GameObject> CachedPrefabs { get => _cachedPrefabs; }
        private readonly Dictionary<string, AsyncOperationHandle> _assetHandles = new Dictionary<string, AsyncOperationHandle>();
        private readonly Dictionary<CoreDefine.ELabelKey, AsyncOperationHandle> _labelHandles = new Dictionary<CoreDefine.ELabelKey, AsyncOperationHandle>();

        private Dictionary<EUnitPrefabKey, GameObject> _cachedPrefabs = new Dictionary<EUnitPrefabKey, GameObject>();
        private Dictionary<ESpriteKey, Sprite> _cachedSprite = new Dictionary<ESpriteKey, Sprite>();
        private Dictionary<EAtlasKey, SpriteAtlas> _cachedAtlas = new Dictionary<EAtlasKey, SpriteAtlas>();
        private Dictionary<ESoundKey, AudioClip> _cachedSound = new Dictionary<ESoundKey, AudioClip>();

        public void Clear()
        {
            foreach (var handle in _assetHandles)
                Addressables.Release(handle);
            foreach (var handle in _labelHandles)
                Addressables.Release(handle);

            _assetHandles.Clear();
            _labelHandles.Clear();

            _cachedAtlas.Clear();
            _cachedPrefabs.Clear();
            _cachedSound.Clear();
            _cachedSprite.Clear();
        }

        public void Init()
        {
            _assetHandles.Clear();
            _labelHandles.Clear();
        }

        #region ===================================== Getter =====================================
        public async UniTask<GameObject> GetCachedPrefabOrNull(EUnitPrefabKey key)
        {
            if (_cachedPrefabs.TryGetValue(key, out var prefab))
                return prefab;
            else
            { 
                GameObject go = await LoadAsync<GameObject>(key.ToString());
                if (go != null)
                    return go;

                Debug.LogError($"[ResourceManager] Prefab not cached: {key}");
                return null;
            }
        }

        public async UniTask<Sprite> GetCachedSpriteOrNull(ESpriteKey key)
        {
            if (_cachedSprite.TryGetValue(key, out var sprite))
                return sprite;
            else
            {
                Sprite sp = await LoadAsync<Sprite>(key.ToString());
                if (sp != null)
                    return sp;

                Debug.LogError($"[ResourceManager] Sprite not cached: {key}");
                return null;
            }
        }

        public async UniTask<SpriteAtlas> GetCachedAtlasOrNull(EAtlasKey key)
        {
            if (_cachedAtlas.TryGetValue(key, out var atlas))
                return atlas;
            else
            {
                SpriteAtlas at = await LoadAsync<SpriteAtlas>(key.ToString());
                if (at != null)
                    return at;

                Debug.LogError($"[ResourceManager] Atlas not cached: {key}");
                return null;
            }
        }

        public async UniTask<AudioClip> GetCachedSoundOrNull(ESoundKey key)
        {
            if (_cachedSound.TryGetValue(key, out var sound))
                return sound;
            else
            {
                AudioClip ac = await LoadAsync<AudioClip>(key.ToString());
                if (ac != null)
                    return ac;

                Debug.LogError($"[ResourceManager] Sound not cached: {key}");
                return null;
            }
        }
        #endregion

        #region ===================================== Local =========================================
        public async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            if (_assetHandles.TryGetValue(key, out var cachedHandle))
                return cachedHandle.Result as T;

            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _assetHandles[key] = handle;
                CachedData(handle.Result);
                return handle.Result;
            }

            Debug.LogError($"[ResourceManager] Failed to load asset : {key}");
            return null;
        }

        public T Load<T>(string key) where T : Object
        {
            return LoadAsync<T>(key).GetAwaiter().GetResult();
        }

        public async UniTask<GameObject> InstantiateLocalAsync(string key = null, GameObject go = null, Transform parent = null)
        {
            GameObject original = null;
            if (key != null)
                original = await LoadAsync<GameObject>(key);
            else
                original = go;
            return InstantiateInternal(original, parent);
        }

        public GameObject InstantiateLocal(string key = null, GameObject go = null, Transform parent = null)
        { 
            GameObject original = null;
            if (key != null)
                original = Load<GameObject>(key);
            else
                original = go;
            return InstantiateInternal(original, parent);
        }
        #endregion

        #region ===================================== Remote & Label =====================================
        public async UniTask PreloadLabelAsync(HDU.Define.CoreDefine.ELabelKey label)
        {
            if (_labelHandles.ContainsKey(label))
                return;

            long size = await Addressables.GetDownloadSizeAsync(label.ToString()); // 이미 다운로드된 리소스는 0 반환
            if (size > 0)
            {
                Debug.Log($"[ResourceManager] Downloading label: {label}, size = {size} bytes.");
                var handle = Addressables.DownloadDependenciesAsync(label.ToString());
                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                    _labelHandles[label] = handle;
                else
                    Debug.LogError($"[ResourceManager] Failed to download label: {label}");
            }
        }

        public async UniTask<List<Object>> LoadAssetsByLabelAsync(CoreDefine.ELabelKey label)
        {
            var result = new List<Object>();

            await Addressables.LoadAssetsAsync<Object>(label.ToString(), asset => {
                CachedData(asset);
                result.Add(asset);
            }).ToUniTask();

            return result;
        }

        public async UniTask<bool> IsLabelPreloaded(CoreDefine.ELabelKey label)
        {
            long size = await Addressables.GetDownloadSizeAsync(label.ToString());
            return size == 0;
        }
        #endregion

        #region ===================================== Internal =====================================
        private GameObject InstantiateInternal(GameObject original, Transform parent)
        {
            if (original == null) return null;

            if (original.TryGetComponent<Poolable>(out var pool))
                return Managers.Pool.Pop(original, parent).gameObject;

            GameObject go = Object.Instantiate(original, parent);
            go.name = original.name;
            return go;
        }
        #endregion

        private void CachedData<T>(T data)
        {
            switch (data)
            {
                case GameObject go:
                    {
                        EUnitPrefabKey key = HDU.Utils.CsUtils.StringToEnum<EUnitPrefabKey>(go.name);
                        if (key == default)
                        {
                            Debug.LogWarning($"[ResourceManager] Invalid prefab key: {go.name}");
                            return;
                        }
                        _cachedPrefabs[key] = go;
                        break;
                    }
                case Sprite sprite:
                    {
                        ESpriteKey key = HDU.Utils.CsUtils.StringToEnum<ESpriteKey>(sprite.name);
                        if (key == default)
                        {
                            Debug.LogWarning($"[ResourceManager] Invalid sprite key: {sprite.name}");
                            return;
                        }
                        _cachedSprite[key] = sprite;
                        break;
                    }
                case SpriteAtlas atlas:
                    {
                        EAtlasKey key = HDU.Utils.CsUtils.StringToEnum<EAtlasKey>(atlas.name);
                        if (key == default)
                        {
                            Debug.LogError($"[ResourceManager] Invalid atlas key: {atlas.name}");
                            return;
                        }
                        _cachedAtlas[key] = atlas;
                        break;
                    }
                case AudioClip clip:
                    {
                        ESoundKey key = HDU.Utils.CsUtils.StringToEnum<ESoundKey>(clip.name);
                        if (key == default)
                        {
                            Debug.LogError($"[ResourceManager] Invalid sound key: {clip.name}");
                            return;
                        }
                        _cachedSound[key] = clip;
                        break;
                    }
            }
        }

        public void ReleaseLabel(CoreDefine.ELabelKey label)
        {
            if(_labelHandles.TryGetValue(label, out var handle))
            {
                Addressables.Release(handle);
                _labelHandles.Remove(label);
            }
        }

        public void ReleaseAsset(string key)
        {
            if (_assetHandles.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _assetHandles.Remove(key);
            }
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