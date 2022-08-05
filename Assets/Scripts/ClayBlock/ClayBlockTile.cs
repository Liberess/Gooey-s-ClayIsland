using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hun.Manager;

public class ClayBlockTile : ClayBlock
{
    private Hun.Player.PlayerController playerCtrl;

    private GameObject currentTemperPrefab = null;

    private void Awake()
    {
        playerCtrl = FindObjectOfType<Hun.Player.PlayerController>();
    }

    public override void OnEnter()
    {
        switch (clayBlockType)
        {
            case ClayBlockType.Grass:
                break;
            case ClayBlockType.Mud:
                break;
            case ClayBlockType.Sand:
                break;
            case ClayBlockType.Ice:
                var dir = (playerCtrl.transform.position - transform.position).normalized;
                Vector3 dirVec = Vector3.zero;

                if(Mathf.Abs(dir.z) > Mathf.Abs(dir.x))
                {
                    if (dir.z > 0)
                        dirVec = Vector3.back;
                    else
                        dirVec = Vector3.forward;
                }
                else
                {
                    if (dir.x > 0)
                        dirVec = Vector3.left;
                    else
                        dirVec = Vector3.right;
                }

                playerCtrl.PlayerMovement.AddMoveForce(dirVec);
                break;
            case ClayBlockType.Lime:
                break;
            case ClayBlockType.Oil:
                break;
            case ClayBlockType.Stone:
                break;
            case ClayBlockType.Water:
                playerCtrl.PlayerMovement.InitializeMovingVector();
                playerCtrl.PlayerMovement.playerGravityY = 0.001f;
                break;
        }
    }

    public override void OnStay()
    {
        switch (clayBlockType)
        {
            case ClayBlockType.Grass:
                break;
            case ClayBlockType.Mud:
                break;
            case ClayBlockType.Sand:
                break;
            case ClayBlockType.Ice:
                break;
            case ClayBlockType.Lime:
                break;
            case ClayBlockType.Oil:
                break;
            case ClayBlockType.Stone:
                break;
            case ClayBlockType.Water:
                break;
        }
    }

    public override void OnExit()
    {
        switch (clayBlockType)
        {
            case ClayBlockType.Grass:
                break;
            case ClayBlockType.Mud:
                break;
            case ClayBlockType.Sand:
                break;
            case ClayBlockType.Ice:
                break;
            case ClayBlockType.Lime:
                break;
            case ClayBlockType.Oil:
                break;
            case ClayBlockType.Stone:
                break;
            case ClayBlockType.Water:
                playerCtrl.PlayerMovement.playerGravityY = 1f;
                break;
        }
    }

    public override void OnMouthful()
    {
        if (!IsMouthful)
            return;

        base.OnMouthful();

        gameObject.SetActive(false);
    }

    public override void OnSpit(Vector3 targetPos)
    {
        if (!IsMouthful)
            return;

        base.OnSpit(targetPos);

        gameObject.transform.position = targetPos;
        gameObject.SetActive(true);
    }

    public override void OnFusion(ClayBlock blockA, ClayBlock blockB)
    {
        Instantiate(currentTemperPrefab, blockB.transform.position, Quaternion.identity);
        base.OnFusion(blockA, blockB); //Destroy
    }

    public bool IsSuccessFusion(ClayBlock blockA, ClayBlock blockB)
    {
        bool isSuccess = false;

        switch (blockA.ClayBlockType)
        {
            case ClayBlockType.Grass:
                if (blockB.ClayBlockType == ClayBlockType.Grass)
                {
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.Canon);
                }
                else if(blockB.ClayBlockType == ClayBlockType.Sand)
                {
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.Trampoline);
                }
                isSuccess = true;
                break;
            case ClayBlockType.Mud:
                isSuccess = true;
                break;
            case ClayBlockType.Sand:
                if (blockB.ClayBlockType == ClayBlockType.Grass)
                {
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.Trampoline);
                }
                isSuccess = true;
                break;
            case ClayBlockType.Ice:
                isSuccess = true;
                break;
            case ClayBlockType.Lime:
                isSuccess = true;
                break;
            case ClayBlockType.Oil:
                isSuccess = true;
                break;
        }

        OnFusion(blockA, blockB);
        return isSuccess;
    }
}