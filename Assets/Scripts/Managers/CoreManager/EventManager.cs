namespace HDU.Managers
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public class EventManager : IManager
    {
        private Dictionary<Define.CoreDefine.EEventType, Delegate> _dictEvents;

        public void Init()
        {
            _dictEvents = new Dictionary<Define.CoreDefine.EEventType, Delegate>();   
        }

        public void Clear()
        {
            _dictEvents.Clear();
        }


        #region ## Regist and Remove ##

        // Regist =====================================================================================
        private void AddDelegate(Define.CoreDefine.EEventType evtType, Delegate newDelegate)
        {
            if (_dictEvents.TryGetValue(evtType, out var existingDelegate))
                _dictEvents[evtType] = Delegate.Combine(existingDelegate, newDelegate);
            else
                _dictEvents[evtType] = newDelegate;
        }

        public void RegistEvent(Define.CoreDefine.EEventType evtType, Action action) => AddDelegate(evtType, action);
        public void RegistEvent<T>(Define.CoreDefine.EEventType evtType, Action<T> action) => AddDelegate(evtType, action);
        public void RegistEvent<T1, T2>(Define.CoreDefine.EEventType evtType, Action<T1,T2> action) => AddDelegate(evtType, action);
        public void RegistEvent<T1, T2, T3>(Define.CoreDefine.EEventType evtType, Action<T1, T2, T3> action) => AddDelegate(evtType, action);

        // Remove =====================================================================================
        private void RemoveDelegate(Define.CoreDefine.EEventType evtType, Delegate oldDelegate)
        {
            if(_dictEvents.TryGetValue(evtType, out var existingDelegate))
            {
                var newDelegate = Delegate.Remove(existingDelegate, oldDelegate);
                if (newDelegate == null)
                    _dictEvents.Remove(evtType);
                else
                    _dictEvents[evtType] = newDelegate;
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"등록된 이벤트 {evtType} 가 존재하지 않습니다!");
            }
#endif
        }

        public void RemoveEvent(Define.CoreDefine.EEventType evtType, Action action) => RemoveDelegate(evtType, action);

        public void RemoveEvent<T>(Define.CoreDefine.EEventType evtType, Action<T> action) => RemoveDelegate(evtType, action);

        public void RemoveEvent<T1, T2>(Define.CoreDefine.EEventType evtType, Action<T1, T2> action) => RemoveDelegate(evtType, action);

        public void RemoveEvent<T1, T2, T3>(Define.CoreDefine.EEventType evtType, Action<T1, T2, T3> action) => RemoveDelegate(evtType, action);

        #endregion


        #region ## Invoke ##
        public void InvokeEvent(Define.CoreDefine.EEventType evtType)
        {
            if (_dictEvents.TryGetValue(evtType, out var del))
            {
                if (del is Action action)
                    action.Invoke();
                else
                    LogTypeMismatch(evtType, del, typeof(Action));
            }
        }

        public void InvokeEvent<T>(Define.CoreDefine.EEventType evtType, T param)
        {
            if (_dictEvents.TryGetValue(evtType, out var del))
            {
                if (del is Action<T> action)
                    action.Invoke(param);
                else
                    LogTypeMismatch(evtType, del, typeof(Action<T>));
            }
        }

        public void InvokeEvent<T1, T2>(Define.CoreDefine.EEventType evtType, T1 param1, T2 param2)
        {
            if (_dictEvents.TryGetValue(evtType, out var del))
            {
                if (del is Action<T1, T2> action)
                    action.Invoke(param1, param2);
                else
                    LogTypeMismatch(evtType, del, typeof(Action<T1, T2>));
            }
        }

        public void InvokeEvent<T1, T2, T3>(Define.CoreDefine.EEventType evtType, T1 param1, T2 param2, T3 param3)
        {
            if (_dictEvents.TryGetValue(evtType, out var del))
            {
                if (del is Action<T1, T2, T3> action)
                    action.Invoke(param1, param2, param3);
                else
                    LogTypeMismatch(evtType, del, typeof(Action<T1, T2, T3>));
            }
        }

        private void LogTypeMismatch(Define.CoreDefine.EEventType evtType, Delegate del, Type expectedType)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[EventManager] 이벤트{evtType}는 델리게이트 타입{expectedType}와 일치하지 않습니다.\n등록된 타입: {del.GetType()}");
#endif
        }
        #endregion
    }
}