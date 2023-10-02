using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO", menuName = "Map/MapData_SO")]
public class MapData_SO : ScriptableObject
{
    [SceneName]public string sceneName;
    
    [Header("地圖信息")] 
    public int gridWidth;
    public int gridHeight;
    
    [Header("左下角原點")] 
    public int originX;
    public int originY;
    
    public List<TileProperty> tileProperties;
}