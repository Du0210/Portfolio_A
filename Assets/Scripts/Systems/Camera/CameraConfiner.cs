namespace HDU.GameSystem
{
    using System.Linq;
    using UnityEngine;

    public class CameraConfiner : MonoBehaviour
    {
        [System.Serializable]
        public class Volume
        {
            public Collider collider;     // isTrigger 권장 (BoxCollider 또는 MeshCollider)
            public Transform insidePoint; // 이 볼륨 내부의 기준점(중앙)
        }

        public Volume[] volumes;
        public float margin = 0.15f;     // 벽에서 약간 띄우기
        public float smooth = 0f;        // 필요 시 보정 스무딩(0은 즉시)

        Vector3 _smoothed;

        void Awake() => _smoothed = Vector3.positiveInfinity;

        public Vector3 ClampInside(Vector3 pos)
        {
            if (volumes == null || volumes.Length == 0) return pos;

            // 1) 이미 어떤 볼륨 안에 있으면 그대로(또는 살짝 안쪽으로)
            foreach (var v in volumes)
            {
                if (!v.collider) continue;
                Vector3 closest = v.collider.ClosestPoint(pos);
                bool inside = (closest - pos).sqrMagnitude < 1e-6f; // 내부면 ClosestPoint=자기자신
                if (inside)
                {
                    // 안전 여유: 벽에 너무 붙었으면 안쪽으로 margin 만큼 살짝 이동
                    if (v.insidePoint)
                    {
                        Vector3 toInside = (v.insidePoint.position - pos).normalized;
                        pos += toInside * 0.0001f; // 미세한 내부 오프셋
                    }
                    return Smooth(pos);
                }
            }

            // 2) 밖에 있다면: 가장 가까운 볼륨의 표면으로 스냅 후, 안쪽으로 margin 만큼 밀어넣기
            float bestDist = float.MaxValue;
            Vector3 bestPoint = pos;
            Vector3 inward = Vector3.zero;

            foreach (var v in volumes)
            {
                if (!v.collider) continue;
                Vector3 closest = v.collider.ClosestPoint(pos);      // 표면의 최근접점
                float d = (closest - pos).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    bestPoint = closest;
                    inward = v.insidePoint ? (v.insidePoint.position - closest).normalized : Vector3.zero;
                }
            }

            Vector3 corrected = bestPoint + inward * margin;
            return Smooth(corrected);
        }

        Vector3 Smooth(Vector3 target)
        {
            if (smooth <= 0f || !_smoothed.IsFinite()) { _smoothed = target; return target; }
            _smoothed = Vector3.Lerp(_smoothed, target, Time.deltaTime * smooth);
            return _smoothed;
        }
    }

    static class VecExt
    {
        public static bool IsFinite(this Vector3 v)
            => float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
    }
}