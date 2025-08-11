//using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using HDU.Define;

public class UI_Slot : UI_Base/*EnhancedScrollerCellView*/
{
    [Header("General Reference")]
    [SerializeField] protected ScrollRect _scrollRect;
    [SerializeField] protected RectTransform _slotRect;

    protected bool _isEnable = false;
    protected bool _isDragging = false;

    protected virtual void AddScrollRectEvent()
    {
        gameObject.AddUIEvent(PointerDown, CoreDefine.EUIEventType.LClickDown);
        gameObject.AddUIEvent(OnBeginDrag, CoreDefine.EUIEventType.BeginDrag);
        gameObject.AddUIEvent(OnDrag, CoreDefine.EUIEventType.Drag);
        gameObject.AddUIEvent(OnEndDrag, CoreDefine.EUIEventType.EndDrag);
    }
    protected virtual void AddScrollRectEventChild(GameObject go)
    {
        go.AddUIEvent(PointerDown, CoreDefine.EUIEventType.LClickDown);
        go.AddUIEvent(OnBeginDrag, CoreDefine.EUIEventType.BeginDrag);
        go.AddUIEvent(OnDrag, CoreDefine.EUIEventType.Drag);
        go.AddUIEvent(OnEndDrag, CoreDefine.EUIEventType.EndDrag);
    }

    protected void SetSlotRect(Transform parent)
    {
        if (parent != null)
            _slotRect.SetParent(parent);
        _slotRect.localScale = new Vector3(1, 1, 1);
        _slotRect.localPosition = new Vector3(_slotRect.localPosition.x, _slotRect.localPosition.y, 0);
    }

    /// <summary>
    /// 호출할 UI 기능
    /// </summary>
    /// <param name="type"></param>

    #region ======================= PointerEvent ========================
    protected void PointerDown(PointerEventData ptData)
    {
        if (_scrollRect == null) return;
        _isEnable = true;
        _scrollRect.StopMovement();
    }
    protected void OnBeginDrag(PointerEventData ptData)
    {
        if (_scrollRect == null) return;
        _isEnable = true;
        _isDragging = true;
        _scrollRect.OnBeginDrag(ptData);
    }
    protected void OnDrag(PointerEventData ptData)
    {
        if (_scrollRect == null) return;
        if (Mathf.Abs(ptData.delta.x) > 2 || Mathf.Abs(ptData.delta.y) > 2)
            _isEnable = false;
        _scrollRect.OnDrag(ptData);
    }
    protected void OnEndDrag(PointerEventData ptData)
    {
        if (_scrollRect == null) return;
        _scrollRect.OnEndDrag(ptData);
        _isDragging = false;
        _isEnable = true;
    }
    #endregion
}
