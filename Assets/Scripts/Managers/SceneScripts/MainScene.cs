namespace HDU.Managers
{
    using HDU.Define;
    using UnityEngine;
    
    public class MainScene : BaseScene
    {
        [SerializeField] Transform _gridPoint;

        [Header("그리드 디버깅")]
        [SerializeField] Vector2Int _gridSizeDebug = new Vector2Int(20, 20);
        [SerializeField] float _cellSizeDebug = 1f;

        private async void Awake()
        {
            Managers.Click.SetCamera(Camera.main);
            // 전환을 위해 각각의 그리드 생성
            Managers.Grid.InitGridJobCompatible(_gridPoint);
            Managers.Grid.InitGrid(_gridPoint);
            await Managers.Resource.LoadAssetsByLabelAsync(CoreDefine.ELabelKey.cdn);
            await Managers.Resource.LoadAssetsByLabelAsync(CoreDefine.ELabelKey.local);
            Managers.Event.InvokeEvent(CoreDefine.EEventType.OnSetUnitSlot);
        }

        public override void Clear()
        {

        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying && Managers.Grid.IsSetActiveGrid)
                Managers.Grid.DrawGizmos();
        }

        public void RebuildGrid()
        {
            Managers.Grid.InitGrid_Debug(_gridPoint, _gridSizeDebug, _cellSizeDebug);
        }
    }
}