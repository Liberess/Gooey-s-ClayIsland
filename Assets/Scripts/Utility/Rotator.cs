using UnityEngine;

namespace Hun.Utility
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private bool xRotation;
        [SerializeField] private bool yRotation;
        [SerializeField] private bool zRotation;

        [SerializeField, Range(0f, 120f)] private float rotationSpeed = 60;

        private float xRotSpeed = 0f;
        private float yRotSpeed = 0f;
        private float zRotSpeed = 0f;

        private void Start()
        {
            if (xRotation)
                xRotSpeed = rotationSpeed;
            else
                xRotSpeed = 0f;

            if (yRotation)
                yRotSpeed = rotationSpeed;
            else
                yRotSpeed = 0f;

            if (zRotation)
                zRotSpeed = rotationSpeed;
            else
                zRotSpeed = 0f;
        }

        private void Update()
        {
            transform.Rotate(xRotSpeed * Time.deltaTime,
                yRotSpeed * Time.deltaTime, zRotSpeed * Time.deltaTime);
        }
    }
}