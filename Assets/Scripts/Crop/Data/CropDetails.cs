using UnityEngine;

[System.Serializable]
public class CropDetails
{
    public int seedItemID;
    [Header("不同階段需要的天數")] 
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }

    [Header("不同階段物品Prefab")] 
    public GameObject[] growthPrefab;

    [Header("不同階段的圖片")] 
    public Sprite[] growthSprites;

    [Header("可種植的季節")] 
    public Season[] seasons;

    [Space] 
    [Header("收割工具")] 
    public int[] harvestToolItemID;

    [Header("每種工具使用次數")] 
    public int[] requireActionCount;
    
    [Header("轉換新物品ID")] 
    public int transferItemID;

    [Space] 
    [Header("收割果實信息")] 
    public int[] producedItemID;
    public int[] produceMinAmount;
    public int[] produceMaxAmount;
    public Vector2 spawnRadius;

    [Header("再次生長時間")] 
    public int dayToRegrow;
    public int regrowTimes;

    [Header("Options")] 
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticleEffect;
    
    public ParticleEffectType effectType;
    public Vector3 effectPos;
    public SoundName soundEffect;

    /// <summary>
    /// 檢查當前工具是否可用
    /// </summary>
    /// <param name="toolID">工具ID</param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvestToolItemID)         
        {
            if (tool == toolID)
            {
                return true;
            }
        }
        return false;
    }

    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if (harvestToolItemID[i] == toolID)
            {
                return requireActionCount[i];
            }
        }

        return -1;
    }
}
