namespace HDU.SaveData
{
    [System.Serializable]
    public class SettingData
    {
        public float BgmVolume = 1f;
        public float SfxVolume = 1f;
        public string Language = "ko";

        public SettingData()
        {
            Initialize();
        }

        public void Initialize()
        {
            BgmVolume = 1f;
            SfxVolume = 1f;
            Language = "ko";
        }
    }
}