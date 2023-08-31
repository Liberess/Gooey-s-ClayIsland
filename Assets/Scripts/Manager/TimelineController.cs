using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    public PlayableDirector PlayableDirector { get => playableDirector; }

    [SerializeField] private TimelineAsset timeline;

    [SerializeField] private Hun.TutorialPanel tutorialPanel;
    private bool isTutorialPlay = false;

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if(playableDirector.state != PlayState.Playing && isTutorialPlay == false)
        {
            if(tutorialPanel != null)
            {
                isTutorialPlay = true;
                tutorialPanel.gameObject.SetActive(true);
            } 
        }
    }

    public void Play()
    {
        playableDirector.Play();
    }
    public void PlayFromTimeline()
    {
        playableDirector.Play(timeline);
    }
    public void SetTime()
    {
        playableDirector.time = 1;
    }
}