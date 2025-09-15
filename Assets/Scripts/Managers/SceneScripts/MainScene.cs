namespace HDU.Managers
{
    using UnityEngine;

    public class MainScene : BaseScene
    {
        [SerializeField] Transform _gridPoint;

        [Header("그리드 디버깅")]
        [SerializeField] Vector2Int _gridSizeDebug = new Vector2Int(20, 20);
        [SerializeField] float _cellSizeDebug = 1f;

        private void Awake()
        {
            Managers.Click.SetCamera(Camera.main);
            // 전환을 위해 각각의 그리드 생성
            Managers.Grid.InitGridJobCompatible(_gridPoint);
            Managers.Grid.InitGrid(_gridPoint);
        }

        public override void Clear()
        {

        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                Managers.Grid.DrawGizmos();
        }

        public void RebuildGrid()
        {
            Managers.Grid.InitGrid_Debug(_gridPoint, _gridSizeDebug, _cellSizeDebug);
        }
    }
}