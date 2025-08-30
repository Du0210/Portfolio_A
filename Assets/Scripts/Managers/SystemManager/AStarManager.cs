using HDU.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace HDU.Managers
{
    public class AStarManager : IManager
    {
        private Vector2Int GridSize { get => Managers.Grid.GridSize; }
        private float CellSize { get => Managers.Grid.CellSize; }

        public void Init()
        {
            
        }

        public void Clear()
        {
            
        }

        public List<Node> FindPath(Vector3 startWorldPos, Vector3 targetWorldPos, float unitRadius)
        {
            GridManager grid = Managers.Grid;
            Node startNode = grid.GetNodeFromWorldPos(startWorldPos, GridSize, CellSize);
            Node targetNode = grid.GetNodeFromWorldPos(targetWorldPos, GridSize, CellSize);

            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                // FCost가 가장 낮은 노드 선택
                Node currentNode = openList[0];
                for(int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost 
                        && openList[i].HCost < currentNode.HCost))
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                //목표 지점에 도달했으면 경로 반환
                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);
                //if (currentNode.GridPos == targetNode.GridPos)
                //    return RetracePath(startNode, targetNode);
                //if (currentNode.GridPos == targetNode.GridPos)
                //    return RetracePath(startNode, targetNode);


                // 이웃 노드 검사
                foreach (Node neighbor in grid.GetNeighbors(currentNode, GridSize, unitRadius))
                {
                    if (closedList.Contains(neighbor))
                        continue;

                    int newMovementCostToneighbor = currentNode.GCost + GetDistance(currentNode, neighbor);

                    if(newMovementCostToneighbor < neighbor.GCost || !openList.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToneighbor;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if(!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }

            Debug.LogWarning($"FindPath FAILED. Start: {startNode.GridPos}, Target: {targetNode.GridPos}, Walkable: {targetNode.IsWalkable}, Is in Grid: {Managers.Grid.Grid[targetNode.GridPos.x, targetNode.GridPos.y] != null}");
            return null; // 경로를 찾지 못한 경우 null 반환
        }

        /// <summary>
        /// 경로 역추적후 반환    
        /// </summary>
        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node current = endNode;

            while (current != startNode)
            {
                path.Add(current);
                current = current.Parent;
            }

            path.Reverse(); // 경로를 시작점에서 끝점으로 뒤집기

            return path;
        }

        /// <summary>
        /// 경로 역추적후 반환    
        /// </summary>
        private int GetDistance(Node a, Node b)
        {
            int dstX = Mathf.Abs(a.GridPos.x - b.GridPos.x);
            int dstY = Mathf.Abs(a.GridPos.y - b.GridPos.y);
            // 대각선 이동 비용을 고려하여 계산
            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);

            // 대각성 고려하지 않은 경우
            //return 10 * (dstX + dstY);
        }
    }
}
