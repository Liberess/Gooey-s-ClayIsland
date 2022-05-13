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
            // Dash �̺�Ʈ �߰�
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
        /// ��ٸ��� Ÿ�� �ִ���/���� ������ ���� ����
        /// </summary>
        /// <param name="value"> ��ٸ��� Ÿ�� �ִ��� ������</param>
        public void SetLadderState(bool value) => IsLadderInside = value;

        /// <summary>
        /// ��Ż�� �̵��� �߻��ϴ� �޼���
        /// </summary>
        /// <param name="portal">��Ż ������Ʈ</param>
        private void OnWalkedThroughPortal(Hun.Obstacle.Portal portal)
        {
            interactivePortal = portal;
        }

        /// <summary>
        /// �̵� Ű �Է½� �߻��ϴ� �޼���
        /// </summary>
        /// <param name="inputValue">�Է� ��</param>
        private void OnMove(InputValue inputValue)
        {
            var value = inputValue.Get<Vector2>();
            movingInputValue = new Vector3(value.x, 0, value.y);
        }

        /// <summary>
        /// ���� Ű �Է½� �߻��ϴ� �޼���
        /// </summary>
        /// <param name="inputValue">�Է� ��</param>
/*        private void OnJump(InputValue inputValue)
        {
            Jump();
        }*/

        /// <summary>
        /// ���ͷ�Ʈ Ű(Enter) �Է½� �߻��ϴ� �޼���
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
                    // TODO: �������� �̵� ����
                    print("TODO: �������� �̵� ����");

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
        /// �ش� �������� ȸ���մϴ�.
        /// </summary>
        private void Look(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        #region Movement (Move, Jump)

        /// <summary>
        /// ����
        /// </summary>
/*        private void Jump()
        {
            if (IsGrounded) {
                movingVector.y = jumpPower;
            }
        }*/

        /// <summary>
        /// �� ������ �̵��� ó���մϴ�
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

            // ���߿� ���ְų�, ��ٸ��� Ÿ�� ���� �ʴٸ� �߷��� �����Ѵ�.
            if (!IsGrounded && !IsLadderInside)
                CalculateGravityOn(ref movingVector);

            // ĳ���� �̵� �Է°��� �ְ�, ��ٸ��� Ÿ�� ���� ������ (��� ������ �����̴� ����)
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
        /// �̵� ���Ϳ� �߷��� ����մϴ�.
        /// </summary>
        /// <param name="movingVector">�̵� ����</param>
        private void CalculateGravityOn(ref Vector3 movingVector)
        {
            movingVector.y += Physics.gravity.y * Time.deltaTime;
        }

        #endregion

        /// <summary>
        /// �뽬(���� ����Ʈ)Ű�� ������ �� �߻��ϴ� Dash �̺�Ʈ�̴�.
        /// </summary>
        /// <param name="context"> �ݹ� �̺�Ʈ </param>
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
        /// �ֺ��� ��� ������ ������Ʈ�� ã���ϴ�.
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
        ///  ���콺 ����Ű�� ������ �� �߻��ϴ� Mouse Input �̺�Ʈ�̴�.
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