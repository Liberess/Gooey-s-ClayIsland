using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : MonoBehaviour
{
    [SerializeField] Animator effectAnim;

    [SerializeField] GameObject clayEffect;
    [SerializeField] AudioSource intro;

    private int stageNum;

    void Update()
    {
        if(Hun.Manager.GameManager.Instance.SceneIndex == 0)
        {
            if (Input.GetKeyDown("space"))
            {
                intro.Play();
                effectAnim.SetTrigger("GameStart");
                Invoke("GameStart", 2.5f);
            }
        }
    }

    private void GameStart()
    {
        Hun.Manager.GameManager.Instance.LoadScene("LobbyScene");
    }

    public void StageStart(int stageNum)
    {
        this.stageNum = stageNum;
        clayEffect.SetActive(true);
        Invoke("StageStart", 3f);
    }

    private void StageStart()
    {
        if(stageNum == 1)
            Hun.Manager.GameManager.Instance.LoadScene("1-5");
    }
}
