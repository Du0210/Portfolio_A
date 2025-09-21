using HDU.Define;
using System.Text;
using TMPro;
using UnityEngine;

namespace HDU.UI
{
    public class UI_MainPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtInfo;
        [SerializeField] private GameObject _imgCheck_Job;
        [SerializeField] private GameObject _imgCheck_Preload;
        [SerializeField] private GameObject _imgCheck_Grid;

        private StringBuilder _infoSB = new StringBuilder();
        private float _timeAcc = 0f;

        private async void Start()
        {
            _imgCheck_Job.SetActive(HDU.Managers.Managers.IsUseJob);
            _imgCheck_Preload.SetActive(await Managers.Managers.Resource.IsLabelPreloaded(CoreDefine.ELabelKey.cdn));
            _imgCheck_Grid.SetActive(Managers.Managers.Grid.IsSetActiveGrid);

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

        public void OnClickButton_Job()
        {
            Managers.Managers.SwitchUseJob();
            _imgCheck_Job.SetActive(HDU.Managers.Managers.IsUseJob);
        }

        public async void OnClickButton_Preload()
        {
            await HDU.Managers.Managers.Resource.PreloadLabelAsync(CoreDefine.ELabelKey.cdn);
            await Managers.Managers.Resource.LoadAssetsByLabelAsync(CoreDefine.ELabelKey.cdn);
            Managers.Managers.Event.InvokeEvent(CoreDefine.EEventType.OnSetUnitSlot);
            _imgCheck_Preload.SetActive(await Managers.Managers.Resource.IsLabelPreloaded(CoreDefine.ELabelKey.cdn));
        }

        public void OnClickButton_GridSetActive()
        {
            Managers.Managers.Grid.IsSetActiveGrid = !Managers.Managers.Grid.IsSetActiveGrid;
            _imgCheck_Grid.SetActive(Managers.Managers.Grid.IsSetActiveGrid);
        }
    }
}
