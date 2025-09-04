using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace HDU.Jobs
{
    [BurstCompile]
    public struct FindPathJob : IJob
    {
        [ReadOnly] public NativeArray<NodeData> Grid;
        public NativeArray<NodeRecord> Records;

        [ReadOnly] public int2 GridSize;
        [ReadOnly] public float CellSize;
        [ReadOnly] public float3 GridOrigin;

        [ReadOnly] public float3 StartWorldPos;
        [ReadOnly] public float3 TargetWorldPos;

        [WriteOnly] public NativeList<int2> ResultPath;

        public void Execute()
        {
            int startIndex = GetNodeIndexFromWorldPos(StartWorldPos);
            int targetIndex = GetNodeIndexFromWorldPos(TargetWorldPos);

            Debug.Log($"StartIndex: {startIndex}, TargetIndex: {targetIndex}");
            Debug.Log($"StartPos: {StartWorldPos}, TargetPos: {TargetWorldPos}");

            if (!Grid[startIndex].IsWalkable || !Grid[targetIndex].IsWalkable)
                return;

            var startRecord = new NodeRecord
            {
                GCost = 0,
                HCost = GetHeuristic(Grid[startIndex].GridPos, Grid[targetIndex].GridPos),
                ParentIndex = -1,
                Flags = 1 // Open
            };
            Records[startIndex] = startRecord;

            for (int loop = 0; loop < 1000; loop++)
            {
                int current = FindLowestFCostNode();
                Debug.Log($" current: {current} (pos: {Grid[current].GridPos})");

                if (current == -1) return;
                if (current == targetIndex)
                {
                    RetracePath(startIndex, targetIndex);
                    return;
                }

                var r = Records[current];
                r.MarkClosed();
                Records[current] = r;

                var neighbors = new FixedList128Bytes<int2>();
                GetNeighbors(Grid[current].GridPos, ref neighbors);

                for (int i = 0; i < neighbors.Length; i++)
                {
                    Debug.Log($"ÀÌ¿ô Å½»ö: current = {current}, neighbor = {i}, Walkable = {Grid[i].IsWalkable}");

                    int2 n = neighbors[i];
                    int ni = n.y * GridSize.x + n.x;
                    if (!Grid[ni].IsWalkable || Records[ni].IsInClosedSet)
                        continue;

                    int gCost = Records[current].GCost + GetDistance(Grid[current].GridPos, Grid[ni].GridPos);
                    if (gCost < Records[ni].GCost)
                    {
                        var nr = Records[ni];
                        nr.GCost = gCost;
                        nr.HCost = GetHeuristic(Grid[ni].GridPos, Grid[targetIndex].GridPos);
                        nr.ParentIndex = current;
                        if (!nr.IsInOpenSet)
                            nr.MarkOpen();
                        Records[ni] = nr;
                    }
                }
            }
        }

        private int FindLowestFCostNode()
        {
            int best = -1;
            int bestFCost = int.MaxValue;
            for (int i = 0; i < Records.Length; i++)
            {
                if (!Records[i].IsInOpenSet) continue;
                if (Records[i].FCost < bestFCost)
                {
                    bestFCost = Records[i].FCost;
                    best = i;
                }
            }
            return best;
        }

        private void RetracePath(int startIndex, int endIndex)
        {
            var path = new NativeList<int2>(Allocator.Temp);
            int current = endIndex;
            while (current != startIndex && current != -1)
            {
                path.Add(Grid[current].GridPos);
                current = Records[current].ParentIndex;
            }

            for (int i = path.Length - 1; i >= 0; i--)
                ResultPath.Add(path[i]);

            path.Dispose();
        }

        private int GetNodeIndexFromWorldPos(float3 pos)
        {
            int x = (int)math.floor((pos.x - GridOrigin.x) / CellSize);
            int y = (int)math.floor((pos.z - GridOrigin.z) / CellSize);
            x = math.clamp(x, 0, GridSize.x - 1);
            y = math.clamp(y, 0, GridSize.y - 1);
            return y * GridSize.x + x;
        }

        public static readonly FixedList128Bytes<int2> DIRECTIONS = new FixedList128Bytes<int2>
        {
            new int2( 0,  1),
            new int2( 0, -1),
            new int2( 1,  0),
            new int2(-1,  0),
            new int2(-1,  1),
            new int2( 1,  1),
            new int2(-1, -1),
            new int2( 1, -1),
        };

        private void GetNeighbors(int2 current, ref FixedList128Bytes<int2> neighbors)
        {
            //int2[] dirs = new int2[]
            //{
            //new int2(0,1), new int2(0,-1),
            //new int2(1,0), new int2(-1,0),
            //new int2(1,1), new int2(1,-1),
            //new int2(-1,1), new int2(-1,-1),
            //};
            foreach (var dir in DIRECTIONS)
            {
                int2 n = current + dir;
                if (n.x >= 0 && n.x < GridSize.x && n.y >= 0 && n.y < GridSize.y)
                    neighbors.Add(n);
            }
        }

        private int GetHeuristic(int2 a, int2 b) =>
            10 * (math.abs(a.x - b.x) + math.abs(a.y - b.y)); // Manhattan

        private int GetDistance(int2 a, int2 b)
        {
            int dx = math.abs(a.x - b.x);
            int dy = math.abs(a.y - b.y);
            return 14 * math.min(dx, dy) + 10 * math.abs(dx - dy);
        }
    }
}