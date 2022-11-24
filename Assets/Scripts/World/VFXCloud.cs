using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXCloud : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim= GetComponent<Animator>();
    }

    public void FadeInAnimation() => anim.SetTrigger("doFadeIn");

    public void FadeOutAnimation() => anim.SetTrigger("doFadeOut");
}