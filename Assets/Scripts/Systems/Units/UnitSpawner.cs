namespace HDU.GameSystem
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;

    public class UnitSpawner : MonoBehaviour
    {
        [Header("임시")]
        [SerializeField] private GameObject _unitPrefab_S;
        [SerializeField] private GameObject _unitPrefab_R;

        [SerializeField] private HDU.Define.CoreDefine.ESpawnType _spawmType;
        [SerializeField] private float radius = 10f;
        [SerializeField] private Color _gizmoColor = Color.yellow;

        private readonly int MAXUNITCOUNT = 1;

        private async void Start()
        {
            await SpawnUpdate();
        }
        private async UniTask SpawnUpdate()
        {
            while (true)
            {
                switch (_spawmType)
                {
                    case Define.CoreDefine.ESpawnType.None:
                        Debug.LogError("Spawn Type is None");
                        return;
                    case Define.CoreDefine.ESpawnType.Slime_R:
                        {
                            if (Managers.Managers.Unit.GetUnitCountByType<Unit_Slime_R>() >= MAXUNITCOUNT)
                            {
                                await UniTask.DelayFrame(1);
                                continue;
                            }

                            // x,y 평면 상의 랜덤 좌표
                            Vector2 randomPos = Random.insideUnitCircle * radius;

                            // 3D 변환 후 스포너 위치 기준으로 조정
                            Vector3 spawnPos = new Vector3(randomPos.x, 0, randomPos.y);
                            spawnPos += transform.position;

                            if (!IsOverlapping(spawnPos, 0.1f))
                            {
                                await UniTask.DelayFrame(1);
                                continue;
                            }

                            Managers.Managers.Unit.SpawnUnit<Unit_Slime_R>(spawnPos, _unitPrefab_R, parent: transform);
                            await UniTask.Delay(1000);
                            break;
                        }
                    case Define.CoreDefine.ESpawnType.Slime_S:
                        {
                            if (Managers.Managers.Unit.GetUnitCountByType<Unit_Slime_S>() >= MAXUNITCOUNT)
                            {
                                await UniTask.DelayFrame(1);
                                continue;
                            }

                            // x,y 평면 상의 랜덤 좌표
                            Vector2 randomPos = Random.insideUnitCircle * radius;

                            // 3D 변환 후 스포너 위치 기준으로 조정
                            Vector3 spawnPos = new Vector3(randomPos.x, 0, randomPos.y);
                            spawnPos += transform.position;

                            if (!IsOverlapping(spawnPos, 0.1f))
                            {
                                await UniTask.DelayFrame(1);
                                continue;
                            }

                            Managers.Managers.Unit.SpawnUnit<Unit_Slime_S>(spawnPos, _unitPrefab_S, parent: transform);
                            await UniTask.Delay(1000);
                            break;
                        }
                }
            }
        }

        private bool IsOverlapping(Vector3 postion, float radius)
        {
            return Physics.OverlapSphere(postion, radius, LayerMask.GetMask("Obstacle")).Length == 0;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}