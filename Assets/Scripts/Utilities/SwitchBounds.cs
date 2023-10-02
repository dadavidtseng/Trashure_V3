using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
#region 事件函數
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SwitchConfinerShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchConfinerShape;
    }
#endregion

#region 註冊事件
    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = 
            GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        
        confiner.m_BoundingShape2D = confinerShape;
        
        //Call this if the bounding shape's shape change at runtime
        confiner.InvalidatePathCache();
    }
#endregion
}
