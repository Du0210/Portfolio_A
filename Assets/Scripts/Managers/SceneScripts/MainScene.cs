namespace HDU.Managers
{
    using UnityEngine;

    public class MainScene : BaseScene
    {
        [SerializeField] Transform _gridPoint;

        [Header("�׸��� �����")]
        [SerializeField] Vector2Int _gridSizeDebug = new Vector2Int(20, 20);
        [SerializeField] float _cellSizeDebug = 1f;

        private void Awake()
        {
            Managers.Click.SetCamera(Camera.main);
            // ��ȯ�� ���� ������ �׸��� ����
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