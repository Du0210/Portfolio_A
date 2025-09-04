using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace HDU.Jobs
{
    [BurstCompile]
    public struct FindClosestUnitJob: IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> Positions;
        [ReadOnly] public NativeArray<int> UnitTypes;

        [WriteOnly] public NativeArray<int> ClosestTargetIndices;

        public void Execute(int index)
        {
            float3 from = Positions[index];
            int fromType = UnitTypes[index];

            float minDist = float.MaxValue;
            int closestIdx = -1;

            for (int i = 0; i < Positions.Length; i++)
            {
                if (i == index || UnitTypes[i] == fromType) continue;

                float dist = math.distance(from, Positions[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestIdx = i;
                }
            }
            
            ClosestTargetIndices[index] = closestIdx;
        }
    }
}
