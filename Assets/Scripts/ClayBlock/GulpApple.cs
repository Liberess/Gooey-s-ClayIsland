using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hun.Player;

public class GulpApple : ClayBlock
{
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerController>().GetComponent<PlayerHealth>();
    }

    public override void OnEnter()
    {

    }

    public override void OnStay()
    {

    }

    public override void OnExit()
    {

    }

    public override void OnMouthful()
    {
        if (!IsMouthful)
            return;

        base.OnMouthful();
        playerHealth.RestoreHeart(1);

        Destroy(this.gameObject);
    }
}
