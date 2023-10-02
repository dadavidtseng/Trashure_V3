using UnityEngine;

namespace Trashure.AStar
{
    public class GridNodes
    {
        private int width;
        private int height;
        private Node[,] gridNode;

        /// <summary>
        /// 構造函數初始化節點範圍數組
        /// </summary>
        /// <param name="width">地圖寬度</param>
        /// <param name="height">地圖高度</param>
        public GridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;

            gridNode = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridNode[x, y] = new Node(new Vector2Int(x, y));
                }
            }
        }

        public Node GetGridNode(int xPos, int yPos)
        {
            if (xPos < width && yPos < height)
            {
                return gridNode[xPos, yPos];
            }
            Debug.Log("超出網格範圍");
            return null;
        }
    }
}
