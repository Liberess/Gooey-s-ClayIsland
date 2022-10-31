using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger2 : MonoBehaviour
{
    public void Change1()
    {
        Hun.Manager.GameManager.Instance.LoadScene("LobbyScene3");
    }
}