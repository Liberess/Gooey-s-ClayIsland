using Hun.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    [SerializeField] Animator effectAnim;

    [SerializeField] GameObject clayEffect;
    [SerializeField] AudioSource intro;

    private bool isStart;
    private int sceneIndex;

    private void Start()
    {
        isStart = false;

        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        if(!isStart && sceneIndex == 0)
        {
            if (Input.GetKeyDown("space"))
            {
                isStart = true;
                intro.Play();
                //effectAnim.SetTrigger("GameStart");
                VFXManager.Instance.CloudFadeOut();
                Invoke("GameStart", 2.5f);
                DataManager.Instance.GameData.gameState = GameState.Lobby;
            }
        }
    }

    private void GameStart()
    {
        SceneManager.LoadScene("LobbyScene1");
    }

    public void StageStart()
    {
        clayEffect.SetActive(true);
        //clayEffect.GetComponent<Animator>().SetTrigger("GameEnd");
        VFXManager.Instance.CloudFadeOut();
        Invoke(nameof(LoadStage), 3f);
    }

    private void LoadStage()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        switch(sceneIndex)
        {
            case 2: sceneName = "1-1"; break;
            case 3: sceneName = "1-2"; break;
            case 4: sceneName = "1-3"; break;
        }

        DataManager.Instance.GameData.gameState = GameState.Stage;
        SceneManager.LoadScene(sceneName);
    }
}
