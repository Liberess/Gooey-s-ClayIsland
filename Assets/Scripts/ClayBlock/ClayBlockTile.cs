using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayBlockTile : ClayBlock
{
    public override void OnEnter()
    {
        var player = FindObjectOfType<Hun.Player.Player>();

        switch (clayBlockType)
        {
            case ClayBlockType.Grass:
                break;
            case ClayBlockType.Mud:
                break;
            case ClayBlockType.Sand:
                break;
            case ClayBlockType.Ice:
                var dir = (player.transform.position - transform.position).normalized;
                Debug.Log(dir);

                if(Mathf.Abs(dir.z) > Mathf.Abs(dir.x))
                {
                    if (dir.z > 0)
                        Debug.Log("back");
                    else
                        Debug.Log("forward");
                }
                else
                {
                    if (dir.x > 0)
                        Debug.Log("left");
                    else
                        Debug.Log("right");
                }

                break;
            case ClayBlockType.Lime:
                break;
            case ClayBlockType.Oil:
                break;
            case ClayBlockType.Stone:
                break;
            case ClayBlockType.Water:
                player.TriggerSand();
                player.playerGravityY = 0.001f;
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
                var player = FindObjectOfType<Hun.Player.Player>();
                player.playerGravityY = 1f;
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
}