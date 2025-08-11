namespace HDU.Managers
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Cysharp.Threading.Tasks;

    public class SceneManagerEx : IManager
    {
        public IScene CurrentScene { get; private set; }
        public Define.CoreDefine.ESceneType CurrentSceneType { get; private set; }
        public Define.CoreDefine.ESceneType NextScene { get; private set; }

        private bool _isLoading = false;

        public SceneManagerEx()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        ///  초기화
        /// </summary>
        public void Clear()
        {
            CurrentScene?.Clear();
        }

        public void Init()
        {

        }

        /// <summary>
        /// Scene을 로드한다.
        /// </summary>
        /// <param name="type"> 로드할 Scene의 타입 </param>
        public void LoadScene(Define.CoreDefine.ESceneType type)
        {
            if (_isLoading)
                return;

            _isLoading = true;
            NextScene = type;
            Managers.Clear();

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GetSceneName(type));
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _isLoading = false;
            CurrentSceneType = NextScene;

            //CurrentScene = GameObject.FindAnyObjectByType<BaseScene>();

            CurrentSceneInit().Forget();

            if (CurrentScene == null)
            {
                Debug.LogError($"BaseScene not found in loaded scene: {scene.name}");
                return;
            }
        }

        private async UniTask CurrentSceneInit()
        {
            await UniTask.WaitUntil(() => CurrentScene != null);
            CurrentScene.Init();
        }

        /// <summary>
        /// 해당 씬의 이름을 가져온다.
        /// </summary>
        /// <param name="type"> 가져올 Scene의 타입 </param>
        /// <returns> Scene 이름의 string </returns>
        public string GetSceneName(Define.CoreDefine.ESceneType type)
        {
            return System.Enum.GetName(typeof(Define.CoreDefine.ESceneType), type);
        }
        public void SetCurrentScene(IScene scene)
        {
            CurrentScene = scene;
        }
    }
}