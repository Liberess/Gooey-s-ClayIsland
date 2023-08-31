using Hun.Item;
using Hun.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockItem : MonoBehaviour, IItem
{
    public void OnEnter()
    {
        UseItem();
    }

    public void OnExit()
    {
        
    }

    public void UseItem()
    {
        //게임매니저 시간 20초 추가
        GameManager.Instance.GetCountTime(20);
        AudioManager.Instance.PlayOneShotSFX(ESFXName.GetItem);
        Destroy(gameObject);
    }
}