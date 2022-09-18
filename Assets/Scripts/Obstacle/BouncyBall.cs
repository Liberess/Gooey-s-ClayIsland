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
    private const float minTimeBetPushed = 0.5f;
    [SerializeField] private float pushTime = 0f;
    [SerializeField] private bool bRunningMoveCo = false;

    private static readonly Vector3[] DefaultDirVectors =
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };
    private DirectionVector directionVector = new DirectionVector();

    private Animator anim;
    private Rigidbody rigid;
    private BoxCollider boxCol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        boxCol = GetComponent<BoxCollider>();

        directionVector.currentVectors[(int)DirectionType.Forward] = -transform.forward;
        directionVector.currentVectors[(int)DirectionType.Back] = transform.forward;
        directionVector.currentVectors[(int)DirectionType.Left] = transform.right;
        directionVector.currentVectors[(int)DirectionType.Right] = -transform.right;
    }

    private void Start()
    {
        canInteract = true;
        bRunningMoveCo = false;
        player = FindObjectOfType<Hun.Player.PlayerMovement>();

        SetupDirectionVectors();
    }

    private void Update()
    {
        if (canInteract && isAttachedPlayer && player.MovingInputValue != Vector3.zero)
        {
            pushTime += Time.deltaTime;

            if (pushTime >= minTimeBetPushed)
            {
                Debug.Log("ÀÌµ¿");
                pushTime = 0f;
                canInteract = false;

                if (!bRunningMoveCo)
                    StartCoroutine(MovementCo());

                if (!IsInvoking(nameof(SetInteractOn)))
                    Invoke(nameof(SetInteractOn), 0.5f);
            }
        }
    }

    private void SetupDirectionVectors()
    {
        Vector3[] newVecs =
            {
                transform.position + transform.forward,
                transform.position + -transform.forward,
                transform.position + -transform.right,
                transform.position + transform.right
            };

        directionVector.SetVectors(newVecs);
    }

    private void SetInteractOn() => canInteract = true;

    private Vector3 GetTranslatePosition()
    {
        SetupDirectionVectors();
        Vector3 dirVec = transform.position;

        for (int i = 0; i < directionVector.dirVectors.Length; i++)
        {
            Collider[] colliders = Physics.OverlapBox(directionVector.dirVectors[i],
                boxCol.size / 2, Quaternion.identity, targetLayer);

            if (colliders != null && colliders?.Length > 0)
            {
                dirVec = directionVector.currentVectors[i];
                break;
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dirVec, out hit, 1f))
        {
            if (hit.collider)
                return Vector3.zero;
        }

        return dirVec + transform.position;
    }

    private IEnumerator MovementCo()
    {
        bRunningMoveCo = true;

        Vector3 targetPos = GetTranslatePosition();

        if(targetPos != Vector3.zero)
        {
            while (true)
            {
                float distance = Vector3.Distance(transform.position, targetPos);
                if (distance <= 0.001f)
                    break;

                transform.position = Vector3.MoveTowards(transform.position,
                    targetPos, moveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            transform.position = targetPos;
        }

        bRunningMoveCo = false;
        yield return null;
    }

    public override void OnEnter()
    {
        pushTime = 0f;
        isAttachedPlayer = true;
    }

    public override void OnStay()
    {

    }

    public override void OnExit()
    {
        isAttachedPlayer = false;
    }
}