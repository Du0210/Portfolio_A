using HDU.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace HDU.Managers
{
    public class Node
    {
        public Vector2Int GridPos;              // 그리드 상의 위치
        public Vector3 WorldPos;                // 실제 월드 공간 상의 좌표
        public bool IsWalkable;                 // 장애물 여부

        // A* 거리 비용들
        public int GCost;               // 시작점으로부터 현재 노드까지 실제 거리
        public int HCost;                       // 현재 -> 목표까지 예상 거리
        public int FCost => GCost + HCost;      // 전체 비용

        public Node Parent;

        // 유닛 크기를 비교해서 갈 수 있는지 없는지 체크
        public bool IsWalkableForUnit(float unitRadius, LayerMask obstacleMask)
        {
            bool b = !Physics.CheckSphere(WorldPos, unitRadius * 1.5f, obstacleMask);
            return b;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Node other) return false;
            return GridPos == other.GridPos;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class GridManager : IManager
    {
        // 가로 세로 셀 개수
        public Vector2Int GridSize { get; private set; } = new Vector2Int(8, 24);
        public float CellSize { get; private set; } = 0.5f;

        private Node[,] _grid;
        public Node[,] Grid => _grid;

        private Transform _gridPoint;

        public void Init()
        {

        }
        public void Clear()
        {

        }
        public void InitGrid(Transform gridPoint)
        {
            _grid = new Node[GridSize.x, GridSize.y];
            _gridPoint = gridPoint;

            LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
            
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    // 셀의 월드 위치 계산 (x,z 평면 기준)
                    Vector3 worldPos = _gridPoint.position + new Vector3(x * CellSize, 0, y * CellSize);

                    // 장애물 체크
                    bool walkable = !Physics.CheckBox(worldPos, Vector3.one * (CellSize * 0.4f), Quaternion.identity, obstacleLayer);

                    Node node = new Node
                    {
                        GridPos = new Vector2Int(x, y),
                        WorldPos = worldPos,
                        IsWalkable = walkable
                    };

                    _grid[x, y] = node;
                }
            }
        }

        public void InitGrid_Debug(Transform gridPoint, Vector2Int gridSize, float cellSize)
        {
            _grid = new Node[GridSize.x, GridSize.y];
            _gridPoint = gridPoint;
            GridSize = gridSize;
            CellSize = cellSize;

            LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");

            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    // 셀의 월드 위치 계산 (x,z 평면 기준)
                    Vector3 worldPos = _gridPoint.position + new Vector3(x * CellSize, 0, y * CellSize);

                    // 장애물 체크
                    bool walkable = !Physics.CheckBox(worldPos, Vector3.one * (CellSize * 0.4f), Quaternion.identity, obstacleLayer);

                    Node node = new Node
                    {
                        GridPos = new Vector2Int(x, y),
                        WorldPos = worldPos,
                        IsWalkable = walkable
                    };

                    _grid[x, y] = node;
                }
            }
        }

        /// <summary>
        /// 월드 위치 기반으로 가장 가까운 노드 반환
        /// 목적은 유닛의 위치를 내부 grid 상의 노드로 변환
        /// 시작 노드와 목표노드를 찾을때 사용
        /// </summary>
        public Node GetNodeFromWorldPos(Vector3 worldPos, Vector2Int gridSize, float cellSize)
        {
            int x = Mathf.FloorToInt((worldPos.x - _gridPoint.position.x + cellSize * 0.5f) / cellSize);
            int y = Mathf.FloorToInt((worldPos.z - _gridPoint.position.z + cellSize * 0.5f) / cellSize);

            if (x < 0 || x >= gridSize.x || y < 0 || y >= gridSize.y)
            {
                Debug.LogWarning($"[Grid] Out of bounds access. worldPos: {worldPos}, result: ({x}, {y})");
                return null;
            }

            return _grid[x, y];
        }

        /// <summary>
        /// 현재 노드를 기준으로 상하좌우 방향으로 이동 가능한 노드를 탐색
        /// 탐색 중에 주변 노드를 확인할때 사용
        /// </summary>
        public List<Node> GetNeighbors(Node node, Vector2Int gridSize, float unitRadius)
        {
            List<Node> neighbors = new List<Node>();

            // 방향 정의 일단 8방향
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0,1),
                new Vector2Int(0,-1),
                new Vector2Int(1,0),
                new Vector2Int(-1,0),
                new Vector2Int(-1,1),
                new Vector2Int(1,1),
                new Vector2Int(-1,-1),
                new Vector2Int(1,-1)
            };

            foreach(var dir in directions)
            {
                int checkX = node.GridPos.x + dir.x;
                int checkY = node.GridPos.y + dir.y;

                // 범위 내에 있는지 확인
                if(checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    Node neighbor = _grid[checkX, checkY];

                    if (neighbor.IsWalkable /*&& neighbor.IsWalkableForUnit(unitRadius, LayerMask.GetMask(nameof(Define.CoreDefine.ELayerMask.Obstacle)))*/)
                        neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public void DrawGizmos()
        {
            if (_grid == null) return;

            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    Node node = _grid[x, y];

                    Gizmos.color = node.IsWalkable ? Color.green : Color.red;

                    Gizmos.DrawWireCube(node.WorldPos + Vector3.up * 0.1f, Vector3.one * (CellSize * 0.9f));
                }
            }
        }
    }
}
