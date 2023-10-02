using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LightControl : MonoBehaviour
{
    public LightPatternList_SO lightData;
    private Light2D currentLight;
    private LightDetails currentLightDetails;

    private void Awake()
    {
        currentLight = GetComponent<Light2D>();
    }
    
    //實際切換燈光
    public void ChangeLightShift(Season season, LightShift lightShift, float timeDifference)
    {
        currentLightDetails = lightData.GetLightDetails(season, lightShift);

        if (timeDifference < Settings.lightChangDuration)
        {
            var colorOffset = (currentLightDetails.lightColor - currentLight.color) / Settings.lightChangDuration *
                              timeDifference;
            currentLight.color += colorOffset;
            DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightDetails.lightColor,
                Settings.lightChangDuration - timeDifference);
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightDetails.lightAmount,
                Settings.lightChangDuration - timeDifference);
        }

        if (timeDifference >= Settings.lightChangDuration)
        {
            currentLight.color = currentLightDetails.lightColor;
            currentLight.intensity = currentLightDetails.lightAmount;
        }
    }
}
