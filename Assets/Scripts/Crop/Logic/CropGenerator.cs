using UnityEngine;
using Trashure.Map;

namespace Trashure.CropPlant
{
    public class CropGenerator : MonoBehaviour
    {
#region 宣告
        private Grid currentGrid;

        public int seedItemID;
        public int growthDays;
#endregion

#region 事件函數
        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }

        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += GenerateCrop;
        }

        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;
        }
#endregion

#region 註冊事件
        private void GenerateCrop()
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);

            if (seedItemID != 0)
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);

                if (tile == null)
                {
                    tile = new TileDetails();
                    tile.gridX = cropGridPos.x;
                    tile.gridY = cropGridPos.y;
                }

                tile.daySinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;
                
                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }
#endregion
    }
}
