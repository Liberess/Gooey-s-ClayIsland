using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hun.Player
{
    public class PlayerGrappler : MonoBehaviour
    {
        public enum GrapState { Jump, Pull, Destroy };
        private CharacterController characterController;

        [Header("== Grappling Action =="), Space(10)]
        [SerializeField, Range(0f, 2f)] private float pullVelocity = 0.05f;
        [SerializeField, Range(0f, 10f)] private float stopDistance = 3f;
        [SerializeField] private GameObject hookPrefab;
        [SerializeField] private Transform shootTransform;
        public GrapState CurrnetGrapState { get; private set; }
        private Hun.Obstacle.Hook hook;
        public bool IsGrappling { get; private set; }
        private float grapTime = 0.0f;
        private float grapDelayTime = 2.0f;

        private Vector3 movingInputValue;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            IsGrappling = false;
            grapTime = 0.0f;
            grapDelayTime = 2.0f;
        }

        private void Update()
        {
            GrapplingUpdate();
        }

        #region Grappling Action
        /// <summary>
        /// 마우스를 이용하여 갈고리 액션
        /// </summary>
        public void GrapplingUpdate()
        {
            switch (CurrnetGrapState)
            {
                default: break;
                case GrapState.Jump: JumpState(); break;
                case GrapState.Pull: PullState(); break;
                case GrapState.Destroy: DestroyState(); break;
            }
        }

        private void JumpState()
        {
            if (hook == null && grapTime >= grapDelayTime && Input.GetMouseButtonDown(0))
            {
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grapple")))
                {
                    if (hit.collider.CompareTag("GrapJump"))
                    {
                        float gap = hit.collider.gameObject.transform.position.y - transform.position.y;

                        if (Mathf.Abs(gap) <= 1.5f)
                        {
                            grapTime = 0.0f;
                            IsGrappling = false;

                            var lookVec = new Vector3(hit.transform.position.x,
                                transform.position.y, hit.transform.position.z);
                            transform.LookAt(lookVec);

                            hook = Instantiate(hookPrefab, shootTransform.position,
                                Quaternion.identity).GetComponent<Hun.Obstacle.Hook>();
                            hook.InitializeHook(this, hit.collider.transform.Find("HookPos"), shootTransform);

                            Invoke(nameof(DestroyHook), 8f);
                        }
                    }
                }
            }

            grapTime += Time.deltaTime;

            if (!IsGrappling || hook == null)
                return;

            if (Vector3.Distance(transform.position, hook.transform.position) <= stopDistance)
            {
                DestroyHook();
            }
            else
            {
                var lookVec = new Vector3(hook.transform.position.x,
                    transform.position.y, hook.transform.position.z);
                transform.LookAt(lookVec);

                var targetVec = hook.transform.position - transform.position;
                targetVec.y = 0f;

                characterController.Move(targetVec.normalized * pullVelocity);
            }
        }

        private void PullState()
        {
            if (grapTime >= grapDelayTime && Input.GetMouseButton(0))
            {
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grapple")))
                {
                    if (hit.collider.CompareTag("GrapPull") && 
                            hit.collider.TryGetComponent<Hun.Obstacle.GrapPull>(out var grapPullObj))
                    {
                        float gap = hit.collider.gameObject.transform.position.y - transform.position.y;

                        if (Mathf.Abs(gap) <= 1.5f)
                        {
                            if (grapPullObj.PullTimer <= 0.0f)
                            {
                                var lookVec = new Vector3(hit.transform.position.x,
                                    transform.position.y, hit.transform.position.z);
                                transform.LookAt(lookVec);

                                hook = Instantiate(hookPrefab, shootTransform.position,
                                    Quaternion.identity).GetComponent<Hun.Obstacle.Hook>();
                                hook.InitializeHook(this, grapPullObj.transform.Find("HookPos"), shootTransform);
                            }

                            if (grapPullObj.IsActive)
                            {
                                grapPullObj.Progress();
                            }
                            else
                            {
                                // 바라보는 방향 벡터
                                var direction = (hit.collider.transform.GetChild(0).position -
                                    transform.position).normalized;

                                if(movingInputValue != Vector3.zero && (direction - movingInputValue).z > 0)
                                {
                                    grapTime = 0.0f;
                                    IsGrappling = false;
                                    grapPullObj.Open();
                                    //grapPullObj.SetAnimTrigger("doOpen");
                                    DestroyHook();
                                }
                            }
                        }
                    }
                }
            }

            grapTime += Time.deltaTime;
        }

        private void DestroyState()
        {

        }

        /// <summary>
        /// 이동 키 입력시 발생하는 메서드
        /// </summary>
        /// <param name="inputValue">입력 값</param>
        private void OnMove(InputValue inputValue)
        {
            var value = inputValue.Get<Vector2>();
            movingInputValue = new Vector3(value.x, 0, value.y);
        }

        public void StartPull()
        {
            if(CurrnetGrapState == GrapState.Jump)
                IsGrappling = true;
        }

        private void DestroyHook()
        {
            if (hook == null)
                return;

            IsGrappling = false;
            Destroy(hook.gameObject);
            hook = null;
        }

        public void SetGrapState(GrapState grapState) => this.CurrnetGrapState = grapState;

        #endregion
    }
}