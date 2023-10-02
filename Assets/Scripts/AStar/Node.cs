using System;
using UnityEngine;

namespace Trashure.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition;    //網格座標
        public int gCost = 0;    //距離Start格子的距離
        public int hCost = 0;    //距離Target格子的距離
        public int fCost => gCost + hCost;    //當前格子的值
        public bool isObstacle = false;    //當前格子是否為障礙
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            //比較選出最低的f值，返回-1, 0, 1
            int result = fCost.CompareTo(other.fCost);
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}