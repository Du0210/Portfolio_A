namespace HDU.Managers
{
    //using CodeStage.AntiCheat.ObscuredTypes;
    using System;
    using UnityEngine;
    using static HDU.Define.CoreDefine;

    public class TimeManager : HDU.Interface.IManager
    {
        #region Fields
        // 업데이트 가능 여부
        private bool _updatable = false;
        // 최근 서버에서 가져온 Unix 시간
        private double _synchronizedUnixTime = 0d;
        private int _deltaSeconds = 0;
        // 최근 서버에서 가져온 시간
        private System.DateTime _recentDateTime;
        
        private int _playTimeTotalSeconds = 0;
        private float _lastRealtime;
        private float _timeRatio = 1f;
        #endregion

        #region Properties
        // 초기화 여부
        public bool Initialized { get; private set; } = false;

        public bool IsStopTime { get => _timeRatio <= 0 ? true : false; }
        public int PlayTimeSeconds => _playTimeTotalSeconds;
        public TimeSpan PlayTimeSpan => TimeSpan.FromSeconds(_playTimeTotalSeconds);
        // 현재 시간
        public double CurrentTime { get => _deltaSeconds + _synchronizedUnixTime; }
        // 현재 시간 DateTime
        public System.DateTime @DateTime { get => _recentDateTime.AddSeconds(_deltaSeconds); }
        // 다음날까지 남은 시간
        public double UntilNextDay { get => 86400 - (CurrentTime % 86400); }
        // 다음주까지 남은 시간
        public double UntilNextWeek
        {
            get
            {
                switch (_recentDateTime.DayOfWeek)
                {
                    case System.DayOfWeek.Sunday:
                        return this.UntilNextDay;
                    case System.DayOfWeek.Monday:
                        return this.UntilNextDay + (86400 * 6);
                    case System.DayOfWeek.Tuesday:
                        return this.UntilNextDay + (86400 * 5);
                    case System.DayOfWeek.Wednesday:
                        return this.UntilNextDay + (86400 * 4);
                    case System.DayOfWeek.Thursday:
                        return this.UntilNextDay + (86400 * 3);
                    case System.DayOfWeek.Friday:
                        return this.UntilNextDay + (86400 * 2);
                    case System.DayOfWeek.Saturday:
                        return this.UntilNextDay + 86400;
                    default:
                        return -1;
                }
            }
        }
        #endregion
        public void Init()
        {
            
        }

        public void Clear()
        {
            
        }

        #region Behaviour Methods
        public void Update()
        {
            if (!_updatable) return;

            float now = Time.realtimeSinceStartup;
            float elapsed = now - _lastRealtime;

            if (elapsed < 1f) return;

            int ticks = Mathf.FloorToInt(elapsed);
            _lastRealtime = now;

            for (int i = 0; i < ticks; i++)
                TickOneSecond();
        }

        private void TickOneSecond()
        {
            _deltaSeconds++;
            _playTimeTotalSeconds++;

            Managers.Event.InvokeEvent(EEventType.OnChangedSeconds);

            if (_playTimeTotalSeconds % 60 == 0)
                Managers.Event.InvokeEvent(EEventType.OnChangedMinute);

            if (_playTimeTotalSeconds % 3600 == 0)
                Managers.Event.InvokeEvent(EEventType.OnChangedHour);

            // 자동 저장
            Managers.Save.AutoSaveUpdate();
        }

        #endregion

        #region Public Methods
        // 서버 타임 가져온 뒤 동기화
        public void SynchronizeTime(double serverUnixTime)
        {
            // 최초 서버시간 가져왔는지
            if (!Initialized)
            {
                _updatable = true;
                _lastRealtime = Time.realtimeSinceStartup;
            }

            _synchronizedUnixTime = serverUnixTime;
            _deltaSeconds = 0;
            _recentDateTime = HDU.Utils.CsUtils.UnixTimeToDateTime(serverUnixTime);

            Managers.Event.InvokeEvent(EEventType.OnSynchronizeTime, CurrentTime);

            Initialized = true; 
        }

        #endregion

        #region Private Methods
        // 날짜 바뀌었을 때 호출
        private void OnChangedDay(bool isWeekChanged)
        {

        }
        // 월 바뀌었을 때 호출
        private void OnChangedMonth(bool isWeekChanged)
        {

        }

        // 7일 이내에 주가 바뀌었는지 체크
        // ex) 마지막 접속-일요일 -> 현재 접속-월요일  == 바뀜
        private bool WeekChangedOnSynchronizeTime(System.DayOfWeek PrevDay, System.DayOfWeek CurrentDay)
        {
            switch (PrevDay)
            {
                case System.DayOfWeek.Sunday:
                    return CurrentDay != System.DayOfWeek.Sunday ? true : false;
                case System.DayOfWeek.Tuesday:
                    return CurrentDay == System.DayOfWeek.Monday ? true : false;
                case System.DayOfWeek.Wednesday:
                    return CurrentDay == System.DayOfWeek.Monday || CurrentDay == System.DayOfWeek.Tuesday ? true : false;
                case System.DayOfWeek.Thursday:
                    return CurrentDay == System.DayOfWeek.Monday || CurrentDay == System.DayOfWeek.Tuesday || CurrentDay == System.DayOfWeek.Wednesday ? true : false;
                case System.DayOfWeek.Friday:
                    return CurrentDay == System.DayOfWeek.Monday || CurrentDay == System.DayOfWeek.Tuesday || CurrentDay == System.DayOfWeek.Wednesday || CurrentDay == System.DayOfWeek.Thursday ? true : false;
                case System.DayOfWeek.Saturday:
                    return CurrentDay != System.DayOfWeek.Sunday && CurrentDay != System.DayOfWeek.Saturday ? true : false;
                default:
                    return false;
            }
        }
        #endregion

        #region CustomDeltaTime
        public float GetTimeRatio() => _timeRatio;
        public void SetTimeRatio(float ratio)
        {
            if (0 <= ratio && ratio <= 2f)
            {
                _timeRatio = ratio;
            }
            else
            {
                Debug.LogWarning("TimeScale 값 잘못 넣음");
            }
        }
        public float GetDeltaTime() => Time.deltaTime * _timeRatio;
        public float GetFixedDeltaTime() => Time.fixedDeltaTime * _timeRatio;
        #endregion
    }
}
