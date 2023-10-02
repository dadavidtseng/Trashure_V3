using UnityEngine;

namespace Trashure.CropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;

        private Transform playerTransform => FindObjectOfType<Player>().transform;

        public void InitCropData(int ID)
        {
            cropDetails = CropManager.Instance.GetCropDetails(ID);
        }

        /// <summary>
        /// 生成果實
        /// </summary>
        public void SpawnHarvestItems()
        {
            for (int i = 0; i < cropDetails.producedItemID.Length; i++)
            {
                int amountToProduce;

                if (cropDetails.produceMinAmount[i] == cropDetails.produceMaxAmount[i])
                {
                    //代表指生成指定數量的
                    amountToProduce = cropDetails.produceMinAmount[i];
                }
                else //物品隨機數量
                {
                    amountToProduce =
                        Random.Range(cropDetails.produceMinAmount[i], cropDetails.produceMaxAmount[i] + 1);
                }

                //執行生成指定數量的物品
                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)
                    {
                        EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                    }
                    else //在世界地圖上生成物品
                    {
                        //判斷應該生成的物品方向
                        var dirX = transform.position.x > playerTransform.position.x ? 1 : -1;
                        //一定範圍內的隨機
                        var spawnPos =
                            new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                                transform.position.y +
                                Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);

                        EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                    }
                }
            }
        }
    }
}
