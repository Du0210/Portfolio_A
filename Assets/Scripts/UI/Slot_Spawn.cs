using HDU.Define;
using UnityEngine;
using HDU.Managers;

namespace HDU.UI
{
    public class Slot_Spawn : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _imgIcon;
        [SerializeField] private GameObject _imgSelect;

        private CoreDefine.EUnitPrefabKey _unitKey;

        private void OnEnable()
        {
            Managers.Managers.Event.RegistEvent(CoreDefine.EEventType.OnRefreshUnitSlots, OnDrawSelect);
        }
        private void OnDisable()
        {
            Managers.Managers.Event.RemoveEvent(CoreDefine.EEventType.OnRefreshUnitSlots, OnDrawSelect);
        }

        public async void SetUI(CoreDefine.EUnitPrefabKey key)
        {
            _unitKey = key;
            _imgIcon.sprite = await Managers.Managers.Resource.GetCachedSpriteOrNull((CoreDefine.ESpriteKey)((int)key));
            OnDrawSelect();
        }

        private void OnDrawSelect()
        {
            if (_unitKey < CoreDefine.EUnitPrefabKey.Unit_Slime_S)
                _imgSelect.SetActive(Managers.Managers.Unit.SlimeType == _unitKey);
            else
                _imgSelect.SetActive(Managers.Managers.Unit.SpikeType == _unitKey);
        }

        public void OnClickSlot()
        {
            if (_unitKey < CoreDefine.EUnitPrefabKey.Unit_Slime_S)
                Managers.Managers.Unit.SlimeType = _unitKey;
            else
                Managers.Managers.Unit.SpikeType = _unitKey;

            Managers.Managers.Event.InvokeEvent(CoreDefine.EEventType.OnRefreshUnitSlots);
        }
    }
}
