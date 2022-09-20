using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton1 : MonoBehaviour
{
    [SerializeField] Animator effectAnim;

    [SerializeField] GameObject clayEffect;
    [SerializeField] AudioSource intro;

    private int stageNum;

    public void StageStart(int stageNum)
    {
        this.stageNum = stageNum;
        clayEffect.SetActive(true);
        Invoke("StageStart", 3f);
    }

    private void StageStart()
    {
        if(stageNum == 2)
            Hun.Manager.GameManager.Instance.LoadScene("1-6");
    }
}
