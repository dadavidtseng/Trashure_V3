using System.Collections.Generic;
using UnityEngine;
using Trashure.Map;

namespace Trashure.AStar
{
    public class AStar : Singleton<AStar>
    {
        private GridNodes gridNodes;
        private Node startNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<Node> openNodeList; //當前選中Node周圍的8個點
        private HashSet<Node> closeNodeList; //所有被選中的點

        private bool pathFound;

        /// <summary>
        /// 構建路徑更新Stack的每一步
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="npcMovementStack"></param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos, Stack<MovementStep> npcMovementStack)
        {
            pathFound = false;

            if (GenerateGridNodes(sceneName, startPos, endPos))
            {
                //查找最短路徑
                if (FindShortestPath())
                {
                    //構建NPC移動路徑
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
                }
            }
        }
        
        /// <summary>
        /// 構建網格節點信息，初始化兩個列表
        /// </summary>
        /// <param name="sceneName">場景名字</param>
        /// <param name="startPos">起點</param>
        /// <param name="endPos">終點</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions,
                    out Vector2Int gridOrigin))
            {
                //根據瓦片地圖範圍構建網格移動節點範圍數組
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();

                closeNodeList = new HashSet<Node>();
            }
            else
            {
                return false;
            }
            //gridNodes的範圍是從(0, 0)開始，所以需要減去原點座標得到實際位置
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    var key = tilePos.x + "x" + tilePos.y + "y" + sceneName;

                    TileDetails tile = GridMapManager.Instance.GetTileDetails(key);

                    if (tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tile.isNPCObstacle)
                        {
                            node.isObstacle = true;
                        }
                    }
                }
            }
            
            return true;
        }

        /// <summary>
        /// 找到最短路徑所有Node添加到closedNodeList
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            //添加起點
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {
                //節點排序，Node內涵比較函數
                openNodeList.Sort();

                Node closeNode = openNodeList[0];

                openNodeList.RemoveAt(0);
                closeNodeList.Add(closeNode);

                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }
                
                //計算周圍8個Node補充道OpenList
                EvaluateNeighbourNodes(closeNode);
            }

            return pathFound;
        }

        /// <summary>
        /// 評估周圍8個點，並生成對應消耗值
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)    //避免循環到自己(0, 0)
                    {
                        continue;
                    }

                    validNeighbourNode = GetValidNeighbourNodes(currentNodePos.x + x, currentNodePos.y + y);

                    if (validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            //鏈接父節點
                            validNeighbourNode.parentNode = currentNode;
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 找到有效的Node，非障礙，非已選擇
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidNeighbourNodes(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
            {
                return null;
            }

            Node neighbourNode = gridNodes.GetGridNode(x, y);

            if (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode))
            {
                return null;
            }
            else
            {
                return neighbourNode;
            }
        }

        /// <summary>
        ///返回兩點距離值
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns>14的倍數+10的倍數</returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }

            return 14 * xDistance + 10 * (yDistance - xDistance);
        }

        /// <summary>
        /// 更新路徑每一步的座標和場景名字
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="npcMovementStop"></param>
        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> npcMovementStop)
        {
            Node nextNode = targetNode;

            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate =
                    new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                //壓入Stack
                npcMovementStop.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}
