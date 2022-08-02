using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private Transform curCheckPoint;

        [Header("== Movement Property ==")]
        [SerializeField, Range(0F, 10F)] private float movingSpeed = 2F;
        //[SerializeField, Range(0F, 10F)] private float jumpPower = 3F;
        [SerializeField, Range(0f, 5f)] private float dashSpeed = 1.5f;
        private float currentDashSpeed = 1f;
        [SerializeField, Range(0f, 10f)] private float ladderUpDownSpeed = 3f;
        [SerializeField, Range(0f, 10f)] private float JumpSpeed = 5f;
        [HideInInspector] public float playerGravityY = 1f;

        private bool isMove = true;
        private Vector3 movingInputValue;
        private Vector3 movingVector = Vector3.zero;

        [Header("== Mouthful Property ==")]
        //[HideInInspector] public ClayBlock targetEntity;
        [SerializeField] private Transform mouthfulRoot;
        [SerializeField] private float mouthfulDistance = 1f;
        [SerializeField] private float spitRadius = 1f;

        [SerializeField] private ClayBlock targetClayBlock;
        private List<ClayBlock> targetClayBlockList = new List<ClayBlock>();

        private RaycastHit hitBlock;
        private RaycastHit[] hits = new RaycastHit[10];
        private bool HasMouthfulObj => targetClayBlock != null;
        private const float minTimeBetMouthful = 1.0f;
        private float lastMouthfulTime;
        private bool IsMouthful
        {
            get
            {
                if (Time.time >= lastMouthfulTime + minTimeBetMouthful &&
                    !anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Mouthful"))
                    return true;

                return false;
            }
        }

        private bool isInteracting = false;
        private bool isCarryingObject = false;

        public bool IsLadderInside { get; private set; }
        public bool IsTrampilineInside { get; private set; }
        public bool IsCanonInside { get; private set; }

        private bool IsGrounded => characterController.isGrounded;

        [SerializeField] private Animator anim;

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
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

            lastMouthfulTime = Time.time;
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
        /// Ʈ���޸��� Ÿ�� �ִ���/���� ������ ���� ����
        /// </summary>
        /// <param name="value"> Ʈ���޸��� Ÿ�� �ִ��� ������</param>
        public void SetTrampilineState(bool value) => IsTrampilineInside = value;

        /// <summary>
        /// ������ Ÿ�� �ִ���/���� ������ ���� ����
        /// </summary>
        /// <param name="value"> Ʈ���޸��� Ÿ�� �ִ��� ������</param>
        public void SetCanonState(bool value) => IsCanonInside = value;

        /// <summary>
        /// ��Ż�� �̵��� �߻��ϴ� �޼���
        /// </summary>
        /// <param name="portal">��Ż ������Ʈ</param>
        private void OnWalkedThroughPortal(Hun.Obstacle.Portal portal)
        {
            interactivePortal = portal;
        }

        #region Mouthful-Spit
        /// <summary>
        /// �ӱݱ�/��� Ű(Space) �Է½� �߻��ϴ� �޼���
        /// </summary>
        private void OnMouthful()
        {
            if (!IsMouthful)
                return;

            if (targetClayBlock == null)
            {
                Mouthful();
                StartCoroutine(CheckMouthfulAnimState());
            }
            else
            {
                anim.SetTrigger("isMouthful");

                if (Physics.Raycast(mouthfulRoot.position,
                    mouthfulRoot.forward, out hitBlock, mouthfulDistance))
                {
                    // �տ� ���� Ÿ���� ClayBlock�� �ִٸ� ��ġ�⸦ �Ѵ�.
                    if (hitBlock.collider.TryGetComponent<ClayBlock>(out ClayBlock clayBlock))
                    {
                        if (targetClayBlock.ClayBlockType == clayBlock.ClayBlockType)
                        {
                            Debug.Log("��ġ�� ����");
                            clayBlock.OnFusion();
                            Destroy(targetClayBlock);
                            targetClayBlock = null;
                            return;
                        }
                    }
                    else // ���� ClayBlock�� �ƴ� �ٸ� ��ü�� �ִٸ�, ������ ��ġ���� �ʴ´�.
                    {
                        return;
                    }
                }

                // �ٽ� ���� �� �տ� �ɸ��� �͵� ����, �Ʒ��� ClayBlock�� ������ ���� �� �ִ�.
                var targetVec = transform.position + transform.forward * 1f;
                if (Physics.Raycast(targetVec, Vector3.down * 1.2f, out hitBlock,
                    mouthfulDistance, LayerMask.GetMask("ClayBlock")))
                {
                    Spit();
                }
            }
        }

        /// <summary>
        /// �տ� Ray�� ���� ClayBlock�� �ִٸ� �ӱݱ⸦ �Ѵ�.
        /// </summary>
        private void Mouthful()
        {
            RaycastHit hit;

            if (Physics.Raycast(mouthfulRoot.position,
                mouthfulRoot.forward, out hit, mouthfulDistance))
            {
                if (hit.collider.TryGetComponent<ClayBlock>(out targetClayBlock))
                {
                    if (targetClayBlock.IsMouthful)
                    {
                        targetClayBlock.OnMouthful();
                        targetClayBlock.transform.SetParent(transform);
                    }
                }
            }

            anim.SetTrigger("isMouthful");
        }

        /// <summary>
        /// ClayBlock�� ��ġ�� �ʴ� ��쿡 ����´�.
        /// </summary>
        private void Spit()
        {
            targetClayBlock.transform.SetParent(null);
            //var targetPos = transform.position + (Vector3.up * 0.5f + transform.forward * 1.5f);
            var targetPos = hitBlock.transform.position + Vector3.up * 1f;
            targetClayBlock.OnSpit(targetPos);
            targetClayBlock = null;
        }

        private IEnumerator CheckMouthfulAnimState()
        {
            WaitForSeconds delay = new WaitForSeconds(0.01f);

            isMove = false;
            anim.SetBool("isWalk", false);

            while (true)
            {
                yield return delay;

                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                    break;
            }

            isMove = true;
        }
        #endregion

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

        public void TeleportToCheckPoint()
        {
            TeleportPlayerTransform(curCheckPoint);
        }

        /// <summary>
        /// �ش� �������� ȸ���մϴ�.
        /// </summary>
        private void Look(Quaternion rotation) => transform.rotation = rotation;

        #region Movement (Move, Jump)

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
        /// �� ������ �̵��� ó���մϴ�
        /// </summary>
        private void UpdateMovement()
        {
            if (!isMove)
            {
                CalculateGravityOn(ref movingVector);
                return;
            }

            if (grappler.IsGrappling || IsTrampilineInside || IsCanonInside)
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

                    //var tmp = movingVector.y;
                    movingVector = cameraYAxisRotation * movingInputValue;
                    movingVector *= movingSpeed * currentDashSpeed;
                    //movingVector.y = tmp;

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
        }

        /// <summary>
        /// �̵� ���Ϳ� �߷��� ����մϴ�.
        /// </summary>
        /// <param name="movingVector">�̵� ����</param>
        private void CalculateGravityOn(ref Vector3 movingVector)
        {
            movingVector.y += Physics.gravity.y * Time.deltaTime * playerGravityY;
        }

        public void TriggerSand() => movingVector = new Vector3(0f, -0.1f, 0f);

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

        /// <summary>
        /// Ʈ���޸��� ����� �� ������ ��ġ�� �̵��մϴ�.
        /// </summary>
        /// <returns></returns>
        /// <param name="poses"> ��ġ �� </param>
        public void JumpToPosByTrampiline(Transform[] poses)
        {
            //anim.SetBool("isJump", true);
            anim.SetBool("isWalk", false);
            StartCoroutine(TrampilineJump(poses));
        }

        IEnumerator TrampilineJump(Transform[] poses)
        {
            Look(Quaternion.LookRotation(poses[3].forward));

            int index = 0;
            while (index < poses.Length)
            {
                transform.position = Vector3.MoveTowards
                    (transform.position, poses[index].transform.position, Time.deltaTime * JumpSpeed);

                if (transform.position == poses[index].transform.position)
                    index++;

                yield return new WaitForSeconds(0.001F);
            }

            IsTrampilineInside = false;
            //anim.SetBool("isJump", false);
        }

        /// <summary>
        /// ������ ����� �� ������ ��ġ�� �̵��մϴ�.
        /// </summary>
        /// <returns></returns>
        /// <param name="canonPos"> ���� ��ġ �� </param>
        /// <param name="destPos"> ���� ��ġ �� </param>
        public void FiredToPosByCanon(Transform canonPos, Vector3 destPos)
        {
            //anim.SetBool("isFired", true);
            anim.SetBool("isWalk", false); //Test

            StartCoroutine(CanonFired(canonPos, destPos));
        }

        IEnumerator CanonFired(Transform canonPos, Vector3 destPos)
        {
            Look(Quaternion.LookRotation(canonPos.transform.forward));

            Vector3 newPos = canonPos.transform.position;
            newPos.y = newPos.y - 0.5f;
            transform.position = newPos;

            yield return new WaitForSeconds(1F);

            while (transform.position != destPos)
            {
                transform.position = Vector3.MoveTowards
                    (transform.position, destPos, Time.deltaTime * movingSpeed);

                yield return new WaitForSeconds(0.001F);
            }

            IsCanonInside = false;
            //anim.SetBool("isFired", false);
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

            if (other.TryGetComponent(out ClayBlock clayBlock))
                clayBlock.OnEnter();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IObstacle obstacle))
                obstacle.OnExit();

            if (other.TryGetComponent(out ClayBlock clayBlock))
                clayBlock.OnExit();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out ClayBlock clayBlock))
                clayBlock.OnEnter();
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider.TryGetComponent(out ClayBlock clayBlock))
                clayBlock.OnExit();
        }

        public static event UnityAction<Player> PlayerSpawnedEvent;
        public event UnityAction PlayerDiedEvent;
    }
}