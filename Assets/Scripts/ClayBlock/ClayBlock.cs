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

    public ClayBlock[] currentClayBlocks = new ClayBlock[2];

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
        blockA.gameObject.SetActive(false);
        blockB.gameObject.SetActive(false);
        //Destroy(blockB);
        //Destroy(blockB.gameObject);
        //Destroy(blockA);
        //Destroy(blockA.gameObject);
    }

    public virtual void OnDivision()
    {
        var player = FindObjectOfType<Hun.Player.PlayerMouthful>();

        var srcClayBlock = currentClayBlocks[0];
        var destClayBlock = currentClayBlocks[1];
/*
        var upPos1 = (srcClayBlock.transform.localScale * 0.5f)
            + srcClayBlock.transform.up;

        var upPos2 = (destClayBlock.transform.localScale * 0.5f)
            + destClayBlock.transform.up;
*/
        if(srcClayBlock.clayBlockType == ClayBlockType.Sand)
        {
            player.SetTargetClayBlock(srcClayBlock);
            SetClayBlock(true, player);
        }

        if (srcClayBlock.clayBlockType == ClayBlockType.Ice
    && destClayBlock.clayBlockType == ClayBlockType.Sand)
        {
            player.SetTargetClayBlock(destClayBlock);
            SetClayBlock(false, player);
        }
        else
        {
            player.SetTargetClayBlock(srcClayBlock);
            SetClayBlock(true, player);
        }

        if (srcClayBlock.clayBlockType == ClayBlockType.Grass
            && (destClayBlock.clayBlockType == ClayBlockType.Sand ||
            destClayBlock.clayBlockType == ClayBlockType.Ice))
        {
            player.SetTargetClayBlock(destClayBlock);
            SetClayBlock(false, player);
        }
        else
        {
            player.SetTargetClayBlock(srcClayBlock);
            SetClayBlock(true, player);
        }

        Destroy(gameObject);
    }

    private void SetClayBlock(bool isSrc, Hun.Player.PlayerMouthful player)
    {
        if (isSrc)
        {
            currentClayBlocks[0].transform.SetParent(player.transform);
            currentClayBlocks[0].gameObject.SetActive(false);

            currentClayBlocks[1].transform.SetParent(null);
            currentClayBlocks[1].transform.position = transform.position;
            currentClayBlocks[1].gameObject.SetActive(true);
        }
        else
        {
            currentClayBlocks[0].transform.SetParent(null);
            currentClayBlocks[0].transform.position = transform.position;
            currentClayBlocks[0].gameObject.SetActive(true);

            currentClayBlocks[1].transform.SetParent(player.transform);
            currentClayBlocks[1].gameObject.SetActive(false);
        }
    }
}