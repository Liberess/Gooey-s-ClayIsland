using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClayBlockType
{
    Grass = 0, Mud, Sand, Ice, Lime, Oil, Stone
}

public class ClayBlock : MonoBehaviour
{
    [SerializeField] protected ClayBlockType clayBlockType;
    public ClayBlockType ClayBlockType { get => clayBlockType; }

    public bool IsMouthful => clayBlockType != ClayBlockType.Stone;

    public void OnEnter()
    {

    }

    public void OnStay()
    {

    }

    public void OnExit()
    {

    }

    #region Mouthful - Spit - Fusion

    /// <summary>
    /// 플레이어가 머금기를 하면 호출되는 메서드
    /// </summary>
    public virtual void OnMouthful()
    {
        if (!IsMouthful)
            return;

        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 플레이어가 뱉기를 하면 호출되는 메서드
    /// </summary>
    public virtual void OnSpit(Vector3 targetPos)
    {
        if (!IsMouthful)
            return;

        gameObject.transform.position = targetPos;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
    }

    public virtual void OnFusion()
    {
        if (!IsMouthful)
            return;

        Destroy(gameObject);
    }

    #endregion
}