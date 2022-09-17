using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BouncyBall : ClayBlock
{
    private Hun.Player.PlayerMovement player;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField, Range(0f, 5f)] private float moveSpeed = 2f;
    private bool canInteract = true;
    private bool isAttachedPlayer = false;
    private bool IsPushing
    {
        get
        {
            if (isAttachedPlayer && (Time.time >= startPushedTime + minTimeBetPushed))
                return true;

            return false;
        }
    }
    private const float minTimeBetPushed = 2f;
    private float startPushedTime;

    private Vector3[] colliderVectors;

    private Animator anim;
    private Rigidbody rigid;
    private BoxCollider boxCol;

    private void OnDrawGizmosSelected()
    {
        Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue };

        Vector3[] newVecs =
            {
                transform.position + Vector3.forward,
                transform.position + Vector3.back,
                transform.position + Vector3.left,
                transform.position + Vector3.right
            };

        for (int i = 0; i < newVecs.Length; i++)
        {
            Gizmos.color = Color.Lerp(Color.white, colors[i], 0.5f);
            Gizmos.DrawSphere(newVecs[i], 0.5f);
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        boxCol = GetComponent<BoxCollider>();

        colliderVectors = new Vector3[]
            {
                transform.position + Vector3.forward,
                transform.position  + Vector3.back,
                transform.position  + Vector3.left,
                transform.position  + Vector3.right
            };
    }

    private void Start()
    {
        canInteract = true;
        player = FindObjectOfType<Hun.Player.PlayerMovement>();
    }

    private void Update()
    {
/*        if (!canInteract)
            return;*/

        if(IsPushing && player.MovingInputValue != Vector3.zero)
        {
            canInteract = false;
            Debug.Log("Input");
        }
    }

    public override void OnEnter()
    {
        isAttachedPlayer = true;
        startPushedTime = Time.time;

        Vector3 dirVec = Vector3.zero;

        for (int i = 0; i < colliderVectors.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(colliderVectors[i], Vector3.down, out hit, 1f))
            {
                if (hit.collider.TryGetComponent(out ClayBlockTile clayBlockTile))
                {
                    if (clayBlockTile.ClayBlockType == ClayBlockType.Ice)
                        continue;

                    Debug.Log("바닥에 " + clayBlockTile.name + "가 있음");

/*                    Collider[] colliders = Physics.OverlapBox(colliderVectors[i],
                        boxCol.size / 2, Quaternion.identity, targetLayer);

                    if (colliders != null && colliders.Length > 0)
                        dirVec = colliderVectors[i];*/
                }
            }
            else
            {
                continue;
            }
        }
    }

    public override void OnStay()
    {

    }

    public override void OnExit()
    {
        isAttachedPlayer = false;
    }
}