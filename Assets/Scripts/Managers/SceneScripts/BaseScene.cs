using HDU.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.InputSystem;

public abstract class BaseScene : MonoBehaviour
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