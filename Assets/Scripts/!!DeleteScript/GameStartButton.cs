using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    [SerializeField] Animator effectAnim;

    [SerializeField] GameObject clayEffect;
    [SerializeField] AudioSource intro;

    private int stageNum;

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
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
        SceneManager.LoadScene("LobbyScene1");
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
            SceneManager.LoadScene("1-5");
    }
}
