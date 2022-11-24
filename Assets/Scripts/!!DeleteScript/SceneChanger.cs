using UnityEngine;
using System.Collections;
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
        VFXManager.Instance.CloudFadeOut();
        StartCoroutine(LoadScene(sceneIndex - 1));
    }

    private IEnumerator LoadScene(int index)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(index);
    }

    public void ChangeNextLobbyScene()
    {
        VFXManager.Instance.CloudFadeOut();
        StartCoroutine(LoadScene(sceneIndex + 1));
    }
}