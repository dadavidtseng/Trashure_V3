using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPatternList_SO", menuName = "Light/Light Pattern")]
public class LightPatternList_SO : ScriptableObject
{
    public List<LightDetails> lightPatternList;

    /// <summary>
    /// 根據季節和週期返回燈光詳情
    /// </summary>
    /// <param name="season">季節</param>
    /// <param name="lightShift">週期</param>
    /// <returns></returns>
    public LightDetails GetLightDetails(Season season, LightShift lightShift)
    {
        return lightPatternList.Find(l => l.season == season && l.lightShift == lightShift);
    }
}

[System.Serializable]
public class LightDetails
{
    public Season season;
    public LightShift lightShift;
    public Color lightColor;
    public float lightAmount;
}