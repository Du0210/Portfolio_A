using HDU.Define;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HDU.UI
{
    public static class UIExtensions
    {
        public static void AddUIEvent(this GameObject go, UnityAction<PointerEventData> action, CoreDefine.EUIEventType type = CoreDefine.EUIEventType.LClick)
        {
            UI_Base.AddUIEvent(go, action, type);
        }
        public static void RemoveUIEvent(this GameObject go, UnityAction<PointerEventData> action, CoreDefine.EUIEventType type = CoreDefine.EUIEventType.LClick)
        {
            UI_Base.RemoveUIEvent(go, action, type);
        }
    }
}