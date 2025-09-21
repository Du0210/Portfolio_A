namespace HDU.Managers
{
    using System.Collections.Generic;
    using UnityEngine;
    using HDU.Interface;
    using Unity.Collections;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Cysharp.Threading.Tasks;

    public class UnitManager : IManager
    {
        public HDU.Define.CoreDefine.EUnitPrefabKey SpikeType = Define.CoreDefine.EUnitPrefabKey.Unit_Slime_S;
        public HDU.Define.CoreDefine.EUnitPrefabKey SlimeType = Define.CoreDefine.EUnitPrefabKey.Unit_Slime_R;

        private List<IUnit> _unitList;
        private readonly float TARGETFINEDINTERVAL = 0.5f;
        private float _targetFinedTimer = 0f;

        public void Init()
        {
            _unitList = new List<IUnit>();
        }

        public void Clear()
        {
            
        }

        public void Update()
        {
            _targetFinedTimer += Time.deltaTime;

            if (_targetFinedTimer >= TARGETFINEDINTERVAL)
            {
                _targetFinedTimer = 0f;
                RunTargetSelectionJob();
            }
        }

        public async UniTask<T> SpawnUnit<T>(Vector3 pos, HDU.Define.CoreDefine.EUnitPrefabKey name, Transform parent = null) where T : IUnit
        {
            var cache = await Managers.Resource.GetCachedPrefabOrNull(name);
            var go = Managers.Resource.InstantiateLocal(go: cache, parent: parent);
            T unit = go.GetComponent<T>();
            unit.Initialize(pos);
            _unitList.Add(unit);

            return unit;
        }
        public void DespawnUnit(IUnit unit)
        {
            if (_unitList.Contains(unit))
                _unitList.Remove(unit);

            Managers.Resource.Destroy(unit.GameObject);
        }


        public int GetUnitAllCount()
        {
            return _unitList.Count;
        }

        public int GetUnitCountByType<T>() where T : IUnit
        {
            int count = 0;
            foreach (var unit in _unitList)
            {
                if (unit is T)
                {
                    count++;
                }
            }
            return count;
        }

        public IUnit FindNearstTargetOrNull(IUnit from)
        {
            IUnit target = HDU.Utils.UnityUtils.FindClosestUnit<IUnit>(_unitList, from);
            return target != null && target != from ? target : null;
        }
        
        #region Job
        private void RunTargetSelectionJob()
        {
            int count = _unitList.Count;
            if (count == 0) return;

            var positions = new NativeArray<float3>(count, Allocator.TempJob);
            var types = new NativeArray<int>(count, Allocator.TempJob);
            var results = new NativeArray<int>(count, Allocator.TempJob);

            for (int i = 0; i < count; i++)
            {
                positions[i] = _unitList[i].Transform.position;
                types[i] = (int)_unitList[i].UnitType;
            }

            var job = new HDU.Jobs.FindClosestUnitJob
            {
                Positions = positions,
                UnitTypes = types,
                ClosestTargetIndices = results
            };

            JobHandle handle = job.Schedule(count, 32); // 병렬 스레드 실행
            handle.Complete();                          // 작업 동기 대기 

            for (int i = 0; i < count; i++)
            {
                int targetIndex = results[i];
                if (targetIndex >= 0 && targetIndex < count)
                    _unitList[i].Target = _unitList[targetIndex];
                else
                    _unitList[i].Target = null;
            }

            // 메모리 누수 방지를 위해 NativeArray 해제
            positions.Dispose();
            types.Dispose();
            results.Dispose();
        }
        #endregion
    }
}