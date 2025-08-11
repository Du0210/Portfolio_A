namespace HDU.Managers
{
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public class UIManager : IManager
    {
        public void Init()
        {
            SetUIRoot();
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
                // �ӽ� ī�޶� �Ŵ��� ����� �ű⼭ �Ҵ��ϱ�
            }
            else
                UIRoot = uiRoot.transform;
            UIRoot.GetComponent<Canvas>().worldCamera = GameObject.Find("UI_Camera").GetComponent<Camera>();

            UIRootRaycast = Utils.UnityUtils.GetOrAddComponent<CanvasGroup>(UIRoot.gameObject);

            if (uiPopupRoot == null)
            {
                GameObject go = new GameObject { name = "#UI_PopupRoot" };
                UIPopupRoot = go.transform;
                // �ӽ�
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

        public T MakeSlot<T>(Transform parent = null, string name = null) where T : IUI
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;
            GameObject go = Managers.Resource.Instantiate($"UI/Slots/{name}");

            if (parent != null)
                go.transform.SetParent(parent);

            return go.GetComponent<T>();
        }

        public T MakePopup<T>(Transform parent = null, string name = null, bool isAddSorting = true) where T : IUI, IPopupUI
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;

            if (isAddSorting)
                CurPopupSort++;
            GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");

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
            // �˾��� �����ư Ŭ�������� ������
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
            // �˾��� esc or escapeŰ�� ������
            else
            {
                IPopupUI ui = _popupStack.Pop();

                // ���� �˾��� ����������
                if (ui != null)
                {
                    // �˾��� ������ �ȵǴ� ��Ȳ�ϋ�
                    if (ui.IsCantCloseBack)
                        _popupStack.Push(ui);
                    // �˾��� ������ �Ǵ� ��Ȳ�϶�
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
                // �˾��� �ƹ��͵� ������
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