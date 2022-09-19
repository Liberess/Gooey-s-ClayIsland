using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : MonoBehaviour
{
    [SerializeField] Animator effectAnim;

    [SerializeField] GameObject clayEffect;

    private int stageNum;

    void Update()
    {
        if(Hun.Manager.GameManager.Instance.SceneIndex == 0)
        {
            if (Input.GetKeyDown("space"))
            {
                effectAnim.SetTrigger("GameStart");
                Invoke("GameStart", 3f);
            }
        }
    }

    private void GameStart()
    {
        Hun.Manager.GameManager.Instance.LoadScene("WorldMapScene");
    }

    public void StageStart(int stageNum)
    {
        this.stageNum = stageNum;
        clayEffect.SetActive(true);
        Invoke("StageStart", 2f);
    }

    private void StageStart()
    {
        if(stageNum == 1)
            Hun.Manager.GameManager.Instance.LoadScene("1-1");
    }
}
