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
        //���ӸŴ��� �ð� 20�� �߰�
        Debug.Log("���ӸŴ��� �ð� 20�� �߰�");
        Debug.Log("���� s019 1ȸ ���");
        Destroy(gameObject);
    }
}