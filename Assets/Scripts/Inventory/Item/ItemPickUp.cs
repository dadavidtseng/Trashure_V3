using UnityEngine;

namespace Trashure.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
#region 事件函數
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item = other.GetComponent<Item>();

            if (item != null)
            {
                if (item.itemDetails.canPickup)
                {
                    //拾取物品添加到背包
                    InventoryManager.Instance.AddItem(item, true);
                    
                    //播放音效
                    EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
#endregion
    }
}


