using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

namespace HDU.UI
{
    public class UI_Button : UI_EventHandler
    {
        [SerializeField] private Image ImgBtn;
        [SerializeField] private TextMeshProUGUI TxtTMP;
        //[SerializeField] private bool IsPlayClickSound = false;
        [SerializeField] protected bool IsAnim = true;
        [SerializeField] private bool IsSetScroll = false;

        protected ScrollRect _scrollRect;
        private RectTransform _rect;
        private Vector3 _oriScale;

        protected override void Awake()
        {
            base.Awake();
            _scrollRect = HDU.Utils.UnityUtils.GetComponentInNearestParent<ScrollRect>(transform);
            _rect = GetComponent<RectTransform>();
            _oriScale = _rect.localScale;
        }

        private void OnDisable()
        {
            //if(DOTween.IsTweening(_rect))
            //    _rect.DOKill();
            _rect.localScale = _oriScale;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!IsAnim) return;
            //if (DOTween.IsTweening(_rect))
            //    _rect.DOKill();
            //_rect.DOScaleX(_oriScale.x * 0.9f, 0.1f);
            //_rect.DOScaleY(_oriScale.x * 1.1f, 0.1f);
            base.OnPointerDown(eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!IsAnim) return;
            //if (DOTween.IsTweening(_rect))
            //    _rect.DOKill();
            //_rect.DOScale(_oriScale, 0.1f);
            base.OnPointerUp(eventData);
        }
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (IsSetScroll && _scrollRect != null)
                _scrollRect.OnBeginDrag(eventData);
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (IsSetScroll && _scrollRect != null)
                _scrollRect.OnDrag(eventData);
            base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (IsSetScroll && _scrollRect != null)
                _scrollRect.OnEndDrag(eventData);
            base.OnEndDrag(eventData);
        }

    }
}