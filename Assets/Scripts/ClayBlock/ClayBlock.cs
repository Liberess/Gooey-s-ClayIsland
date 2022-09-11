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

    public List<ClayBlock> currentClayBlockList = new List<ClayBlock>();

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
        List<Vector3> randPosList = new List<Vector3>();

        int layerMask = (1 << LayerMask.GetMask("ClayBlock")) +
            (1 << LayerMask.GetMask("TemperObject"));
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2,
            Quaternion.identity, layerMask);

        var boxCol = GetComponent<BoxCollider>();
        float rangeX = boxCol.bounds.size.x;
        float rangeZ = boxCol.bounds.size.z;
        
        foreach (var block in currentClayBlockList)
        {
            block.transform.SetParent(null);
            var upPos = (block.transform.localScale * 0.5f) + block.transform.up;

            for(int i = 0; i < 30; i++)
            {
                Vector3 targetPos = Hun.Utility.Utility.GetRandPointOnNavMesh(
                    transform.position + upPos, rangeX * 3f, UnityEngine.AI.NavMesh.AllAreas);

                for(int j = 0; j < randPosList.Count; j++)
                {
                    float distance = Vector3.Distance(targetPos, randPosList[j]);
                    if(distance > rangeX)
                    {
                        Debug.Log("1");
                        randPosList.Add(targetPos);
                        transform.position = targetPos;
                        block.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}