using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_Base : MonoBehaviour , IUI
{
    [Header("UI Base Variable")]
    [SerializeField] protected CanvasScaler _canvasScaler;

    protected virtual void AwakeInit()
    {
        if (_canvasScaler != null)
        {
            float ratio = (float)Screen.width / (float)Screen.height;
            if (ratio < 0.6f)
                _canvasScaler.matchWidthOrHeight = 0;
            else
                _canvasScaler.matchWidthOrHeight = 1;
        }
    }

    protected virtual void Init() { }

    protected void Awake()
    {
        AwakeInit();
    }

    protected void Start()
    {
        Init();
    }

    public static void AddUIEvent(GameObject go, UnityAction<PointerEventData> action, HDU.Define.CoreDefine.EUIEventType type)
    {
        UI_EventHandler evt = HDU.Utils.UnityUtils.GetOrAddComponent<UI_EventHandler>(go);
        evt.AddEvent(type, action);
    }

    public static void RemoveUIEvent(GameObject go, UnityAction<PointerEventData> action, HDU.Define.CoreDefine.EUIEventType type)
    {
        UI_EventHandler evt = go.GetComponent<UI_EventHandler>();
        if (evt != null)
            evt.RemoveEvent(type, action);
    }
}
