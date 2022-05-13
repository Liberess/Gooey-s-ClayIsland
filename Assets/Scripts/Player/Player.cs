using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Hun.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Hun.Camera.MainCamera mainCamera;
        private CharacterController characterController;
        private Grappler grappler;

        private Hun.Obstacle.Portal interactivePortal = null;
        private Hun.Obstacle.CarriableStageObject interactiveCarriableObject = null;

        [Header("== Movement Property ==")]
        [SerializeField, Range(0F, 10F)] private float movingSpeed = 2F;
        //[SerializeField, Range(0F, 10F)] private float jumpPower = 3F;
        [SerializeField, Range(0f, 5f)] private float dashSpeed = 1.5f;
        private float currentDashSpeed = 1f;
        [SerializeField, Range(0f, 10f)] private float ladderUpDownSpeed = 3f;

        private Vector3 movingInputValue;
        private Vector3 movingVector = Vector3.zero;

        private bool isInteracting = false;
        private bool isCarryingObject = false;

        public bool IsLadderInside { get; private set; }

        private bool IsGrounded => characterController.isGrounded;

        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            grappler = GetComponent<Hun.Player.Grappler>();
        }

        private void Start()
        {
            PlayerSpawnedEvent?.Invoke(this);

            IsLadderInside = false;
            currentDashSpeed = 1f;

            if (mainCamera == null)
                mainCamera = FindObjectOfType<Hun.Camera.MainCamera>();

            SetupDashEvent();

            StartCoroutine(FindInterativeCarriableStageObject());
        }

        private void Update()
        {
            UpdateMovement();
        }

        private void SetupDashEvent()
        {
            // Dash 이벤트 추가
            InputActionMap playerActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
            InputAction dashAction = playerActionMap.FindAction("Dash");
            dashAction.performed += Dash;
            dashAction.canceled += Dash;
        }

        private void OnDestroy()
        {
            InputActionMap playerActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
            InputAction dashAction = playerActionMap.FindAction("Dash");
            dashAction.performed -= Dash;
            dashAction.canceled -= Dash;
        }

        /// <summary>
        /// 사다리에 타고 있는지/있지 않은지 상태 설정
        /// </summary>
        /// <param name="value"> 사다리에 타고 있는지 없는지</param>
        public void SetLadderState(bool value) => IsLadderInside = value;

        /// <summary>
        /// 포탈로 이동시 발생하는 메서드
        /// </summary>
        /// <param name="portal">포탈 오브젝트</param>
        private void OnWalkedThroughPortal(Hun.Obstacle.Portal portal)
        {
            interactivePortal = portal;
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

        /// <summary>
        /// 점프 키 입력시 발생하는 메서드
        /// </summary>
        /// <param name="inputValue">입력 값</param>
/*        private void OnJump(InputValue inputValue)
        {
            Jump();
        }*/

        /// <summary>
        /// 인터랙트 키(Enter) 입력시 발생하는 메서드
        /// </summary>
        private void OnInteract()
        {
            if (isInteracting)
            {
                if (isCarryingObject)
                {
                    interactiveCarriableObject.transform.parent = null;
                    isInteracting = false;
                }
            }
            else
            {
                if (interactiveCarriableObject != null)
                {
                    interactiveCarriableObject.transform.parent = transform;
                    isInteracting = true;
                    isCarryingObject = true;
                }
                else if (interactivePortal != null)
                {
                    // TODO: 스테이지 이동 수정
                    print("TODO: 스테이지 이동 수정");

                    if (interactivePortal.PortalType == Obstacle.PortalType.Stage)
                        interactivePortal.ActiveStagePortal();
                    else
                        TeleportPlayerTransform(interactivePortal.transform);
                }
            }
        }

        public void TeleportPlayerTransform(Transform targetPos)
        {
            characterController.enabled = false;
            transform.position = targetPos.position;
            characterController.enabled = true;
        }

        /// <summary>
        /// 해당 방향으로 회전합니다.
        /// </summary>
        private void Look(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        #region Movement (Move, Jump)

        /// <summary>
        /// 점프
        /// </summary>
/*        private void Jump()
        {
            if (IsGrounded) {
                movingVector.y = jumpPower;
            }
        }*/

        /// <summary>
        /// 매 프레임 이동을 처리합니다
        /// </summary>
        private void UpdateMovement()
        {
            if (grappler.IsGrappling)
                return;

            if (movingInputValue != Vector3.zero)
            {
                if (IsLadderInside)
                {
                    var cameraYAxisRotation = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);

                    if (movingInputValue.z > 0)
                    {
                        movingVector = Vector3.up * ladderUpDownSpeed;
                    }
                    else if (movingInputValue.z < 0)
                    {
                        if (IsGrounded)
                            IsLadderInside = false;

                        movingVector = Vector3.down * ladderUpDownSpeed;
                    }
                    else
                    {
                        movingVector = Vector3.zero;
                    }

                    Look(Quaternion.LookRotation(cameraYAxisRotation * movingInputValue));

                    characterController.Move(movingVector * Time.deltaTime);
                }
                else
                {
                    var cameraYAxisRotation = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);

                    var tmp = movingVector.y;
                    movingVector = cameraYAxisRotation * movingInputValue;
                    movingVector *= movingSpeed * currentDashSpeed;
                    movingVector.y = tmp;

                    Look(Quaternion.LookRotation(cameraYAxisRotation * movingInputValue));
                }
            }
            else
            {
                movingVector.x = movingVector.z = 0;
                anim.SetBool("isWalk", false);
            }

            // 공중에 떠있거나, 사다리에 타고 있지 않다면 중력을 적용한다.
            if (!IsGrounded && !IsLadderInside)
                CalculateGravityOn(ref movingVector);

            // 캐릭터 이동 입력값이 있고, 사다리에 타고 있지 않으면 (평소 땅에서 움직이는 상태)
            if (movingVector != Vector3.zero && !IsLadderInside)
            {
                characterController.Move(movingVector * Time.deltaTime);

                if (movingInputValue != Vector3.zero)
                    anim.SetBool("isWalk", true);
                else
                    anim.SetBool("isWalk", false);
            }
            /*            if (movingInputValue != Vector3.zero)
                        {
                            if (movingVector != Vector3.zero && !IsLadderInside)
                            {
                                characterController.Move(movingVector * Time.deltaTime);
                                anim.SetBool("isWalk", true);
                            }
                        }
                        else
                        {
                            anim.SetBool("isWalk", false);
                        }*/
        }

        /// <summary>
        /// 이동 벡터에 중력을 계산합니다.
        /// </summary>
        /// <param name="movingVector">이동 벡터</param>
        private void CalculateGravityOn(ref Vector3 movingVector)
        {
            movingVector.y += Physics.gravity.y * Time.deltaTime;
        }

        #endregion

        /// <summary>
        /// 대쉬(왼쪽 시프트)키를 눌렀을 때 발생하는 Dash 이벤트이다.
        /// </summary>
        /// <param name="context"> 콜백 이벤트 </param>
        private void Dash(InputAction.CallbackContext context)
        {
            if (!IsGrounded || IsLadderInside)
                return;

            if (context.action.phase == InputActionPhase.Performed)
            {
                currentDashSpeed = dashSpeed;
                anim.SetFloat("walkSpeed", 1.5f);
            }
            else if (context.action.phase == InputActionPhase.Canceled)
            {
                currentDashSpeed = 1f;
                anim.SetFloat("walkSpeed", 1);
            }
        }

        /// <summary>
        /// 주변의 운반 가능한 오브젝트를 찾습니다.
        /// </summary>
        /// <returns></returns>
        IEnumerator FindInterativeCarriableStageObject()
        {
            while (true)
            {
                if (!isInteracting)
                {
                    interactiveCarriableObject = null;
                    var colliders = Physics.OverlapCapsule(transform.position + (Vector3.up * 0.5F), transform.position + (Vector3.up * 1.5F), 1F);

                    foreach (var c in colliders)
                    {
                        if (c.TryGetComponent<Hun.Obstacle.CarriableStageObject>(out var o))
                        {
                            interactiveCarriableObject = o;
                        }
                    }
                }
                yield return new WaitForSeconds(0.1F);
            }
        }

        /// <summary>
        ///  마우스 왼쪽키를 눌렀을 때 발생하는 Mouse Input 이벤트이다.
        /// </summary>
        private void OnMouseInteract()
        {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Interactable")))
            {
                if (hit.collider.TryGetComponent<Hun.Item.PieceStar>(
                        out Hun.Item.PieceStar pieceStar))
                {
                    pieceStar.UseItem();
                }

                if (hit.collider.TryGetComponent<Hun.Obstacle.BreakableWall>(
                        out Hun.Obstacle.BreakableWall breakableWall))
                {
                    breakableWall.InteractWall();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Hun.Obstacle.Portal portal))
            {
                if (portal.EftType == Hun.Obstacle.Portal.EffectType.Enter)
                    TeleportPlayerTransform(portal.TargetPos);
                else
                    OnWalkedThroughPortal(portal);
            }

            if (other.TryGetComponent(out IObstacle obstacle))
                obstacle.OnEnter();

            if (other.TryGetComponent(out Hun.Item.IItem item))
                item.OnEnter();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IObstacle obstacle))
                obstacle.OnExit();
        }

        public static event UnityAction<Player> PlayerSpawnedEvent;
        public event UnityAction PlayerDiedEvent;
    }
}