using HDU.Define;
using HDU.Interface;
using HDU.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace HDU.UI
{
    public class UI_SpawnPanel : MonoBehaviour
    {
        [SerializeField] RectTransform _panelRect;
        [SerializeField] GameObject _slotPrefab;
        [SerializeField] Transform _slotParent_Slime;
        [SerializeField] Transform _slotParent_Spike;

        private HashSet<CoreDefine.EUnitPrefabKey> _unitKeySet = new HashSet<CoreDefine.EUnitPrefabKey>(); 

        private void Awake()
        {
            SetSlotList();
        }

        private void OnEnable()
        {
            Managers.Managers.Event.RegistEvent(Define.CoreDefine.EEventType.OnSetUnitSlot, SetSlotList);
        }

        private void OnDisable()
        {
            Managers.Managers.Event.RemoveEvent(Define.CoreDefine.EEventType.OnSetUnitSlot, SetSlotList);
        }

        private void SetSlotList()
        {
            Dictionary<CoreDefine.EUnitPrefabKey, GameObject> dict = Managers.Managers.Resource.CachedPrefabs;
            foreach (var item in dict)
            {
                IUnit unit = item.Value.GetComponent<IUnit>();
                if (unit.UnitType == CoreDefine.EUnitType.Slime)
                {
                    if (_unitKeySet.Contains(unit.UnitPrefabKey))
                        continue;
                    _unitKeySet.Add(unit.UnitPrefabKey);
                    var go = Managers.Managers.Resource.InstantiateLocal(go:_slotPrefab, parent: _slotParent_Slime);
                    Slot_Spawn slot = go.GetComponent<Slot_Spawn>();
                    slot.SetUI(unit.UnitPrefabKey);
                }
                else
                {
                    if (_unitKeySet.Contains(unit.UnitPrefabKey))
                        continue;
                    _unitKeySet.Add(unit.UnitPrefabKey);
                    var go = Managers.Managers.Resource.InstantiateLocal(go: _slotPrefab, parent: _slotParent_Spike);
                    Slot_Spawn slot = go.GetComponent<Slot_Spawn>();
                    slot.SetUI(unit.UnitPrefabKey);
                }
            }
        }
    }
}
