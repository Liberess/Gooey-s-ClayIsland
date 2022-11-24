using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private int sceneIndex;

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void ChangePreviousLobbyScene()
    {
        SceneManager.LoadScene(sceneIndex - 1);
    }

    public void ChangeNextLobbyScene()
    {
        SceneManager.LoadScene(sceneIndex + 1);
    }
}