namespace HDU.Systems
{
    using UnityEngine;

    public class CustomDeltaTime
    {
        private static float _timeRatio = 1f;
        public static float GetTimeRatio() => _timeRatio;
        public static bool IsStop = false;
        public static void SetTimeRatio(float ratio)
        {
            if (0 <= ratio && ratio <= 2f)
            {
                if (ratio <= 0.1f)
                    IsStop = true;
                else
                    IsStop = false;

                _timeRatio = ratio;
            }
            else
            {
                Debug.LogWarning("TimeScale 값 잘못 넣음");
            }
        }

        public static float GetDeltaTime() => Time.deltaTime * _timeRatio;
        public static float GetFixedDeltaTime() => Time.fixedDeltaTime * _timeRatio;
    }
}