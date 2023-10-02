using System;
using UnityEngine;
using UnityEngine.UI;

namespace Trashure.Inventory
{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;
        public Text itemName;
        public InputField tradeAmount;
        public Button submitButton;
        public Button cancelButton;

        private ItemDetails item;
        private bool isSellTrade;

        private void Awake()
        {
            cancelButton.onClick.AddListener(CancelTrade);
            submitButton.onClick.AddListener(TradeItem);
        }

        /// <summary>
        /// 設置TradeUI顯示詳情
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isSell"></param>
        public void SetUpTradeUI(ItemDetails item, bool isSell)
        {
            this.item = item;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            isSellTrade = isSell;
            tradeAmount.text = string.Empty;
        }

        private void TradeItem()
        {
            var amount = Convert.ToInt32(tradeAmount.text);

            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);
            
            CancelTrade();
        }
        
        private void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }
    }
}
