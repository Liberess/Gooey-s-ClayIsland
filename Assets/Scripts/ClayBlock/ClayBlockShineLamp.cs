using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayBlockShineLamp : ClayBlock
{
    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void OnStay()
    {

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

        var colliders = Physics.OverlapSphere(transform.position, 1f,
            LayerMask.GetMask("ShineTower"));

        foreach(var tower in colliders)
        {
            gameObject.transform.position += new Vector3(0, 0.5f, 0);
            Destroy(tower);
            Destroy(this, 0.5f);
            Hun.Manager.GameManager.Instance.StageClear();
            return;
        }
    }
}