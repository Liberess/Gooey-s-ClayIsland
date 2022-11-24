using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private enum MoveDirection { Left, Right }
    [SerializeField] private MoveDirection moveDirc;
    private Vector3 moveVec;

    [SerializeField, Range(0.0f, 5.0f)] private float moveSpeed = 3f;
    [SerializeField] private float maxLeftPosX = -15f;
    [SerializeField] private float maxRightPosX = 15f;

    private void Start()
    {
        if (moveDirc == MoveDirection.Left)
            moveVec = Vector3.right;
        else
            moveVec = Vector3.left;
    }

    private void FixedUpdate()
    {
        if(moveDirc == MoveDirection.Left)
        {
            if (transform.position.x <= maxLeftPosX)
                transform.position = new Vector3(maxRightPosX, transform.position.y, transform.position.z);
        }
        else
        {
            if (transform.position.x >= maxRightPosX)
                transform.position = new Vector3(maxLeftPosX, transform.position.y, transform.position.z);
        }

        transform.Translate(moveVec * moveSpeed * Time.deltaTime);
    }
}