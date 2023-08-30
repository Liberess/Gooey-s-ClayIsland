using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using Hun.Manager;

public class SceneController : MonoBehaviour
{
    private int sceneIndex;
    private bool isProgressing = false;
    [SerializeField] private string LobbySceneName;

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        InputSystem.onAnyButtonPress.CallOnce(x => OnGoToLobby());
    }

    public void OnGoToLobby()
    {
        if (isProgressing)
            return;
        
        isProgressing = true;
        VFXManager.Instance.CloudFadeOut();
        DataManager.Instance.GameData.gameState = GameState.Lobby;
        StartCoroutine(LoadScene(LobbySceneName));
    }

    public void ChangePreviousLobbyScene()
    {
        if (isProgressing)
            return;
        
        isProgressing = true;
        VFXManager.Instance.CloudFadeOut();
        StartCoroutine(LoadScene(sceneIndex - 1));
    }

    public void ChangeNextLobbyScene()
    {
        if (isProgressing)
            return;
        
        isProgressing = true;
        VFXManager.Instance.CloudFadeOut();
        StartCoroutine(LoadScene(sceneIndex + 1));
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
