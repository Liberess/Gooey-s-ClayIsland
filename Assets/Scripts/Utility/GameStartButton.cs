using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            Hun.Manager.GameManager.Instance.LoadScene("WorldMapScene");
        }
    }
}
