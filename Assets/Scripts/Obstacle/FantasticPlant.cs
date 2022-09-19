using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FantasticPlant : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var temp = other.GetComponent<Hun.Player.PlayerMouthful>();
        if (temp && temp.TargetClayBlock.ClayBlockType == ClayBlockType.ShineLamp)
        {
            Hun.Manager.GameManager.Instance.StageClear();
        }
    }
}
