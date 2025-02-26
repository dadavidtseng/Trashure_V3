using UnityEngine;

public class TriggerItemFader : MonoBehaviour
{
#region 事件函數
    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemFader[] faders = other.GetComponentsInChildren<ItemFader>();

        if (faders.Length > 0)
        {
            foreach (var item in faders)
            {
                item.FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ItemFader[] faders = other.GetComponentsInChildren<ItemFader>();

        if (faders.Length > 0)
        {
            foreach (var item in faders)
            {
                item.FadeIn();
            }
        }
    }
#endregion
}
