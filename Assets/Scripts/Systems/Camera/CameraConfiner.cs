namespace HDU.GameSystem
{
    using System.Linq;
    using UnityEngine;

    public class CameraConfiner : MonoBehaviour
    {
        [System.Serializable]
        public class Volume
        {
            public Collider collider;     // isTrigger ���� (BoxCollider �Ǵ� MeshCollider)
            public Transform insidePoint; // �� ���� ������ ������(�߾�)
        }

        public Volume[] volumes;
        public float margin = 0.15f;     // ������ �ణ ����
        public float smooth = 0f;        // �ʿ� �� ���� ������(0�� ���)

        Vector3 _smoothed;

        void Awake() => _smoothed = Vector3.positiveInfinity;

        public Vector3 ClampInside(Vector3 pos)
        {
            if (volumes == null || volumes.Length == 0) return pos;

            // 1) �̹� � ���� �ȿ� ������ �״��(�Ǵ� ��¦ ��������)
            foreach (var v in volumes)
            {
                if (!v.collider) continue;
                Vector3 closest = v.collider.ClosestPoint(pos);
                bool inside = (closest - pos).sqrMagnitude < 1e-6f; // ���θ� ClosestPoint=�ڱ��ڽ�
                if (inside)
                {
                    // ���� ����: ���� �ʹ� �پ����� �������� margin ��ŭ ��¦ �̵�
                    if (v.insidePoint)
                    {
                        Vector3 toInside = (v.insidePoint.position - pos).normalized;
                        pos += toInside * 0.0001f; // �̼��� ���� ������
                    }
                    return Smooth(pos);
                }
            }

            // 2) �ۿ� �ִٸ�: ���� ����� ������ ǥ������ ���� ��, �������� margin ��ŭ �о�ֱ�
            float bestDist = float.MaxValue;
            Vector3 bestPoint = pos;
            Vector3 inward = Vector3.zero;

            foreach (var v in volumes)
            {
                if (!v.collider) continue;
                Vector3 closest = v.collider.ClosestPoint(pos);      // ǥ���� �ֱ�����
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