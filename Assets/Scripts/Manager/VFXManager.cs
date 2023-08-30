using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }
    
    private VFXClay vfxClay;
    private VFXCloud vfxCloud;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        vfxCloud = FindObjectOfType<VFXCloud>();
        if(vfxCloud)
            vfxCloud.FadeInAnimation();

        vfxClay = FindObjectOfType<VFXClay>();
        if (vfxClay)
            vfxClay.FadeInAnimation();
    }

    public void CloudFadeIn() => vfxCloud.FadeInAnimation();

    public void CloudFadeOut() => vfxCloud.FadeOutAnimation();

    public void ClayFadeIn() => vfxClay.FadeInAnimation();

    public void ClayFadeOut() => vfxClay.FadeOutAnimation();
}