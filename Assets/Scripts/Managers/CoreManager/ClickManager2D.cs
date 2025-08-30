namespace HDU.Managers
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using HDU.Interface;

    public class ClickManager2D : IManager
    {
        private Camera _camera;
        private LayerMask _layerMask;

        public void Init()
        {
            int objectLayer = LayerMask.NameToLayer("Object");
            _layerMask = (1 << objectLayer);
        }

        public void Clear()
        {

        }

        public void SetCamera(Camera cam)
        {
            _camera = cam;
        }

        public void Update()
        {

#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                ProcessRaycast(pos);
            }
            //#elif UNITY_ANDROID || UNITY_IOS
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if(touch.phase == TouchPhase.Began)
                {
                    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        return;
                    Vector2 pos = _camera.ScreenToWorldPoint(touch.position);
                    ProcessRaycast(pos);
                }
            }
#endif
        }

        private void ProcessRaycast(Vector2 pos)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0, _layerMask);

            if(hit.collider != null)
            {
                var clickable = hit.collider.GetComponentInParent<IClickable>();
                if (clickable != null)
                    clickable.OnClick2D();
            }
        }
    }
}

