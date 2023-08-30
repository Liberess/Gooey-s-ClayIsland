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
    [SerializeField] private string LobbySceneName;

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex != 0)
            AudioManager.Instance.PlayBGM((EBGMName)DataManager.Instance.GameData.gameState);

        if(sceneIndex == 0)
            InputSystem.onAnyButtonPress.CallOnce(x => OnGoToLobby());
    }

    public void OnGoToLobby()
    {
        if (isProgressing)
            return;
        
        isProgressing = true;
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayOneShotSUI(ESUIName.TitleBtn);
        VFXManager.Instance.CloudFadeOut();
        StartCoroutine(LoadScene(LobbySceneName));
    }

    private IEnumerator LoadScene(int stageIndex, float delay = 2.0f)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(stageIndex);
    }
    
    private IEnumerator LoadScene(string stageName, float delay = 2.0f)
    {
        yield return new WaitForSeconds(delay);
        DataManager.Instance.GameData.gameState = GameState.Lobby;
        Hun.Manager.LoadingManager.LoadScene(stageName);
    }
}
