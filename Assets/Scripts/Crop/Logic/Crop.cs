using System.Collections;
using UnityEngine;

public class Crop : MonoBehaviour
{
#region 宣告
    public CropDetails cropDetails;
    public TileDetails tileDetails;
    private int harvestActionCount;
    public bool CanHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;

    private Animator anim;

    private Transform playerTransform => FindObjectOfType<Player>().transform;
#endregion

    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        tileDetails = tile;
        
        //工具使用次數
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1)
        {
            return;
        }

        anim = GetComponentInChildren<Animator>();

        //點擊計數器
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;
            
            //判斷是否有動畫 樹木
            if (anim != null && cropDetails.hasAnimation)
            {
                if (playerTransform.position.x < transform.position.x)
                {
                    anim.SetTrigger("RotateRight");
                }
                else
                {
                    anim.SetTrigger("RotateLeft");
                }
            }
            //播放粒子
            if (cropDetails.hasParticleEffect)
            {
                EventHandler.CallParticleEffectEvent(cropDetails.effectType,
                    transform.position + cropDetails.effectPos);
            }
            //播放聲音
            if (cropDetails.soundEffect != SoundName.none)
            {
                EventHandler.CallPlaySoundEvent(cropDetails.soundEffect);
            }
        }

        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)
            {
                //生成農作物
                SpawnHarvestItems();
            }
            else if(cropDetails.hasAnimation)
            {
                if (playerTransform.position.x < transform.position.x)
                {
                    anim.SetTrigger("FallingRight");
                }
                else
                {
                    anim.SetTrigger("FallingLeft");
                }
                EventHandler.CallPlaySoundEvent(SoundName.TreeFalling);
                StartCoroutine(HarvestAfterAnimation());
            }
        }
    }

    private IEnumerator HarvestAfterAnimation()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("END"))
        {
            yield return null;
        }
        SpawnHarvestItems();
        
        //轉換新物件
        if (cropDetails.transferItemID > 0)
        {
            CreateTransferCrop();
        }
    }

    private void CreateTransferCrop()
    {
        tileDetails.seedItemID = cropDetails.transferItemID;
        tileDetails.daySinceLastHarvest = -1;
        tileDetails.growthDays = 0;
        
        EventHandler.CallRefreshCurrentMap();
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
            else    //物品隨機數量
            {
                amountToProduce = Random.Range(cropDetails.produceMinAmount[i], cropDetails.produceMaxAmount[i] + 1);
            }
            
            //執行生成指定數量的物品
            for (int j = 0; j < amountToProduce; j++)
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
                else    //在世界地圖上生成物品
                {
                    //判斷應該生成的物品方向
                    var dirX = transform.position.x > playerTransform.position.x ? 1 : -1;
                    //一定範圍內的隨機
                    var spawnPos = 
                        new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX), 
                        transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);

                    EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                }
            }
        }

        if (tileDetails != null)
        {
            tileDetails.daySinceLastHarvest++;
            
            //是否可以重複生長
            if (cropDetails.dayToRegrow > 0 && tileDetails.daySinceLastHarvest < cropDetails.regrowTimes)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.dayToRegrow;
                //刷新種子
                EventHandler.CallRefreshCurrentMap();
            }
            else    //不可重複生長
            {
                tileDetails.daySinceLastHarvest = -1;
                tileDetails.seedItemID = -1;
                //FIXME : 自己設計
                // tileDetails.daySinceDug = -1;
            }
            
            Destroy(gameObject);
        }
    }
}
