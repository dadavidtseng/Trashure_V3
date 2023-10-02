using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
#region 宣告
    public MapData_SO mapData;
    public GridType gridType;
    private Tilemap currentTilemap;
#endregion

#region 事件函數
    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();

            if (mapData != null)
            {
                mapData.tileProperties.Clear();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            
            UpdateTileProperties();
#if UNITY_EDITOR
            if (mapData != null)
            {
                EditorUtility.SetDirty(mapData);
            }
#endif
        }
    }
#endregion

    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();

        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                //已繪製範圍的左下角座標
                Vector3Int startPos = currentTilemap.cellBounds.min;
                //已繪製範圍的右上角座標
                Vector3Int endPos = currentTilemap.cellBounds.max;

                for (int x = startPos.x; x < endPos.x; x++)
                {
                    for (int y = startPos.y; y < endPos.y; y++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            TileProperty newTile = new TileProperty
                            {
                                tileCoordinate = new Vector2Int(x, y),
                                gridType = this.gridType,
                                boolTypeValue = true
                            };
                            
                            mapData.tileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}
