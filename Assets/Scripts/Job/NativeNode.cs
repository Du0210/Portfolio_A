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
        public int ParentIndex;
        public byte Flags; // Bitmask: 1=open, 2=closed

        public int FCost => GCost + HCost;

        public bool IsInOpenSet => (Flags & 1) != 0;
        public bool IsInClosedSet => (Flags & 2) != 0;

        public void MarkOpen() => Flags |= 1;
        public void MarkClosed()
        {
            Flags &= unchecked((byte)~1); // remove Open
            Flags |= 2;                   // add Closed
        }
    }
}
