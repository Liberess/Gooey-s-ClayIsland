using System.Collections;
using UnityEngine;

namespace Hun.Camera
{
    public class CameraFocus : MonoBehaviour
    {
        [SerializeField] private bool isSwap = false;
        [SerializeField] private Transform target;

        private void Start()
        {
            if (target == null)
                target = FindObjectOfType<Player.PlayerController>().transform;
        }

        private void Update()
        {
            if (target == null)
                return;

            if(!isSwap)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, 2f * Time.deltaTime);
                //transform.position = target.position;
            }
        }

        /// <summary>
        /// 회전값을 타겟의 회전값으로 리셋합니다.
        /// </summary>
        public void ResetRotation()
        {
            if (target == null)
                return;

            transform.rotation = target.rotation;
        }

        public IEnumerator SetTarget(Transform _target)
        {
            if (target != _target)
            {
                isSwap = true;

                while (true)
                {
                    var gap = Vector3.Distance(_target.position, transform.position);

                    if (Mathf.Abs(gap) <= 0.5f)
                        break;

                    transform.position = Vector3.Lerp(transform.position, _target.position, 3f * Time.deltaTime);

                    yield return null;
                }
            }

            isSwap = false;
            target = _target;
            yield return null;
        }
    }
}