namespace HDU.GameSystem
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;

    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private HDU.Define.CoreDefine.ESpawnType _spawmType;
        [SerializeField] private float radius = 10f;
        [SerializeField] private Color _gizmoColor = Color.yellow;

        private readonly int MAXUNITCOUNT = 10000;

        private GameObject _unitPrefab_S;
        private GameObject _unitPrefab_R;

        private async void Start()
        {
            if(_unitPrefab_R == null)
                _unitPrefab_R = await Managers.Managers.Resource.LoadAsync<GameObject>(nameof(HDU.Define.CoreDefine.EAddressableKey.Unit_Slime_R));
            if (_unitPrefab_S == null)
                _unitPrefab_S = await Managers.Managers.Resource.LoadAsync<GameObject>(nameof(HDU.Define.CoreDefine.EAddressableKey.Unit_Slime_S));

            await SpawnUpdate();
        }

        private async UniTask SpawnUpdate()
        {
            while (true)
            {
                GameObject unit = null;
                int currentCount = 0;

                switch (_spawmType)
                {
                    case Define.CoreDefine.ESpawnType.None:
                        Debug.LogError("Spawn Type is None");
                        return;
                    case Define.CoreDefine.ESpawnType.Slime_R:
                        {
                            currentCount = Managers.Managers.Unit.GetUnitCountByType<Unit_Slime_R>();
                            unit = _unitPrefab_R;
                            break;
                        }
                    case Define.CoreDefine.ESpawnType.Slime_S:
                        {
                            currentCount = Managers.Managers.Unit.GetUnitCountByType<Unit_Slime_S>();
                            unit = _unitPrefab_S;
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

                Managers.Managers.Unit.SpawnUnit<Unit_Slime_S>(spawnPos, unit, parent: transform);
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