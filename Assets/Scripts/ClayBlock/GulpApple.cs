using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GulpApple : ClayBlock
{
    [SerializeField] private Hun.Entity.Player.PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = 
            GameObject.FindGameObjectWithTag("Player").GetComponent<Hun.Entity.Player.PlayerHealth>();
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
