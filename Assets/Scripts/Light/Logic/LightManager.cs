using UnityEngine;

public class LightManager : MonoBehaviour
{
#region 宣告
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currentSeason;
    private float timeDifference = Settings.lightChangDuration;
#endregion

#region 事件函數
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }
#endregion

#region 註冊事件
    private void OnAfterSceneLoadedEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();

        foreach (LightControl light in sceneLights)
        {
            //LightControl改變燈光的方法
            light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
        }
    }
    
    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        currentSeason = season;
        this.timeDifference = timeDifference;

        if (currentLightShift != lightShift)
        { 
            currentLightShift = lightShift;

            foreach (LightControl light in sceneLights)
            {
                //LightControl改變燈光的方法
                light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
            }
        }
    }
    
    private void OnStartNewGameEvent(int obj)
    {
        currentLightShift = LightShift.Morning;
    }
#endregion
}
