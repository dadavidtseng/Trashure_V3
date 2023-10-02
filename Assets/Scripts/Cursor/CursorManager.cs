using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Trashure.Map;
using Trashure.CropPlant;
using Trashure.Inventory;

public class CursorManager : MonoBehaviour
{
#region 宣告
    public Sprite normal, tool, seed, item;

    private Sprite currentSprite;    //存儲當前鼠標圖片
    private Image cursorImage;
    private RectTransform cursorCanvas;
    
    //建造圖標跟隨
    private Image buildImage;

    //鼠標檢測
    private Camera mainCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    private bool cursorPositionValid;

    private ItemDetails currentItem;

    private Transform PlayerTransform => FindObjectOfType<Player>().transform;
#endregion

#region 事件函數
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }
    
    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        //拿到建造圖標
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);
        
        currentSprite = normal;
        SetCursorImage(normal);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (cursorCanvas == null)
        {
            return;
        }

        cursorImage.transform.position = Input.mousePosition;
        
        if (! InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
            buildImage.gameObject.SetActive(false);
        }
    }
#endregion

#region 註冊事件
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
            buildImage.gameObject.SetActive(false);
        }
        else    //物品被選中才切換圖片
        {
            currentItem = itemDetails;
            //WORKFLOW : 添加所有類型對應圖片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool => tool,
                _ => normal
            };
            cursorEnable = true;
            
            //顯示建造物品圖片
            if (itemDetails.itemType == ItemType.Furniture)
            {
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
                buildImage.SetNativeSize();
            }
        }
    }
    
    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }
    
    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }
#endregion

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            //執行方法
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }
        
    /// <summary>
    /// 設置鼠標圖片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
        
    /// <summary>
    /// 設置鼠標可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, 0.5f);
    }

    /// <summary>
    /// 設置鼠標不可用
    /// </summary>
    private void SetCursorInvalid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
        buildImage.color = new Color(1, 0, 0, 0.5f);
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);
        
        //建造圖片跟隨移動
        buildImage.rectTransform.position = Input.mousePosition;
        
        //判斷在使用範圍內
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius ||
            Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInvalid();
            return;
        }
        
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);
            
            //WORKFLOW : 補充所有物品類型的判斷
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if(currentTile.daySinceDug > -1 && currentTile.seedItemID == -1)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
                case ItemType.HoeTool:
                    if(currentTile.canDig)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
                case ItemType.WaterTool:
                    if(currentTile.daySinceDug > -1 && currentTile.daySinceWatered == -1)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
                case ItemType.BreakTool:
                case ItemType.ChopTool:
                    if (crop != null)
                    {
                        if(crop.CanHarvest&&crop.cropDetails.CheckToolAvailable(currentItem.itemID))
                        {
                            SetCursorValid();
                        }
                        else
                        {
                            SetCursorInvalid();
                        }  
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
                case ItemType.CollectTool:
                    if (currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))
                        {
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays)
                            {
                                SetCursorValid();
                            }
                            else
                            {
                                SetCursorInvalid();
                            }
                        }
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
                case ItemType.ReapTool:
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos, currentItem))
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;
                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);
                    var bluePrintDetails =
                        InventoryManager.Instance.bluePrintData.GetBluePrintDetails(currentItem.itemID);

                    if (currentTile.canPlaceFurniture && InventoryManager.Instance.CheckStock(currentItem.itemID) &&
                        !HaveFurnitureInRadius(bluePrintDetails))
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    
                    break;
            }
        }
        else
        {
            SetCursorInvalid();
        }
    }
    
    /// <summary>
    /// 是否與UI互動
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        return false;
    }
    
    private bool HaveFurnitureInRadius(BluePrintDetails bluePrintDetails)
    {
        var buildItem = bluePrintDetails.buildPrefab;
        Vector2 point = mouseWorldPos;
        var size = buildItem.GetComponent<BoxCollider2D>().size;
        
        var otherColl = Physics2D.OverlapBox(point, size, 0);
        if (otherColl != null)
        {
            return otherColl.GetComponent<Furniture>();
        }

        return false;
    }
}
