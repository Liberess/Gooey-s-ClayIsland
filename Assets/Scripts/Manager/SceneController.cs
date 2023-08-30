using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Hun.Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private int sceneIndex;
    private bool isProgressing = false;

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        AudioManager.Instance.PlayBGM((EBGMName)DataManager.Instance.GameData.gameState);

        if(sceneIndex == 0)
            InputSystem.onAnyButtonPress.CallOnce(x => OnGoToLobby());
    }

    public void OnGoToLobby()
    {
        if (isProgressing)
            return;
        
        isProgressing = true;
        DataManager.Instance.GameData.gameState = GameState.Lobby;
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayOneShotSUI(ESUIName.TitleBtn);
        VFXManager.Instance.CloudFadeOut();
        StartCoroutine(LoadScene("Lobby"));
    }

    private IEnumerator LoadScene(int stageIndex, float delay = 2.0f)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(stageIndex);
    }
    
    private IEnumerator LoadScene(string stageName, float delay = 2.0f)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(stageName);
    }
}
