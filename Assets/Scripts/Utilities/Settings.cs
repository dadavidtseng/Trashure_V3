using System;
using UnityEngine;

public class Settings
{
    public const float itemfadeDuration = 0.35f;
    public const float targetAlpha = 0.45f;
    
    //時間相關
    public const float secondThreshold = 0.01f;    //數值越小時間越快
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 30;
    public const int seasonHold = 3;
    
    //Transition
    public const float fadeDuration = 0.8f;
    
    //割草數量限制
    public const int reapAmount = 2;
    
    //NPC網格移動
    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f;    //20*20占1unit
    public const float animationBreakTime = 5f;    //動畫間隔
    public const int maxGridSize = 9999;
    
    //燈光
    public const float lightChangDuration = 25f;
    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);

    public static Vector3 playerStartPos = new Vector3(-7.5f, -10f, 0);
    public const int playerStartMoney = 100;
}
