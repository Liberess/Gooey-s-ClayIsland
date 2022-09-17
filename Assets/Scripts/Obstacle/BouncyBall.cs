using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBall : ClayBlock
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField, Range(0f, 5f)] private float moveSpeed = 2f;
    private DirectionVector directionVector = new DirectionVector();

    private Animator anim;
    private Rigidbody rigid;
    private SphereCollider sphereCol;

    private void OnDrawGizmosSelected()
    {
        if (ClayBlockType != ClayBlockType.Ice)
            return;

        Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue };

        Vector3 defaultVec = sphereCol.center + transform.position;

        Vector3[] newVecs =
            {
                defaultVec + Vector3.forward,
                defaultVec + Vector3.back,
                defaultVec + Vector3.left,
                defaultVec + Vector3.right
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
        sphereCol = GetComponent<SphereCollider>();

        var defaultVec = sphereCol.center + transform.position;

        Vector3[] newVecs =
            {
                defaultVec + Vector3.forward,
                defaultVec + Vector3.back,
                defaultVec + Vector3.left,
                defaultVec + Vector3.right
            };

        directionVector.SetVectors(newVecs);
        directionVector.currentVectors[(int)DirectionType.Forward] = Vector3.back;
        directionVector.currentVectors[(int)DirectionType.Back] = Vector3.forward;
        directionVector.currentVectors[(int)DirectionType.Left] = Vector3.right;
        directionVector.currentVectors[(int)DirectionType.Right] = Vector3.left;
    }

    public override void OnEnter()
    {
        Vector3 dirVec = Vector3.zero;

        for (int i = 0; i < directionVector.dirVectors.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionVector.defaultVectors[i], out hit, 1f))
            {
                if (hit.collider.TryGetComponent(out ClayBlockTile clayBlockTile))
                {
                    if (clayBlockTile.ClayBlockType == ClayBlockType.Ice)
                        continue;
                }
            }
            else
            {
                continue;
            }

            Collider[] colliders = Physics.OverlapBox(directionVector.dirVectors[i],
                sphereCol.bounds.size / 2, Quaternion.identity, targetLayer);

            if (colliders != null && colliders.Length > 0)
                dirVec = directionVector.currentVectors[i];
        }

        if (dirVec != Vector3.zero)
            rigid.AddForce(dirVec, ForceMode.Impulse);
    }

    public override void OnStay()
    {

    }

    public override void OnExit()
    {

    }
}