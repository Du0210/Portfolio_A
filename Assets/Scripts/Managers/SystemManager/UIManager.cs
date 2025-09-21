namespace HDU.Managers
{
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using HDU.Interface;
    using Cysharp.Threading.Tasks;

    public class UIManager : IManager
    {
        public void Init()
        {
            //SetUIRoot();
        }

        public void Clear()
        {
            ClosedAllPopup();
        }

        #region ============================ Delegate ============================
        //private Action<Define.EEventType> _notifyUIEvent = null;
        //private Action<HDU.Enum.ContentResetType> _restUIEvent = null;
        #endregion


        public Transform UIPopupRoot { get; private set; }
        public Transform UIRoot { get; private set; }
        public CanvasGroup UIRootRaycast;
        public int CurPopupSort { get; private set; } = 0;

        public Canvas RootCanvas;

        Utils.StackList<IPopupUI> _popupStack = new Utils.StackList<IPopupUI>();

        public void SetUIRoot()
        {
            GameObject uiRoot = GameObject.Find("#UI_Root");
            GameObject uiPopupRoot = GameObject.Find("#UI_PopupRoot");
            if (uiRoot == null)
            {
                GameObject go = Resources.Load<GameObject>("UI/#UI_Root");
                UIRoot = GameObject.Instantiate(go).transform;
                // 임시 카메라 매니저 만들면 거기서 할당하기
            }
            else
                UIRoot = uiRoot.transform;
            UIRoot.GetComponent<Canvas>().worldCamera = GameObject.Find("UI_Camera").GetComponent<Camera>();

            UIRootRaycast = Utils.UnityUtils.GetOrAddComponent<CanvasGroup>(UIRoot.gameObject);

            if (uiPopupRoot == null)
            {
                GameObject go = new GameObject { name = "#UI_PopupRoot" };
                UIPopupRoot = go.transform;
                // 임시
            }
            else
                UIPopupRoot = uiPopupRoot.transform;
            UIPopupRoot.GetComponent<Canvas>().worldCamera = GameObject.Find("UI_Camera").GetComponent<Camera>();
        }


        public void SetActiveMainUIRender()
        {
            if (!UIRootRaycast)
                return;

            UIRootRaycast.alpha = UIRootRaycast.alpha == 0 ? 1 : 0;
        }

        #region ============================ Managed ============================

        public async UniTask<T> MakeSlot<T>(Transform parent = null, string name = null) where T : IUI
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;
            GameObject go = await Managers.Resource.InstantiateLocalAsync($"UI/Slots/{name}", parent: parent);

            if (parent != null)
                go.transform.SetParent(parent);

            return go.GetComponent<T>();
        }

        public async UniTask<T> MakePopup<T>(Transform parent = null, string name = null, bool isAddSorting = true) where T : IUI, IPopupUI
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            if (isAddSorting)
                CurPopupSort++;
            GameObject go = await Managers.Resource.InstantiateLocalAsync($"UI/Popup/{name}", parent: parent);

            if (parent != null)
                go.transform.SetParent(parent);
            else
                go.transform.SetParent(UIPopupRoot);

            T popup = go.GetComponent<T>();

            popup.OnCloseRequeted += ClosedPopup;

            if (!popup.IsCantCloseBack)
                _popupStack.Push(popup);

            return popup;
        }

        public void ClosedPopup(IPopupUI popup = null, bool isAddSorting = true)
        {
            // 팝업을 종료버튼 클릭등으로 껐을때
            if (popup != null)
            {
                _popupStack.Remove(popup);
                if (isAddSorting)
                    CurPopupSort--;
                if (popup is MonoBehaviour mono)
                    Managers.Resource.Destroy(mono.gameObject);
                else
                    Debug.LogError("Popup Closed Fail");
            }
            // 팝업을 esc or escape키로 껐을때
            else
            {
                IPopupUI ui = _popupStack.Pop();

                // 현재 팝업이 켜져있을때
                if (ui != null)
                {
                    // 팝업이 닫히면 안되는 상황일떄
                    if (ui.IsCantCloseBack)
                        _popupStack.Push(ui);
                    // 팝업이 닫혀도 되는 상황일때
                    else
                    {
                        popup.OnCloseRequeted -= ClosedPopup;
                        if (popup is MonoBehaviour mono)
                            Managers.Resource.Destroy(mono.gameObject);
                        else
                            Debug.LogError("Popup Closed Fail");
                        if (isAddSorting)
                            CurPopupSort--;
                    }
                }
                // 팝업이 아무것도 없을때
                else
                {

                }
            }

        }
        public void ClosedAllPopup()
        {
            int count = _popupStack.Count;
            for (int i = 0; i < count; i++)
            {
                IPopupUI ui = _popupStack.Pop();
                if (ui != null)
                {
                    if (ui is MonoBehaviour mono)
                        Managers.Resource.Destroy(mono.gameObject);
                    else
                        Debug.LogError("Popup Closed Fail");
                }
            }
        }
        public int GetPopupListCount()
        {
            return _popupStack.Count;
        }
        #endregion

        #region ============================ Observer ============================
        //public void AddObserver(Action<Define.EEventType> action)
        //{
        //    if (_notifyUIEvent == null || !_notifyUIEvent.GetInvocationList().Contains(action))
        //        _notifyUIEvent += action;
        //}
        //public void RemoveObserver(Action<Define.EEventType> action) => _notifyUIEvent -= action;
        //public void Message(Define.EEventType type) => _notifyUIEvent?.Invoke(type);

        //public void AddObserver_ResetUI(Action<HDU.Enum.ContentResetType> action)
        //{
        //    if (_restUIEvent == null || !_restUIEvent.GetInvocationList().Contains(action))
        //        _restUIEvent += action;
        //}
        //public void RemoveObserver_ResetUI(Action<HDU.Enum.ContentResetType> action)
        //{
        //    _restUIEvent -= action;
        //}
        //public void Message_ResetUI(HDU.Enum.ContentResetType type)
        //{
        //    _restUIEvent?.Invoke(type);
        //}
        #endregion
    }
}