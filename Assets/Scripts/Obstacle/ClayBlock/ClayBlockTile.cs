using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayBlockTile : ClayBlock
{
    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStay()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
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