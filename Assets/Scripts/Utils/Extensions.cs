using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return HDU.Utils.UnityUtils.GetOrAddComponent<T>(go);
    }

    public static T GetComponentInNearestParent<T>(this GameObject go, Transform start) where T : Component
    {
        T result;
        Transform trans = start;

        if (trans.parent == null)
        {
            if (trans.TryGetComponent<T>(out result))
                return result;
            else
                return null;
        }
        else
        {
            while (trans.parent != null)
            {
                if (trans.TryGetComponent<T>(out result))
                    return result;
                else
                    trans = trans.parent;
            }

            return null;
        }
    }

    #region Array Initialize
    public static void InitializeArray<T>(this T[] array) where T : class, new()
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new T();
        }
    }
    public static T[] InitalizeArray<T>(int length) where T : class, new()
    {
        T[] array = new T[length];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new T();
        }
        return array;
    }
    #endregion
}
