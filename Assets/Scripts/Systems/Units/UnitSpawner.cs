namespace HDU.GameSystem
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;

    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private HDU.Define.CoreDefine.ESpawnType _spawmType;
        [SerializeField] private float radius = 10f;
        [SerializeField] private Color _gizmoColor = Color.yellow;

        private readonly int MAXUNITCOUNT = 400;

        private async void Start()
        {
            await SpawnUpdate();
        }

        private async UniTask SpawnUpdate()
        {
            while (true)
            {
                int currentCount = 0;

                switch (_spawmType)
                {
                    case Define.CoreDefine.ESpawnType.None:
                        Debug.LogError("Spawn Type is None");
                        return;
                    case Define.CoreDefine.ESpawnType.Slime_R:
                        {
                            currentCount = Managers.Managers.Unit.GetUnitCountByType<Unit_Slime_R>();
                            break;
                        }
                    case Define.CoreDefine.ESpawnType.Slime_S:
                        {
                            currentCount = Managers.Managers.Unit.GetUnitCountByType<Unit_Slime_S>();
                            break;
                        }
                }

                if (currentCount >= MAXUNITCOUNT)
                {
                    await UniTask.DelayFrame(1);
                    continue;
                }

                Vector2 randomPos = Random.insideUnitCircle * radius;

                Vector3 spawnPos = new Vector3(randomPos.x, 0, randomPos.y);
                spawnPos += transform.position;

                if (!IsOverlapping(spawnPos, 0.1f))
                {
                    await UniTask.DelayFrame(1);
                    continue;
                }
                switch (_spawmType)
                {
                    case Define.CoreDefine.ESpawnType.Slime_R: await Managers.Managers.Unit.SpawnUnit<Unit_Slime_R>(spawnPos, Managers.Managers.Unit.SlimeType, parent: transform); break;
                    case Define.CoreDefine.ESpawnType.Slime_S: await Managers.Managers.Unit.SpawnUnit<Unit_Slime_S>(spawnPos, Managers.Managers.Unit.SpikeType, parent: transform); break;
                }

                await UniTask.Delay(1000);
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