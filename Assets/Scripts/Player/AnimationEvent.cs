using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void FootstepSound()
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepSoft);
    }
}
