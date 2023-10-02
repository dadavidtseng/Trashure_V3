using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AudioManager : Singleton<AudioManager>
{
#region 宣告
    [Header("音樂數據庫")] 
    public SoundDetailsList_SO soundDetailsData;
    public SceneSoundList_SO sceneSoundData;

    [Header("Audio Source")] 
    public AudioSource ambientSource;
    public AudioSource gameSource;

    public float MusicStartSecond => Random.Range(5f, 15f);
    private Coroutine soundRoutine;

    [Header("Audio Mixer")] 
    public AudioMixer audioMixer;
    
    [Header("Snapshots")] 
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot ambientSnapshot;
    public AudioMixerSnapshot muteSnapshot;

    private float musicTransitionSecond = 8f;
#endregion

#region 事件函數
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent; 
        EventHandler.PlaySoundEvent -= OnPlaySoundEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }
#endregion

#region 註冊事件
    private void OnAfterSceneLoadedEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);
        if (sceneSound == null)
        {
            return;
        }

        SoundDetails ambient = soundDetailsData.GetSoundDetails(sceneSound.ambient);
        SoundDetails music = soundDetailsData.GetSoundDetails(sceneSound.music);

        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }

        soundRoutine = StartCoroutine(PlaySoundRoutine(ambient, music));
    }
    
    private void OnPlaySoundEvent(SoundName soundName)
    {
        var soundDetails = soundDetailsData.GetSoundDetails(soundName);
        if (soundDetails != null)
        {
            EventHandler.CallInitSoundEffect(soundDetails);
        }
    }
    
    private void OnEndGameEvent()
    {
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }
        
        muteSnapshot.TransitionTo(1f);
    }
#endregion

    private IEnumerator PlaySoundRoutine(SoundDetails ambient, SoundDetails music)
    {
        if (ambient != null && music != null)
        {
            PlayAmbientClip(ambient, 1f);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayMusicClip(music, musicTransitionSecond);
        }
    }

    /// <summary>
    /// 播放背景音樂
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayMusicClip(SoundDetails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(soundDetails.soundVolume));
        gameSource.clip = soundDetails.soundClip;
        if (gameSource.isActiveAndEnabled)
        {
            gameSource.Play();
        }

        normalSnapshot.TransitionTo(transitionTime);
    }
    
    /// <summary>
    /// 播放環境音樂
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientClip(SoundDetails soundDetails,  float transitionTime)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(soundDetails.soundVolume));
        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled)
        {
            ambientSource.Play();

            ambientSnapshot.TransitionTo(transitionTime);
        }
    }

    private float ConvertSoundVolume(float amount)
    {
        return (amount * 100 - 80);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", (value * 100 - 80));
    }
}
