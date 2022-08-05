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
    /// 플레이어가 머금기를 하면 호출되는 메서드
    /// </summary>
    public virtual void OnMouthful()
    {

    }
    
    /// <summary>
    /// 플레이어가 뱉기를 하면 호출되는 메서드
    /// </summary>
    public virtual void OnSpit(Vector3 targetPos)
    {
        transform.rotation = Quaternion.identity;
    }

    public virtual void OnFusion(ClayBlock blockA, ClayBlock blockB)
    {
        Destroy(blockB);
        Destroy(blockA);
        Destroy(blockA.gameObject);
    }
}