using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class MovingFoothold : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                Debug.Log("Enter");
                collision.collider.transform.SetParent(gameObject.transform);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                Debug.Log("Exit");
                collision.collider.transform.SetParent(null);
            }
        }
    }
}