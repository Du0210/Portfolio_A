using HDU.Interface;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HDU.Managers
{
    public abstract class BaseScene : MonoBehaviour, IScene
    {
        public HDU.Define.CoreDefine.ESceneType SceneType { get; protected set; } = HDU.Define.CoreDefine.ESceneType.None;

        public virtual void Init()
        {
            if (EventSystem.current == null)
            {
                GameObject obj = new GameObject("#EventSystem");
                obj.AddComponent<EventSystem>();
                obj.AddComponent<StandaloneInputModule>();

            }
        }

        public abstract void Clear();
    }
}