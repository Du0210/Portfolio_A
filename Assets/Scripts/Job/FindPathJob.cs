using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace HDU.Jobs
{
    #region MyRegion
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

            if (!Grid[startIndex].IsWalkable || !Grid[targetIndex].IsWalkable)
                return;

            var startRecord = new NodeRecord
            {
                GCost = 0,
                HCost = GetHeuristic(Grid[startIndex].GridPos, Grid[targetIndex].GridPos),
                ParentIndex = -1,
                Flags = 1
            };
            Records[startIndex] = startRecord;

            for (int loop = 0; loop < 1000; loop++)
            {
                int current = FindLowestFCostNode(targetIndex);
                if (current == -1)
                    return;

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
                    int2 n = neighbors[i];
                    int ni = n.y * GridSize.x + n.x;

                    if (!Grid[ni].IsWalkable || Records[ni].IsInClosedSet)
                        continue;

                    int newG = Records[current].GCost + GetDistance(Grid[current].GridPos, n);
                    if (newG < Records[ni].GCost)
                    {
                        var nr = Records[ni];
                        nr.GCost = newG;
                        nr.HCost = GetHeuristic(n, Grid[targetIndex].GridPos);
                        nr.ParentIndex = current;
                        if (!nr.IsInOpenSet)
                            nr.MarkOpen();
                        Records[ni] = nr;
                    }
                }
            }
        }

        private void RetracePath(int startIndex, int endIndex)
        {
            var path = new NativeList<int2>(Allocator.Temp);
            int current = endIndex;

            int loop = 0;
            while (current != startIndex && current != -1)
            {
                loop++;
                if (loop > 1000) break;

                path.Add(Grid[current].GridPos);
                current = Records[current].ParentIndex;
            }

            if (current == -1) return;

            for (int i = path.Length - 1; i >= 0; i--)
                ResultPath.Add(path[i]);

            path.Dispose();
        }

        private int FindLowestFCostNode(int targetIndex)
        {
            int bestIndex = -1;
            int bestFCost = int.MaxValue;
            int bestHCost = int.MaxValue;

            for (int i = 0; i < Records.Length; i++)
            {
                if (!Records[i].IsInOpenSet)
                    continue;

                int fCost = Records[i].FCost;

                if (fCost < bestFCost || (fCost == bestFCost && Records[i].HCost < bestHCost))
                {
                    bestFCost = fCost;
                    bestHCost = Records[i].HCost;
                    bestIndex = i;
                }

                // 강제 타겟 우선 처리 (실험적으로 안정성 향상용)
                if (i == targetIndex && Records[i].IsInOpenSet)
                    return i;
            }

            return bestIndex;
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
            foreach (var dir in DIRECTIONS)
            {
                int2 n = current + dir;
                if (n.x >= 0 && n.x < GridSize.x && n.y >= 0 && n.y < GridSize.y)
                    neighbors.Add(n);
            }
        }

        private int GetHeuristic(int2 a, int2 b)
        {
            int dx = math.abs(a.x - b.x);
            int dy = math.abs(a.y - b.y);
            return 14 * math.min(dx, dy) + 10 * math.abs(dx - dy);
        }

        private int GetDistance(int2 a, int2 b) => GetHeuristic(a, b);
    }
    #endregion
}