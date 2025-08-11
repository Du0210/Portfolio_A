using UnityEngine;

[System.Serializable]
public class UserData : ISaveData
{
    public SettingData Setting;
    public string Version;

    public void Initialize()
    {
        Setting = new SettingData();
        Version = "0.0.1";
    }
}
