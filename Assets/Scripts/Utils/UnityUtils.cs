namespace HDU.Utils
{
    using HDU.Interface;
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    public class UnityUtils
    {
        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }

        public static Transform FindClosestUnit(List<Transform> toList, Transform from)
        {
            Transform closest = null;
            float minDist = float.MaxValue;

            foreach (var enemy in toList)
            {
                float dist = Vector3.Distance(from.position, enemy.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }
            return closest;
        }

        //public static T FindClosestUnit<T>(List<T> toList, T from) where T : IUnit
        //{
        //    T closest = default(T);
        //    float minDist = float.MaxValue;

        //    foreach (var enemy in toList)
        //    {
        //        if (from.UnitType == enemy.UnitType)
        //            continue;
                
        //        float dist = Vector3.Distance(from.Transform.position, enemy.Transform.position);
        //        if (dist < minDist)
        //        {
        //            minDist = dist;
        //            closest = enemy;
        //        }
        //    }
        //    return closest;
        //}
        public static T FindClosestUnit<T>(List<T> toList, T from, float maxRange = float.MaxValue) where T : IUnit
        {
            T closest = default;
            float minDist = float.MaxValue;

            foreach (var enemy in toList)
            {
                if (ReferenceEquals(enemy, from)) continue;
                if (from.UnitType == enemy.UnitType) continue;
                if (!enemy.IsVaild || !enemy.GameObject.activeSelf) continue;

                float dist = Vector3.Distance(from.Transform.position, enemy.Transform.position);
                if (dist < 0.1f || dist > maxRange) continue;

                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }

            return closest;
        }

        public static Vector3 GetApproachPosition(Transform to, Transform from, float attackRange)
        {
            Vector3 dir = (to.position - from.position).normalized;
            float stopDistance = Mathf.Clamp(Vector3.Distance(from.position, to.position) - attackRange, 0f, float.MaxValue);
            return from.position + dir * stopDistance;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform transform = FindChild<Transform>(go, name, recursive);

            if (transform == null) return null;

            return transform.gameObject;
        }

        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (go == null) return null;

            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform transform = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        T component = transform.GetComponent<T>();
                        if (component != null) return component;
                    }
                }
            }
            else
            {
                foreach (T component in go.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }

            return null;
        }

        public static T GetComponentInNearestParent<T>(Transform start) where T : Component
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

        public static bool IsObjectInCamera_3D(Vector3 pos, Camera cam)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
            Vector3 point = pos;
            foreach (var plane in planes)
            {
                if (plane.GetDistanceToPoint(point) < 0)
                    return false;
            }
            return true;
        }

        public static bool IsObjectInCamera_2D(Vector3 pos, Camera cam, out int dir)
        {
            Vector3 camInObjPos = cam.WorldToViewportPoint(pos);
            if (camInObjPos.x < 0f) { dir = 3; return false; }  // 3
            else if (camInObjPos.x > 1.2f) { dir = 1; return false; }      // 1
            else if (camInObjPos.y < 0f) { dir = 2; return false; }      // 2
            else if (camInObjPos.y > 1f) { dir = 0; return false; }      // 0
            else { dir = -1; return true; }
        }

        public static void LookAt2D(Transform standard, Transform target)
        {
            if (!target || !standard) return;
            float digree = Mathf.Atan2(target.position.y - standard.position.y, target.position.x - standard.position.x);
            standard.transform.Rotate(0, 0, digree);
        }

        public static Quaternion LookAt2D(Vector2 dir)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public static Vector3 GetSpriteSize(SpriteRenderer target)
        {
            Vector3 worldSize = Vector3.zero;
            Vector2 spriteSize = target.sprite.rect.size;
            Vector2 localSpriteSize = spriteSize / target.sprite.pixelsPerUnit;
            worldSize = localSpriteSize;
            worldSize.x *= target.transform.lossyScale.x;
            worldSize.y *= target.transform.lossyScale.y;
            return worldSize;
        }
    }
}