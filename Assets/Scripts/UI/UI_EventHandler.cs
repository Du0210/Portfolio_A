using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HDU.UI
{
    public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler
    , IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Serializable]
        public class UIEventEntry
        {
            public HDU.Define.CoreDefine.EUIEventType EventType;
            public UnityEvent<PointerEventData> callback;
        }

        [SerializeField] private List<UIEventEntry> UIEventList = new List<UIEventEntry>();
        [SerializeField] private Dictionary<HDU.Define.CoreDefine.EUIEventType, UnityEvent<PointerEventData>> _eventTable = new();


        protected virtual void Awake()
        {
            foreach (var item in UIEventList)
            {
                if (item == null || item.callback == null) continue;
                _eventTable.Add(item.EventType, item.callback);
            }
        }

        public void AddEvent(HDU.Define.CoreDefine.EUIEventType type, UnityAction<PointerEventData> action)
        {
            if (!_eventTable.TryGetValue(type, out var evt) || evt == null)
            {
                evt = new UnityEvent<PointerEventData>();
                _eventTable.Add(type, evt);
            }

            evt.RemoveListener(action);
            evt.AddListener(action);
        }

        public void RemoveEvent(HDU.Define.CoreDefine.EUIEventType type, UnityAction<PointerEventData> action)
        {
            if (_eventTable.TryGetValue(type, out var evt))
                evt.RemoveListener(action);
        }

        private void Invoke(HDU.Define.CoreDefine.EUIEventType type, PointerEventData eventData)
        {
            if (_eventTable.TryGetValue(type, out var handler))
                handler?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                Invoke(HDU.Define.CoreDefine.EUIEventType.LClick, eventData);
            else if (eventData.button == PointerEventData.InputButton.Right)
                Invoke(HDU.Define.CoreDefine.EUIEventType.RClick, eventData);
        }

        public virtual void OnPointerDown(PointerEventData eventData) =>
            Invoke(HDU.Define.CoreDefine.EUIEventType.LClickDown, eventData);

        public virtual void OnPointerUp(PointerEventData eventData) =>
            Invoke(HDU.Define.CoreDefine.EUIEventType.LClickUp, eventData);

        public virtual void OnBeginDrag(PointerEventData eventData) =>
            Invoke(HDU.Define.CoreDefine.EUIEventType.BeginDrag, eventData);

        public virtual void OnDrag(PointerEventData eventData) =>
            Invoke(HDU.Define.CoreDefine.EUIEventType.Drag, eventData);

        public virtual void OnEndDrag(PointerEventData eventData) =>
            Invoke(HDU.Define.CoreDefine.EUIEventType.EndDrag, eventData);

        public virtual void OnDrop(PointerEventData eventData) =>
            Invoke(HDU.Define.CoreDefine.EUIEventType.DragDrop, eventData);

        public virtual void OnPointerEnter(PointerEventData eventData) =>
            Invoke(HDU.Define.CoreDefine.EUIEventType.PointerEnter, eventData);

        public virtual void OnPointerExit(PointerEventData eventData) =>
            Invoke(HDU.Define.CoreDefine.EUIEventType.PointerExit, eventData);
    }
}