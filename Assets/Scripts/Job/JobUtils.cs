using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace HDU.Jobs
{
    public class JobUtils
    {

        //public static readonly FixedList128Bytes<int2> DIRECTIONS = new FixedList128Bytes<int2>
        //{
        //    new int2( 0,  1),
        //    new int2( 0, -1),
        //    new int2( 1,  0),
        //    new int2(-1,  0),
        //    new int2(-1,  1),
        //    new int2( 1,  1),
        //    new int2(-1, -1),
        //    new int2( 1, -1),
        //};

        //public static int GetNodeIndexFromWorldPos(float3 worldPos, int2 gridSize, float cellSize, float3 origin)
        //{
        //    int x = (int)math.floor((worldPos.x - origin.x) / cellSize);
        //    int y = (int)math.floor((worldPos.z - origin.z) / cellSize);

        //    x = math.clamp(x, 0, gridSize.x - 1);
        //    y = math.clamp(y, 0, gridSize.y - 1);

        //    return y * gridSize.x + x;
        //}

        public static int Get1DIndex(int x, int y, int2 gridSize) => y * gridSize.x + x;


        //public static void GetNeighbors(int2 current, int2 gridSize, ref FixedList128Bytes<int2> output)
        //{
        //    for (int i = 0; i < DIRECTIONS.Length; i++)
        //    {
        //        int2 neighbor = current + DIRECTIONS[i];
        //        if (neighbor.x >= 0 && neighbor.x < gridSize.x &&
        //            neighbor.y >= 0 && neighbor.y < gridSize.y)
        //        {
        //            output.Add(neighbor);
        //        }
        //    }
        //}

        //// GCost 계산(경로 예상 거리)
        //public static int GetHeuristic(int2 a, int2 b)
        //{
        //    int dx = math.abs(a.x - b.x);
        //    int dy = math.abs(a.y - b.y);
        //    return 14 * math.min(dx, dy) + 10 * math.abs(dx - dy);
        //}
        //// HCost 계산(실제 이동 거리) 둘의 내용이 똑같지만 일단 기능 구분을 위해 나눠놓음
        //public static int GetDistance(int2 a, int2 b)
        //{
        //    int dx = math.abs(a.x - b.x);
        //    int dy = math.abs(a.y - b.y);
        //    return 14 * math.min(dx, dy) + 10 * math.abs(dx - dy);
        //}
    }
}
