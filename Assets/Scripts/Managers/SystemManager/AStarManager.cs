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
                // FCost�� ���� ���� ��� ����
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

                //��ǥ ������ ���������� ��� ��ȯ
                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);
                //if (currentNode.GridPos == targetNode.GridPos)
                //    return RetracePath(startNode, targetNode);
                //if (currentNode.GridPos == targetNode.GridPos)
                //    return RetracePath(startNode, targetNode);


                // �̿� ��� �˻�
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
            return null; // ��θ� ã�� ���� ��� null ��ȯ
        }

        /// <summary>
        /// ��� �������� ��ȯ    
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

            path.Reverse(); // ��θ� ���������� �������� ������

            return path;
        }

        /// <summary>
        /// ��� �������� ��ȯ    
        /// </summary>
        private int GetDistance(Node a, Node b)
        {
            int dstX = Mathf.Abs(a.GridPos.x - b.GridPos.x);
            int dstY = Mathf.Abs(a.GridPos.y - b.GridPos.y);
            // �밢�� �̵� ����� ����Ͽ� ���
            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);

            // �밢�� ������� ���� ���
            //return 10 * (dstX + dstY);
        }
    }
}
