namespace HDU.Managers
{
    using System.Collections.Generic;
    using UnityEngine;
    using HDU.Interface;

    public class UnitManager : IManager
    {
        private List<IUnit> _unitList;
        public void Init()
        {
            _unitList = new List<IUnit>();
        }

        public void Clear()
        {
            
        }


        public T SpawnUnit<T>(Vector3 pos, GameObject original = null, Transform parent = null) where T : IUnit
        {
            var go = Managers.Resource.Instantiate(obj: original, parent: parent);
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
    }
}