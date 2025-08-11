namespace HDU.Utils
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CsUtils
    {
        public static T GetGenericReturn<T>(object data) => (T)Convert.ChangeType(data, typeof(T));

        #region Enum
        public static T StringToEnum<T>(string str, bool isPrintErrorLog = true) where T : struct, System.Enum
        {
            T result;
            bool isParse = System.Enum.TryParse(str, out result);

            if (isParse)
                return result;
            else
            {
                if (isPrintErrorLog)
                    Debug.LogError("Cant Find Enum Value \nname : " + str);
                return default;
            }
        }
        public static int GetEnumCount<T>()
        {
            T[] myEnum = (T[])System.Enum.GetValues(typeof(T));
            return myEnum.Length;
        }
        #endregion

        #region Time
        public static DateTime UnixTimeToDateTime(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            origin = origin.AddSeconds(timestamp);
            return origin;
        }

        public static System.TimeSpan DateTimeToTimeSpan(System.DateTime dateTime)
        {
            System.DateTime date = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime - date;
        }

        public static double DateTimeToDouble(System.DateTime dateTime)
        {
            return DateTimeToTimeSpan(dateTime).TotalSeconds;
        }

        public static T[] InitializeArray<T>(int length) where T : class, new()
        {
            T[] array = new T[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = new T();
            }
            return array;
        }

        #region TimeTextFormat
        public static string ConvertDoubleToStringHourTimeFormat(double time, bool isNoSec, bool isNoSpace)
        {
            double hour = time / 3600f;
            double min = (time % 3600f) / 60f;
            double sec = (time % 3600f) % 60f;
            string timeText = "";

            if (!isNoSpace)
            {
                if (!isNoSec)
                    timeText = string.Format("{0:00} : {1:00} : {2:00}", Math.Truncate(hour), Math.Truncate(min), Math.Truncate(sec));
                else
                    timeText = string.Format("{0:00} : {1:00}", Math.Truncate(hour), Math.Truncate(min));
            }
            else
            {
                if (!isNoSec)
                    timeText = string.Format("{0:00}:{1:00}:{2:00}", Math.Truncate(hour), Math.Truncate(min), Math.Truncate(sec));
                else
                    timeText = string.Format("{0:00}:{1:00}", Math.Truncate(hour), Math.Truncate(min));
            }

            return timeText;
        }

        public static string ConvertDoubleToStringHourTimeFormat_Kor(double time, bool isNoSec)
        {
            double hour = time / 3600f;
            double min = (time % 3600f) / 60f;
            double sec = (time % 3600f) % 60f;
            string timeText = "";

            if (!isNoSec)
                timeText = string.Format("{0:00시간} : {1:00분} : {2:00초}", Math.Truncate(hour), Math.Truncate(min), Math.Truncate(sec));
            else
            {
                if (hour < 1f)
                    timeText = string.Format("{0:0분}", Math.Truncate(min));
                else
                    timeText = string.Format("{0:0시간} : {1:0분}", Math.Truncate(hour), Math.Truncate(min));
            }


            return timeText;
        }
        public static string ConvertDoubleToStringHourTimeFormat_Kor_NoDot(double time, bool isNoSec)
        {
            double hour = time / 3600f;
            double min = (time % 3600f) / 60f;
            double sec = (time % 3600f) % 60f;
            string timeText = "";

            if (!isNoSec)
                timeText = string.Format("{0:00시간}{1:00분}{2:00초}", Math.Truncate(hour), Math.Truncate(min), Math.Truncate(sec));
            else
                timeText = string.Format("{0:00시간}{1:00분}", Math.Truncate(hour), Math.Truncate(min));

            return timeText;
        }
        public static string ConvertDoubleToStringHourTimeFormat_Eng(double time, bool isNoSec)
        {
            double hour = time / 3600f;
            double min = (time % 3600f) / 60f;
            double sec = (time % 3600f) % 60f;
            string timeText = "";

            if (!isNoSec)
                timeText = string.Format("{0:00H} : {1:00M} : {2:00Sec}", Math.Truncate(hour), Math.Truncate(min), Math.Truncate(sec));
            else
                timeText = string.Format("{0:00H} {1:00M}", Math.Truncate(hour), Math.Truncate(min));

            return timeText;
        }

        public static string ConvertDoubleToStringMinTimeFormat(double time)
        {
            double min = (time % 3600f) / 60f;
            double sec = (time % 3600f) % 60f;
            string timeText = string.Format("{0:00}:{1:00}", Math.Truncate(min), Math.Truncate(sec));
            return timeText;
        }
        public static string ConvertDoubleToStringMinTimeFormat_Kor(double time)
        {
            double min = (time % 3600f) / 60f;
            double sec = (time % 3600f) % 60f;
            string timeText = string.Format("{0:00분} : {1:00초}", Math.Truncate(min), Math.Truncate(sec));
            return timeText;
        }

        public static string InsertCommaInNumber(long number)
        {
            int digit = 0;
            long temp = number;

            while (temp >= 1000)
            {
                temp /= 1000;
                digit++;
            }

            if (digit > 0)
            {
                string result = number.ToString();

                for (int i = 0; i < digit; i++)
                {
                    result = result.Insert(result.Length - (4 + (i * 4)) + 1, ",");
                }
                return result;
            }
            else
                return number.ToString();

        }
        #endregion

        #endregion

        #region Number Unit
        private static List<int> _numListG = new List<int>();
        private static string[] _goldUnitArrG = new string[] {
            "", "K", "M","a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az",
            "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "vz"};

        private static List<int> _numListK = new List<int>();
        private static string[] _goldUnitArrK = new string[] { "", "만", "억", "조", "경", "해", "자", "양", "구", "간", "정", "재", "극",
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az",
            "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz"};
        
        private static System.Text.StringBuilder _stringBuilder = new System.Text.StringBuilder();

        public static string GetGoldText_Kor(long goods)
        {
            int placeN = 4;
            long value = goods;

            int p = (int)Mathf.Pow(10, placeN);

            do
            {
                _numListK.Add((int)(value % p));
                value /= p;
            }
            while (value >= 1);

            string retStr = "";
            for (int i = Math.Max(0, _numListK.Count - 2); i < _numListK.Count; i++)
            {
                if (i != _numListK.Count - 1 && _numListK[i] == 0) continue;
                retStr = _numListK[i] + _goldUnitArrK[i] + retStr;
            }

            _numListK.Clear();
            return retStr;
        }

        public static string GetGoldText_Kor(double goods, bool isEng, bool isOneUnitKor = false)
        {
            if (goods == 0)
                return "0";

            _stringBuilder.Clear();
            goods = Math.Truncate(goods);
            if (!isEng)
            {
                int unitCount = (isOneUnitKor) ? 1 : 2;
                int placeN = 4;
                double value = goods;

                int p = (int)Mathf.Pow(10, placeN);

                do
                {
                    _numListK.Add((int)(value % p));
                    value /= p;
                }
                while (value >= 1);

                for (int i = _numListK.Count - 1; i >= Math.Max(0, _numListK.Count - unitCount/*2*/); i--)
                {
                    if (i != _numListK.Count - 1 && _numListK[i] == 0) continue;
                    _stringBuilder.Append(_numListK[i]);
                    _stringBuilder.Append(_goldUnitArrK[i]);
                    _stringBuilder.Append(" ");
                }

                _numListK.Clear();
            }
            else
            {
                int placeN = 3;
                int num = 0;
                double value = goods;
                int p = (int)Mathf.Pow(10, placeN);

                if (value < 1000)
                {
                    return value.ToString("F0");
                }
                else
                {
                    do
                    {
                        _numListG.Add((int)(value % p));
                        value /= p;
                    } while (value >= 1);

                    if (_numListG.Count < 2)
                        num = _numListG[0];
                    else
                        num = _numListG[_numListG.Count - 1] * p + _numListG[_numListG.Count - 2];

                    float f = (num / (float)p);
                    _stringBuilder.Append(f.ToString("N2") + GetUnitText_Global(_numListG.Count - 1));
                    _numListG.Clear();
                }
            }
            return _stringBuilder.ToString();
        }

        public static string GetGoldText_Kor_One(double goods)
        {
            if (goods == 0)
                return "0";

            int placeN = 4;
            double value = goods;

            int p = (int)Mathf.Pow(10, placeN);

            do
            {
                _numListK.Add((int)(value % p));
                value /= p;
            }
            while (value >= 1);

            string retStr = "";
            for (int i = Math.Max(0, _numListK.Count - 1); i < _numListK.Count; i++)
            {
                if (i != _numListK.Count - 1 && _numListK[i] == 0) continue;
                retStr = _numListK[i] + _goldUnitArrK[i] + " " + retStr;
            }

            _numListK.Clear();
            return retStr;
        }

        private static string GetUnitText_Global(int index)
        {
            string retStrt = "";
            int num = index;
            retStrt = _goldUnitArrG[num];
            return retStrt;
        }
        #endregion
    }

    public class StackList<T>
    {
        private List<T> _Data = new List<T>();

        public int Count { get { return _Data.Count; } }

        /// <summary>
        /// StackList안에 값을 넣습니다.
        /// </summary>
        /// <param name="Data"></param>
        public void Push(T Data)
        {
            _Data.Add(Data);
        }

        /// <summary>
        /// 마지막 값을 삭제 후 반환합니다.
        /// </summary>
        /// <returns> Last Value </returns>
        public T Pop()
        {
            if (_Data.Count > 0)
            {
                T temp = _Data[_Data.Count - 1];
                _Data.RemoveAt(_Data.Count - 1);
                return temp;
            }
            else
            {
                //Debug.Log("StackList Pop Falied");
                return default(T);
            }
        }

        /// <summary>
        /// 마지막 값을 반환합니다.
        /// </summary>
        /// <returns> Last Value </returns>
        public T Peek()
        {
            if (_Data.Count > 0)
            {
                T temp = _Data[_Data.Count - 1];
                return temp;
            }
            else
            {
                //Debug.Log("StackList Peek Falied");
                return default(T);
            }
        }

        /// <summary>
        /// 해당 인덱스 값을 삭제 후 반환합니다.
        /// </summary>
        /// <param name="index"> 해당 인덱스 </param>
        /// <returns> Value </returns>
        public T RemoveAt(int index)
        {
            if (index > _Data.Count)
            {
                Debug.LogError("인덱스에 해당하는 값이 없습니다.");
                return default(T);
            }

            T temp = _Data[index];

            _Data.RemoveAt(index);

            return temp;
        }

        public void Remove(T value)
        {
            if (!_Data.Contains(value))
                Debug.LogError("해당하는 값이 없습니다.");

            _Data.Remove(value);
        }

        /// <summary>
        /// Value에 맞는 Index를 반환합니다. 
        /// </summary>
        /// <param name="data"> Value </param>
        /// <returns> Index </returns>
        public int IndexOf(T data)
        {
            int index = _Data.IndexOf(data);

            return index;
        }

        public bool Contains(T data) => _Data.Contains(data);

        public void Insert(int index, T value) => _Data.Insert(index, value);

        public T this[int index]
        {
            get { return _Data[index]; }
            set { _Data[index] = value; }
        }
        public void Clear() => _Data.Clear();
    }
}