using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClayBlockType
{
    Grass = 0, Mud, Sand, Ice, Lime, Oil
}

public abstract class ClayBlock : MonoBehaviour
{
    public ClayBlockType ClayBlockType { get; protected set; }

    public abstract void OnEnter();
    public abstract void OnStay();
    public abstract void OnExit();

    /// <summary>
    /// �÷��̾ �ӱݱ⸦ �ϸ� ȣ��Ǵ� �޼���
    /// </summary>
    public virtual void OnMouthful()
    {
        Debug.Log("OnMouthful");
    }
    
    /// <summary>
    /// �÷��̾ ��⸦ �ϸ� ȣ��Ǵ� �޼���
    /// </summary>
    public virtual void OnSpit(Vector3 targetPos)
    {
        Debug.Log("OnSpit");
    }
}