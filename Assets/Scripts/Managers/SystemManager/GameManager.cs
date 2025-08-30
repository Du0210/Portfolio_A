namespace HDU.Managers
{
    using System.Diagnostics;
    using UnityEngine;

    public class GameManager : HDU.Interface.IManager
    {
        bool _isInit = false;

        public void Init()
        {
            if (!_isInit)
            {
                Application.targetFrameRate = 60;
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");

                //if (LogEnabled)
                //{
                //    Debug.unityLogger.logEnabled = true;

                //    SRDebug.Init();
                //    SRDebug.Instance.IsTriggerEnabled = true;
                //}
                //else
                //{
                //    Debug.unityLogger.logEnabled = false;
                //}

                //CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector.StartDetection();
            }
        }

        public void Clear()
        {
            
        }
    }
}