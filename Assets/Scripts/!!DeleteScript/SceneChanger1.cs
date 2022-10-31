using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger1 : MonoBehaviour
{
    public void Change()
    {
        Hun.Manager.GameManager.Instance.LoadScene("LobbyScene2");
    }
}