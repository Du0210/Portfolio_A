using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CodeDummy_EventBus
{
    // ======================== ListenerEntry 정의 ===========================
    private class ListenerEntry
    {
        public Delegate Callback;
        public int Priority;
        public bool Once;
    }

    private class ListenerEntry<T>
    {
        public Delegate Callback;
        public int Priority;
        public Predicate<T> Condition;
        public bool Once;
    }

    private class ListenerEntry<T1, T2>
    {
        public Delegate Callback;
        public int Priority;
        public Predicate<(T1, T2)> Condition;
        public bool Once;
    }

    private class ListenerEntry<T1, T2, T3>
    {
        public Delegate Callback;
        public int Priority;
        public Predicate<(T1, T2, T3)> Condition;
        public bool Once;
    }

    // ======================== 딕셔너리 ===========================
    private Dictionary<HDU.Define.CoreDefine.EEventType, List<ListenerEntry>> _zeroParamEvents = new();
    private Dictionary<HDU.Define.CoreDefine.EEventType, List<ListenerEntry<object>>> _oneParamEvents = new();
    private Dictionary<HDU.Define.CoreDefine.EEventType, List<ListenerEntry<object, object>>> _twoParamEvents = new();
    private Dictionary<HDU.Define.CoreDefine.EEventType, List<ListenerEntry<object, object, object>>> _threeParamEvents = new();

    private void OnDestroy()
    {
        _zeroParamEvents.Clear();
        _oneParamEvents.Clear();
        _twoParamEvents.Clear();
        _threeParamEvents.Clear();
    }

    // ======================== 등록 ===========================
    #region RegistEvent
    // 0 param
    public void RegistEvent(HDU.Define.CoreDefine.EEventType evt, Action callback, int priority = 0, bool once = false)
        => AddListener(_zeroParamEvents, evt, callback, priority, once);

    public void RegistEvent(HDU.Define.CoreDefine.EEventType evt, Func<UniTask> asyncCallback, int priority = 0, bool once = false)
        => AddListener(_zeroParamEvents, evt, asyncCallback, priority, once);

    // 1 param
    public void RegistEvent<T>(HDU.Define.CoreDefine.EEventType evt, Action<T> callback, int priority = 0, Predicate<T> condition = null, bool once = false)
        => AddListener(_oneParamEvents, evt, callback, priority, condition, once);

    public void RegistEvent<T>(HDU.Define.CoreDefine.EEventType evt, Func<T, UniTask> asyncCallback, int priority = 0, Predicate<T> condition = null, bool once = false)
        => AddListener(_oneParamEvents, evt, asyncCallback, priority, condition, once);

    // 2 param
    public void RegistEvent<T1, T2>(HDU.Define.CoreDefine.EEventType evt, Action<T1, T2> callback, int priority = 0, Predicate<(T1, T2)> condition = null, bool once = false)
        => AddListener(_twoParamEvents, evt, callback, priority, condition, once);

    public void RegistEvent<T1, T2>(HDU.Define.CoreDefine.EEventType evt, Func<T1, T2, UniTask> asyncCallback, int priority = 0, Predicate<(T1, T2)> condition = null, bool once = false)
        => AddListener(_twoParamEvents, evt, asyncCallback, priority, condition, once);

    // 3 param
    public void RegistEvent<T1, T2, T3>(HDU.Define.CoreDefine.EEventType evt, Action<T1, T2, T3> callback, int priority = 0, Predicate<(T1, T2, T3)> condition = null, bool once = false)
        => AddListener(_threeParamEvents, evt, callback, priority, condition, once);

    public void RegistEvent<T1, T2, T3>(HDU.Define.CoreDefine.EEventType evt, Func<T1, T2, T3, UniTask> asyncCallback, int priority = 0, Predicate<(T1, T2, T3)> condition = null, bool once = false)
        => AddListener(_threeParamEvents, evt, asyncCallback, priority, condition, once);

    #endregion

    // ======================== AddListener ===========================
    #region AddListener
    private void AddListener(Dictionary<HDU.Define.CoreDefine.EEventType, List<ListenerEntry>> dict, HDU.Define.CoreDefine.EEventType evt, Delegate callback, int priority, bool once)
    {
        if (!dict.TryGetValue(evt, out var list)) list = dict[evt] = new();
        list.Add(new ListenerEntry { Callback = callback, Priority = priority, Once = once });
        list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    private void AddListener<T>(Dictionary<HDU.Define.CoreDefine.EEventType, List<ListenerEntry<object>>> dict, HDU.Define.CoreDefine.EEventType evt, Delegate callback, int priority, Predicate<T> cond, bool once)
    {
        if (!dict.TryGetValue(evt, out var list)) list = dict[evt] = new();
        list.Add(new ListenerEntry<object> { Callback = callback, Priority = priority, Condition = cond == null ? null : (o => cond((T)o)), Once = once });
        list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    private void AddListener<T1, T2>(Dictionary<HDU.Define.CoreDefine.EEventType, List<ListenerEntry<object, object>>> dict, HDU.Define.CoreDefine.EEventType evt, Delegate callback, int priority, Predicate<(T1, T2)> cond, bool once)
    {
        if (!dict.TryGetValue(evt, out var list)) list = dict[evt] = new();
        list.Add(new ListenerEntry<object, object> { Callback = callback, Priority = priority, Condition = cond == null ? null : (p => cond(((T1)p.Item1, (T2)p.Item2))), Once = once });
        list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    private void AddListener<T1, T2, T3>(Dictionary<HDU.Define.CoreDefine.EEventType, List<ListenerEntry<object, object, object>>> dict, HDU.Define.CoreDefine.EEventType evt, Delegate callback, int priority, Predicate<(T1, T2, T3)> cond, bool once)
    {
        if (!dict.TryGetValue(evt, out var list)) list = dict[evt] = new();
        list.Add(new ListenerEntry<object, object, object> { Callback = callback, Priority = priority, Condition = cond == null ? null : (p => cond(((T1)p.Item1, (T2)p.Item2, (T3)p.Item3))), Once = once });
        list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }
    #endregion
    // ======================== RemoveListener ===========================
    #region ## RemoveEvent ##

    // 0 param
    public void RemoveEvent(HDU.Define.CoreDefine.EEventType evt, Delegate callback)
    {
        if (_zeroParamEvents.TryGetValue(evt, out var list))
            list.RemoveAll(e => e.Callback == callback);
    }

    // 1 param
    public void RemoveEvent<T>(HDU.Define.CoreDefine.EEventType evt, Delegate callback)
    {
        if (_oneParamEvents.TryGetValue(evt, out var list))
            list.RemoveAll(e => e.Callback == callback);
    }

    // 2 param
    public void RemoveEvent<T1, T2>(HDU.Define.CoreDefine.EEventType evt, Delegate callback)
    {
        if (_twoParamEvents.TryGetValue(evt, out var list))
            list.RemoveAll(e => e.Callback == callback);
    }

    // 3 param
    public void RemoveEvent<T1, T2, T3>(HDU.Define.CoreDefine.EEventType evt, Delegate callback)
    {
        if (_threeParamEvents.TryGetValue(evt, out var list))
            list.RemoveAll(e => e.Callback == callback);
    }

    #endregion

    // ======================== Invoke 동기 ===========================
    #region Invoke
    public void Invoke(HDU.Define.CoreDefine.EEventType evt)
    {
        if (!_zeroParamEvents.TryGetValue(evt, out var list)) return;
        var remove = new List<ListenerEntry>();
        foreach (var l in list)
        {
            if (l.Callback is Action a) a.Invoke();
            if (l.Once) remove.Add(l);
        }
        foreach (var r in remove) list.Remove(r);
    }

    public void Invoke<T>(HDU.Define.CoreDefine.EEventType evt, T param)
    {
        if (!_oneParamEvents.TryGetValue(evt, out var list)) return;
        var remove = new List<ListenerEntry<object>>();
        foreach (var l in list)
        {
            if (l.Condition != null && !l.Condition(param)) continue;
            if (l.Callback is Action<T> a) a.Invoke(param);
            if (l.Once) remove.Add(l);
        }
        foreach (var r in remove) list.Remove(r);
    }

    public void Invoke<T1, T2>(HDU.Define.CoreDefine.EEventType evt, T1 p1, T2 p2)
    {
        if (!_twoParamEvents.TryGetValue(evt, out var list)) return;
        var remove = new List<ListenerEntry<object, object>>();
        foreach (var l in list)
        {
            if (l.Condition != null && !l.Condition((p1, p2))) continue;
            if (l.Callback is Action<T1, T2> a) a.Invoke(p1, p2);
            if (l.Once) remove.Add(l);
        }
        foreach (var r in remove) list.Remove(r);
    }

    public void Invoke<T1, T2, T3>(HDU.Define.CoreDefine.EEventType evt, T1 p1, T2 p2, T3 p3)
    {
        if (!_threeParamEvents.TryGetValue(evt, out var list)) return;
        var remove = new List<ListenerEntry<object, object, object>>();
        foreach (var l in list)
        {
            if (l.Condition != null && !l.Condition((p1, p2, p3))) continue;
            if (l.Callback is Action<T1, T2, T3> a) a.Invoke(p1, p2, p3);
            if (l.Once) remove.Add(l);
        }
        foreach (var r in remove) list.Remove(r);
    }

    public void ClearEvent(HDU.Define.CoreDefine.EEventType evt)
    {
        _zeroParamEvents.Remove(evt);
        _oneParamEvents.Remove(evt);
        _twoParamEvents.Remove(evt);
        _threeParamEvents.Remove(evt);
    }
    #endregion

    // ======================== Invoke 비동기 ===========================

    public async UniTask InvokeAsync(HDU.Define.CoreDefine.EEventType evt)
    {
        if (!_zeroParamEvents.TryGetValue(evt, out var list)) return;
        var remove = new List<ListenerEntry>();
        foreach (var l in list)
        {
            if (l.Callback is Func<UniTask> f) await f.Invoke();
            else if (l.Callback is Action a) a.Invoke();
            if (l.Once) remove.Add(l);
        }
        foreach (var r in remove) list.Remove(r);
    }

    public async UniTask InvokeAsync<T>(HDU.Define.CoreDefine.EEventType evt, T param)
    {
        if (!_oneParamEvents.TryGetValue(evt, out var list)) return;
        var remove = new List<ListenerEntry<object>>();
        foreach (var l in list)
        {
            if (l.Condition != null && !l.Condition(param)) continue;
            if (l.Callback is Func<T, UniTask> f) await f.Invoke(param);
            else if (l.Callback is Action<T> a) a.Invoke(param);
            if (l.Once) remove.Add(l);
        }
        foreach (var r in remove) list.Remove(r);
    }

    public async UniTask InvokeAsync<T1, T2>(HDU.Define.CoreDefine.EEventType evt, T1 p1, T2 p2)
    {
        if (!_twoParamEvents.TryGetValue(evt, out var list)) return;

        var remove = new List<ListenerEntry<object, object>>();
        foreach (var l in list)
        {
            if (l.Condition != null && !l.Condition((p1, p2))) continue;

            if (l.Callback is Func<T1, T2, UniTask> f) await f.Invoke(p1, p2);
            else if (l.Callback is Action<T1, T2> a) a.Invoke(p1, p2);

            if (l.Once) remove.Add(l);
        }

        foreach (var r in remove) list.Remove(r);
    }

    public async UniTask InvokeAsync<T1, T2, T3>(HDU.Define.CoreDefine.EEventType evt, T1 p1, T2 p2, T3 p3)
    {
        if (!_threeParamEvents.TryGetValue(evt, out var list)) return;

        var remove = new List<ListenerEntry<object, object, object>>();
        foreach (var l in list)
        {
            if (l.Condition != null && !l.Condition((p1, p2, p3))) continue;

            if (l.Callback is Func<T1, T2, T3, UniTask> f) await f.Invoke(p1, p2, p3);
            else if (l.Callback is Action<T1, T2, T3> a) a.Invoke(p1, p2, p3);

            if (l.Once) remove.Add(l);
        }

        foreach (var r in remove) list.Remove(r);
    }
}
