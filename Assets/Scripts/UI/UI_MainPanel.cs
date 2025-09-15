using System.Text;
using TMPro;
using UnityEngine;

namespace HDU.UI
{
    public class UI_MainPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtInfo;
        [SerializeField] private GameObject _imgCheck;

        private StringBuilder _infoSB = new StringBuilder();
        private float _timeAcc = 0f;

        private void Start()
        {
            _imgCheck.SetActive(HDU.Managers.Managers.IsUseJob);
            DrawInfo();
        }

        private void Update()
        {
            _timeAcc += (Time.unscaledDeltaTime - _timeAcc) * 0.1f;
            DrawInfo();
        }

        private void DrawInfo()
        {
            _infoSB.Clear();
            float ms = _timeAcc * 1000f;
            float fps = 1.0f / _timeAcc;
            _infoSB.Append($"{fps:0.} FPS ({ms:0.0})\n");
            _infoSB.Append($"Unit Count : {HDU.Managers.Managers.Unit.GetUnitAllCount()}\n");
            _txtInfo.text = _infoSB.ToString();
        } 

        public void OnClickButton()
        {
            Managers.Managers.SwitchUseJob();
            _imgCheck.SetActive(HDU.Managers.Managers.IsUseJob);
        }
    }
}
