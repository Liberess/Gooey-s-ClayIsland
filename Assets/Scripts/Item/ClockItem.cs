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
        Debug.Log("게임매니저 시간 20초 추가");
        Debug.Log("사운드 s019 1회 재생");
        Destroy(gameObject);
    }
}