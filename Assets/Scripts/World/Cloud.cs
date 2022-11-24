using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField, Range(0.0f, 5.0f)] private float moveSpeed = 3f;
    [SerializeField] private float maxLeftPosX = -15f;
    [SerializeField] private float maxRightPosX = 15f;

    private void FixedUpdate()
    {
        if(transform.position.x <= maxLeftPosX)
            transform.position = new Vector3(maxRightPosX, transform.position.y, transform.position.z) ;

        transform.Translate(Vector3.right* moveSpeed * Time.deltaTime);
    }
}