namespace HDU.SaveData
{
    [System.Serializable]
    public class UserData : HDU.Interface.ISaveData
    {
        public SettingData Setting;
        public string Version;

        public void Initialize()
        {
            Setting = new SettingData();
            Version = "0.0.1";
        }
    }
}