using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClayBlockType
{
    Grass = 0, Mud, Sand, Ice, Lime, Oil, Stone, Water, ShineLamp, Apple
}

public enum TemperObjectType
{
    Canon = 0, Trampoline
}

public abstract class ClayBlock : MonoBehaviour
{
    [SerializeField] protected ClayBlockType clayBlockType;
    public ClayBlockType ClayBlockType { get => clayBlockType; }

    public bool IsMouthful => clayBlockType != ClayBlockType.Stone;

    public abstract void OnEnter();
    public abstract void OnStay();
    public abstract void OnExit();

    /// <summary>
    /// �÷��̾ �ӱݱ⸦ �ϸ� ȣ��Ǵ� �޼���
    /// </summary>
    public virtual void OnMouthful()
    {

    }
    
    /// <summary>
    /// �÷��̾ ��⸦ �ϸ� ȣ��Ǵ� �޼���
    /// </summary>
    public virtual void OnSpit(Vector3 targetPos)
    {
        transform.rotation = Quaternion.identity;
    }

    public virtual void OnFusion(ClayBlock blockA, ClayBlock blockB)
    {
        Destroy(blockB);
        Destroy(blockB.gameObject);
        Destroy(blockA);
        Destroy(blockA.gameObject);
    }
}