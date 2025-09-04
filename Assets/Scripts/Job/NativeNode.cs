using Unity.Mathematics;
using UnityEngine;

namespace HDU.Jobs
{
    public struct NodeData
    {
        public int2 GridPos;
        public float3 WorldPos;
        public bool IsWalkable;
    }

    public struct NodeRecord
    {
        public int GCost;
        public int HCost;
        public int ParentIndex;     // 부모 노드의 1D 인덱스
        public byte Flags;          // OpenSet / CosedSet 정보를 1바이트로 압축

        public int FCost => GCost + HCost;

        public bool IsInOpenSet => (Flags & 1) != 0;
        public bool IsInClosedSet => (Flags & 2) != 0;

        public void MarkOpen() => Flags |= 1;
        public void MarkClosed() => Flags |= 2;
    }
}
