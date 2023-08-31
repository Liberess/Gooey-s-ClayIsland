using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using FMODPlus;
using NaughtyAttributes;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public enum EAudioType
{
    Master, 
    BGM, 
    SFX, 
    SUI
}

public enum EBGMName
{
    Main,
    Lobby,
    Stage
}

public enum ESFXName
{
    Mouthful,
    Spit,
    Walk,
    Run,
    Canon,
    Trampiline,
    TemperFusion,
    TemperDivision,
    GetItem,
    SlipIce
}

public enum ESUIName
{
    TitleBtn,
    StartBtn,
    Result,
    ResultBtn
}

public class AudioManager : MonoBehaviour
{
    #region Public

    public static AudioManager Instance { get; private set; }
    
    [BoxGroup("Audio Emitter")] public FMODAudioSource bgmAudioSrc;
    [BoxGroup("Audio Emitter")] public FMODAudioSource footstepAudioSrc;
    [BoxGroup("Bus")] public string[] buses;
    
    [BoxGroup("Audio Clips")] public EventReference[] bgmPaths;
    [BoxGroup("Audio Clips")] public EventReference[] sfxPaths;
    [BoxGroup("Audio Clips")] public EventReference[] suiPaths;
    
    #endregion

    #region Private

    private EventInstance bgmInst;
    private EventInstance footstepInst;
    private Bus[] _buses;

    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        _buses = new Bus[]
        {
            RuntimeManager.GetBus(buses[0]),
            RuntimeManager.GetBus(buses[1]),
            RuntimeManager.GetBus(buses[2]),
            RuntimeManager.GetBus(buses[3]),
        };
    }

    private void Start()
    {
        footstepInst = RuntimeManager.CreateInstance(sfxPaths[(int)ESFXName.Walk]);
    }

    /// <summary>
    /// Let the sound play.
    /// </summary>
    public void PlayBGM() => bgmAudioSrc.Play();

    public void PlayBGM(EBGMName bgmName)
    {
        bgmAudioSrc.Clip = bgmPaths[(int)bgmName];
        bgmAudioSrc.Play();
    }

    /// <summary>
    /// Returns whether the background music is paused.
    /// </summary>
    /// <returns></returns>
    public bool IsPlayingBGM() => bgmAudioSrc.IsPlaying();

    /// <summary>
    /// Stop the sound.
    /// </summary>
    /// <param name="fadeOut">true이면 페이드를 합니다.</param>
    public void StopBGM(bool fadeOut = false)
    {
        bgmAudioSrc.AllowFadeout = fadeOut;
        _buses[(int)EAudioType.BGM].stopAllEvents(STOP_MODE.ALLOWFADEOUT);
        bgmAudioSrc.Stop();
    }

    /// <summary>
    /// Pause or resume playing the sound.
    /// </summary>
    /// <param name="pause">true면 정지하고, false면 다시 재생합니다.</param>
    public void SetPauseBGM(bool pause)
    {
        if (pause)
            bgmAudioSrc.Pause();
        else
            bgmAudioSrc.UnPause();
    }

    /// <summary>
    /// Adjust the Audio's volume.
    /// </summary>
    /// <param name="type">설정할 대상 오디오의 타입.</param>
    /// <param name="value">0~1사이의 값, 0이면 뮤트됩니다.</param>
    public void SetVolume(EAudioType type, float value) => _buses[(int)type].setVolume(value);
    
    /// <summary>
    /// Call Key Off when using Sustain Key Point.
    /// </summary>
    public void KeyOff()
    {
        bgmAudioSrc.EventInstance.keyOff();
    }

    /// <summary>
    /// Call Key Off when using Sustain Key Point.
    /// </summary>
    public void TriggerCue()
    {
        KeyOff();
    }

    public void PlayFootstep(bool isWalk)
    {
        var curClip = footstepAudioSrc.Clip;
        var nextClip = isWalk ? sfxPaths[(int)ESFXName.Walk] : sfxPaths[(int)ESFXName.Run];
        if (curClip.Guid == nextClip.Guid)
        {
           if(!footstepAudioSrc.IsPlaying())
               footstepAudioSrc.Play();
        }
        else
        {
            footstepAudioSrc.Clip = nextClip;
            footstepAudioSrc.Stop();
            footstepAudioSrc.Play();
        }
    }

    /// <summary>
    /// Create an instance in-place, play a sound effect, and destroy it immediately.
    /// </summary>
    /// <param name="path">재생할 효과음 경로</param>
    /// <param name="position">해당 위치에서 소리를 재생합니다.</param>
    public void PlayOneShot(EventReference path, Vector3 position = default)
    {
        RuntimeManager.PlayOneShot(path, position);
    }
    
    /// <summary>
    /// Create an instance in-place, play a sound effect, and destroy it immediately.
    /// </summary>
    /// <param name="sfxName">재생할 효과음</param>
    /// <param name="position">해당 위치에서 소리를 재생합니다.</param>
    public void PlayOneShotSFX(ESFXName sfxName, Vector3 position = default)
    {
        //RuntimeManager.
        RuntimeManager.PlayOneShot(sfxPaths[(int)sfxName], position);
    }
    
    /// <summary>
    /// Create an instance in-place, play a sound effect, and destroy it immediately.
    /// </summary>
    /// <param name="suiName">재생할 UI 효과음 경로</param>
    /// <param name="position">해당 위치에서 소리를 재생합니다.</param>
    public void PlayOneShotSUI(ESUIName suiName, Vector3 position = default)
    {
        RuntimeManager.PlayOneShot(suiPaths[(int)suiName], position);
    }

    /// <summary>
    /// 파라미터를 호환하고 인스턴스를 내부에서 만들어서 효과음을 재생하고, 즉시 파괴합니다.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    /// <param name="position"></param>
    public void PlayOneShot(EventReference path, string parameterName, float parameterValue,
        Vector3 position = new Vector3())
    {
        try
        {
            PlayOneShot(path.Guid, parameterName, parameterValue, position);
        }
        catch (EventNotFoundException)
        {
            RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
        }
    }

    /// <summary>
    /// Parameter compatible, create instance internally, play sound effect, destroy immediately.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    /// <param name="position"></param>
    public void PlayOneShot(string path, string parameterName, float parameterValue,
        Vector3 position = new Vector3())
    {
        try
        {
            PlayOneShot(RuntimeManager.PathToGUID(path), parameterName, parameterValue, position);
        }
        catch (EventNotFoundException)
        {
            RuntimeUtils.DebugLogWarning("[FMOD] Event not found: " + path);
        }
    }

    private void PlayOneShot(FMOD.GUID guid, string parameterName, float parameterValue,
        Vector3 position = new Vector3())
    {
        var instance = RuntimeManager.CreateInstance(guid);
        instance.set3DAttributes(position.To3DAttributes());
        instance.setParameterByName(parameterName, parameterValue);
        instance.start();
        instance.release();
    }
}