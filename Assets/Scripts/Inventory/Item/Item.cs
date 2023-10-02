using UnityEngine;
using Trashure.CropPlant;

namespace Trashure.Inventory
{
    public class Item : MonoBehaviour
    {
        public int itemID;

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D coll;
        public ItemDetails itemDetails;
        
        public float interval = 10.0f;
        private float timer = 0.0f;
        public float startSecond = 5.0f;
        public float endsecond = 10.0f;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
            interval = Random.Range(startSecond, endsecond);
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > interval && itemID != 0)
            {
                Init(itemID);
                timer = 0.0f;
            }
        }

        public void Init(int ID)
        {
            itemID = ID;
            
            //Inventory獲得當前數據
            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);

            if (itemDetails != null)
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite != null
                    ? itemDetails.itemOnWorldSprite
                    : itemDetails.itemIcon;
                
                //修改碰撞體尺寸
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }

            if (itemDetails.itemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropData(itemDetails.itemID);
                gameObject.AddComponent<ItemInteractive>();
            }
        }
    }
}

