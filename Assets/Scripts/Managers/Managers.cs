namespace HDU.Managers
{
    using System;
    using UnityEngine;

    public class Managers : MonoBehaviour
    {
        static Managers _instance;

        public static Managers Instance
        {
            get
            {
                Init();
                return _instance;
            }
        }

        public static GameManager Game { get => Instance._game; }
        public static ResourceManager Resource { get => Instance._resource; }
        public static AudioManager Audio { get => Instance._audio; }
        public static PoolManager Pool { get => Instance._pool; }
        public static UIManager UI { get => Instance._ui; }
        public static EventManager Event { get =>  Instance._event; }
        public static TimeManager Time { get => Instance._time; }
        public static SaveManager Save { get => Instance._save; }
        public static ClickManager2D Click { get => Instance._click; }
        public static SceneManagerEx Scene { get => Instance._scene; }

        public static bool IsQuitApplication { get => _isQuitApplication; }

        private static bool _isInit = false;
        private static bool _isQuitApplication = false;

        #region Core Manager
        private GameManager _game = new GameManager();
        private ResourceManager _resource = new ResourceManager();
        private PoolManager _pool = new PoolManager();
        private AudioManager _audio = new AudioManager();
        private UIManager _ui = new UIManager();
        private EventManager _event = new EventManager();
        private TimeManager _time = new TimeManager();
        private SaveManager _save = new SaveManager();
        private ClickManager2D _click = new ClickManager2D();
        private SceneManagerEx _scene = new SceneManagerEx();
        #endregion

        public static void Init()
        {
            if (_instance != null)
                return;

            if (_isQuitApplication || _isInit)
                return;

            GameObject go = GameObject.Find("#Managers");
            if (go == null)
            {
                go = new GameObject { name = "#Managers" };
                go.AddComponent<Managers>();
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();

            _instance._game.Init();
            _instance._resource.Init();
            _instance._audio.Init();
            _instance._pool.Init();
            _instance._ui.Init();
            _instance._event.Init();
            _instance._time.Init();
            _instance._save.Init();
            _instance._click.Init();
            _instance._scene.Init();

            _isInit = true;
        }

        public static void Clear()
        {
            _instance._game.Clear();
            _instance._resource.Clear();
            _instance._audio.Clear();
            _instance._pool.Clear();
            _instance._ui.Clear();
            _instance._event.Clear();
            _instance._time.Clear();
            _instance._save.Clear();
            _instance._click.Clear();
            _instance._scene.Clear();

            _isInit = false;
        }

        private void Update()
        {
            _time.Update();
            _click.Update();
        }

        private void OnApplicationPause(bool pause)
        {
            _save.OnApplicationPause(pause);
        }

        private void OnApplicationQuit()
        {
            _isQuitApplication = true;
            _save.OnApplicationQuit();
        }

        private void OnDestroy()
        {
            if (_instance != null)
                _isQuitApplication = true;
        }
    }
}