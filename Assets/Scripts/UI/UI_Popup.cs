//using DG.Tweening;
using System;
using UnityEngine;
using HDU.Interface;

namespace HDU.UI
{
    public class UI_Popup : UI_Base, IPopupUI
    {
        [Header("Popup Variable")]
        [SerializeField] protected RectTransform _rect;
        [SerializeField] protected bool _isCantCloseBack = false;
        [SerializeField] protected bool _isPlayPopupSound = true;
        [SerializeField] protected Canvas _canvas = null;

        //public bool IsCantCloseBack { get => _isCantCloseBack; }
        bool IPopupUI.IsCantCloseBack { get => _isCantCloseBack; set => _isCantCloseBack = value; }
        Action<IPopupUI, bool> IPopupUI.OnCloseRequeted { get => OnCloseRequeted; set => OnCloseRequeted = value; }

        private Action<IPopupUI, bool> OnCloseRequeted;

        protected override void AwakeInit()
        {
            base.AwakeInit();
            if (_canvas == null)
                _canvas = GetComponent<Canvas>();
            //_canvas.sortingOrder = Managers.UI.CurPopupSort;
            //_canvas.worldCamera = Managers.UI.UICam;
        }

        protected virtual void ClosePopup(UnityEngine.EventSystems.PointerEventData ptData)
        {
            OnCloseRequeted?.Invoke(this, true);
        }
    }
}