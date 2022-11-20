using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Newtonsoft.Json.Linq;
using System;
using Hun.Manager;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    public Sound(string _name, AudioClip _clip)
    {
        name = _name;
        clip = _clip;
    }
}

public enum BGMNames
{
    Main,
    InGame,
    GameWin,
    GameFailed,
}

public enum SFXNames
{
    Button,
    Click,
    Walk,
    Fall
}

public class AudioManager : MonoBehaviour
{
    #region 인스턴스화
    private static AudioManager m_Instance;
    public static AudioManager Instance
    {
        get
        {
            /*            if (!m_Instance)
                        {
                            m_Container = new GameObject();
                            m_Container.name = "AudioManager";
                            m_Instance = m_Container.AddComponent(
                                typeof(AudioManager)) as AudioManager;
                            DontDestroyOnLoad(m_Container);
                        }*/

            return m_Instance;
        }
    }

    private static GameObject m_Container;
    #endregion

    [Header("== Setting Audio Controller ==")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Space(10), Header("== Setting Audio Clip ==")]
    [SerializeField] private List<Sound> bgm = new List<Sound>();
    [SerializeField] private List<Sound> sfx = new List<Sound>();

    [Space(10), Header("== Setting Audio Player ==")]
    [SerializeField] private AudioSource bgmPlayer = null;
    [SerializeField] private AudioSource[] sfxPlayer = null;

    [Space(10), Header("== Setting Audio UI ==")]
    [SerializeField] private Text bgmNumTxt;
    [SerializeField] private Text sfxNumTxt;


    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        InitAudioSetting();
    }

    #region Setup DataField
    [ContextMenu("Setup Audio UI")]
    public void SetAudioObjects()
    {
        if (optionPanel == null)
            optionPanel = GameObject.Find("OptionCanvas").transform.GetChild(0).gameObject;

        var parent = optionPanel.transform.GetChild(0).gameObject;

        if (bgmSlider == null || sfxSlider == null)
        {
            bgmSlider = parent.transform.Find("BGMSlider").GetComponent<Slider>();
            sfxSlider = parent.transform.Find("SFXSlider").GetComponent<Slider>();
        }

        if (bgmNumTxt == null || sfxNumTxt == null)
        {
            bgmNumTxt = bgmSlider.transform.Find("NumTxt").GetComponent<Text>();
            sfxNumTxt = sfxSlider.transform.Find("NumTxt").GetComponent<Text>();
        }
    }

    public void InitAudioSetting()
    {
        SetAudioObjects();

        bgmSlider.maxValue = 100f;
        sfxSlider.maxValue = 100f;

        bgmSlider.value = DataManager.Instance.GameData.bgm;
        sfxSlider.value = DataManager.Instance.GameData.sfx;

        bgmNumTxt.text = Mathf.RoundToInt(bgmSlider.value).ToString();
        sfxNumTxt.text = Mathf.RoundToInt(sfxSlider.value).ToString();

        masterMixer.SetFloat("BGM", bgmSlider.value / 100f);
        masterMixer.SetFloat("SFX", sfxSlider.value / 100f);
    }
    #endregion

    #region UpdateAudioPlayer
    [ContextMenu("Update Audio Player Source")]
    private void UpdateAudioPlayer()
    {
        UpdateBGMPlayer();
        UpdateSFXPlayer();
    }

    private void UpdateBGMPlayer()
    {
        var sfxChild = transform.GetChild(0);
        bgmPlayer = sfxChild.GetComponent<AudioSource>();
    }

    private void UpdateSFXPlayer()
    {
        var sfxChild = transform.GetChild(1);
        sfxPlayer = sfxChild.GetComponents<AudioSource>();
    }
    #endregion

    #region Update Audio Clip
    [ContextMenu("Update Audio Clip Resource")]
    private void UpdateAudioClip()
    {
        UpdateBGMResource();
        UpdateSFXResource();
    }

    private void UpdateBGMResource()
    {
        bgm.Clear();

        AudioClip[] srcs = Resources.LoadAll<AudioClip>("Audio/BGM");

        foreach (var src in srcs)
            bgm.Add(new Sound(src.name.Substring(6), src));
    }

    private void UpdateSFXResource()
    {
        sfx.Clear();

        AudioClip[] srcs = Resources.LoadAll<AudioClip>("Audio/SFX");

        foreach (var src in srcs)
            sfx.Add(new Sound(src.name.Substring(6), src));
    }
    #endregion

    #region Audio Save
    public void BGMSave()
    {
        bgmNumTxt.text = Mathf.RoundToInt(bgmSlider.value).ToString();
        bgmPlayer.volume = bgmSlider.value / 100f;
        DataManager.Instance.GameData.bgm = bgmSlider.value;
    }

    public void SFXSave()
    {
        for (int i = 0; i < sfxPlayer.Length; i++)
            sfxPlayer[i].volume = sfxSlider.value / 100f;

        sfxNumTxt.text = Mathf.RoundToInt(sfxSlider.value).ToString();
        DataManager.Instance.GameData.sfx = sfxSlider.value;
    }
    #endregion

    #region Audio Play & Stop
    public void PlayBGM(BGMNames _name)
    {
        var bgmName = _name.ToString();

        if (bgmPlayer.clip != null && bgmPlayer.clip.name == bgmName)
            return;

        for (int i = 0; i < bgm.Count; i++)
        {
            if (bgmName == bgm[i].name)
            {
                bgmPlayer.clip = bgm[i].clip;
                bgmPlayer.Play();
            }
        }
    }

    public void StopBGM() => bgmPlayer.Stop();

    public void PlaySFX(SFXNames _name)
    {
        var sfxName = _name.ToString();

        for (int i = 0; i < sfx.Count; i++)
        {
            if (sfxName == sfx[i].name)
            {
                for (int x = 0; x < sfxPlayer.Length; x++)
                {
                    if (!sfxPlayer[x].isPlaying)
                    {
                        sfxPlayer[x].clip = sfx[i].clip;
                        sfxPlayer[x].Play();
                        return;
                    }
                }
                return;
            }
        }
    }

    public void StopSFX(SFXNames _name)
    {
        var sfxName = _name.ToString();

        for (int i = 0; i < sfx.Count; i++)
        {
            if (sfxName == sfx[i].name)
            {
                for (int x = 0; x < sfxPlayer.Length; x++)
                {
                    if (sfxPlayer[x].isPlaying && sfxPlayer[x].clip == sfx[i].clip)
                    {
                        sfxPlayer[x].Stop();
                        sfxPlayer[x].clip = null;
                    }
                }
                return;
            }
        }
    }
    #endregion

    public bool SetActiveOptionPanel()
    {
        if (optionPanel.activeSelf)
        {
            optionPanel.SetActive(false);
            return false;
        }
        else
        {
            optionPanel.SetActive(true);
            return true;
        }
    }

    public AudioClip GetBGMClip(BGMNames bgmName) => bgm[(int)bgmName].clip;
    public AudioClip GetSFXClip(SFXNames sfxName) => bgm[(int)sfxName].clip;
}