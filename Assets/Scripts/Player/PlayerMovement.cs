using System.Collections;
using System.Collections.Generic;
using Hun.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hun.Player
{
    public enum PlayerState
    {
        spit = 0,
        mouthful,
    }

    public class PlayerMovement : MonoBehaviour
    {
        private GameManager gameMgr;
        private PlayerController playerCtrl;

        [Header("== Movement Property ==")]
        [SerializeField] private GameObject playerBody;
        [SerializeField] private GameObject[] playerBodys = new GameObject[2];
        [SerializeField, Range(0F, 10F)] private float moveSpeed = 2f;
        [SerializeField, Range(0f, 5f)] private float dashSpeed = 1.5f;
        private float currentDashSpeed = 1f;
        [SerializeField, Range(0f, 10f)] private float ladderUpDownSpeed = 3f;
        [SerializeField, Range(0F, 10F)] private float moveSpeedInCanon = 2f;
        public float MoveSpeedInCanon { get => moveSpeedInCanon; }
        [HideInInspector] public float playerGravityY = 1f;
        public bool IsMove { get; private set; }
        public bool IsOverIce { get; private set; }

        [SerializeField] private float fallDamageValue = 3f;
        [SerializeField] private float maxPositionY;
        private bool isInAir = false;

        public Vector3 MovingInputValue { get; private set; }
        private Vector3 movingVector = Vector3.zero;

        public Vector3 PreviousPos { get; private set; }

        private Rigidbody rigid;
        private Animator anim;
        public Animator Anim { get => anim; }
        [SerializeField] private Animator[] anims = new Animator[2];

        private bool IsGrounded
        {
            get
            {
                if (!(Mathf.Abs(rigid.velocity.y) < 0f || Mathf.Abs(rigid.velocity.y) > 0f))
                {
                    return true;
                }
                else
                {
                    var colliders = Physics.OverlapSphere(transform.position, 0.3f,
                        LayerMask.GetMask("ClayBlock"));

                    if (colliders.Length > 0)
                        return true;
                }

                //Debug.Log("Fall Anim �߰�");
                anim.SetBool("isInAir",true);

                return false;
            }
        }
        public bool getIsGrounded { get => IsGrounded; }

        private bool isMoveForceCoroutineing = false;

        private void Awake()
        {
            anim = anims[(int)PlayerState.spit];
            rigid = GetComponent<Rigidbody>();
            playerCtrl = GetComponent<PlayerController>();
        }

        private void Start()
        {
            IsMove = true;
            currentDashSpeed = 1f;
            maxPositionY = Mathf.Round(transform.position.y);
            
            gameMgr = GameManager.Instance;

            if (playerBodys.Length == 0)
            {
                playerBodys[0] = transform.GetChild(0).gameObject;
                playerBodys[1] = transform.GetChild(1).gameObject;
            }

            if (playerBody == null)
                playerBody = playerBodys[(int)PlayerState.spit];

            SetupDashEvent();

            StartCoroutine(UpdatePreviousPosition());
            StartCoroutine(CheckStopState());
        }

        private void Update()
        {
            CountFallDamage();
            UpdateGravity();
        }

        private void FixedUpdate()
        {
            UpdateMovement();
        }

        public void ChangeModel(PlayerState playerState)
        {
            if(playerState == PlayerState.spit)
            {
                playerBodys[(int)PlayerState.spit].SetActive(true);
                playerBodys[(int)PlayerState.mouthful].SetActive(false);

                playerBody = playerBodys[(int)PlayerState.spit];
                anim = anims[(int)PlayerState.spit];
            }
            else
            {
                playerBodys[(int)PlayerState.mouthful].SetActive(true);
                playerBodys[(int)PlayerState.spit].SetActive(false);

                playerBody = playerBodys[(int)PlayerState.mouthful];
                anim = anims[(int)PlayerState.mouthful];
            }
        }

        #region Movement (Move, Look)
        public void AddMoveForce(Vector3 dir)
        {
            if (!isMoveForceCoroutineing)
                StartCoroutine(AddMoveForceCo(dir));
        }

        private IEnumerator UpdatePreviousPosition()
        {
            WaitForSeconds delay = new WaitForSeconds(0.2f);
            while(true)
            {
                yield return delay;
                PreviousPos = transform.position;
                yield return null;
            }
        }

        public IEnumerator AddMoveForceCo(Vector3 dir)
        {
            IsOverIce = true;
            isMoveForceCoroutineing = true;
            playerCtrl.PlayerInteract.SetSlipIceState(true);
            SetMovement(false);
            anim.SetBool("isWalk", false);

            while (true)
            {
                yield return null;

                //rigid.AddForce(dir * 0.1f, ForceMode.VelocityChange);
                rigid.velocity = dir * 5f;
                //transform.Translate(dir * 4f * Time.deltaTime);

                for (int i = 0; i < playerBodys.Length; i++)
                    playerBody.transform.rotation = Quaternion.LookRotation(dir);
                
                //���� ���� ���� �ʰų�, �̲������� ���°� �ƴ϶��
                if (/*!playerCtrl.PlayerInteract.IsIceInside && */!playerCtrl.PlayerInteract.IsSlipIce)
                    break;
                
                yield return new WaitForEndOfFrame();
            }

            SetMovement(true);
            rigid.velocity = rigid.angularVelocity = Vector3.zero;

            IsOverIce = false;
            isMoveForceCoroutineing = false;

            yield return null;
        }
        
        private IEnumerator CheckStopState()
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            
            while (gameMgr.IsGamePlay)
            {
                if (Vector3.Distance(PreviousPos, transform.position) <= 0.01f )
                {
                    Debug.Log("멈춤");
                    SetMovement(true);
                    playerCtrl.PlayerInteract.SetSlipIceState(false);
                }
  
                yield return delay;
            }
            
            yield return null;
        }

        public void SetMovement(bool value) => IsMove = value;

        /// <summary>
        /// �ش� �������� ȸ���մϴ�.
        /// </summary>
        public void Look(Quaternion rotation)
        {
            foreach(var playerBody in playerBodys)
            {
                playerBody.transform.rotation = rotation;
            }
        }

        /// <summary>
        /// �̵� Ű �Է½� �߻��ϴ� �޼���
        /// </summary>
        /// <param name="inputValue">�Է� ��</param>
        private void OnMove(InputValue inputValue)
        {
            var value = inputValue.Get<Vector2>();
            if (value != null)
                MovingInputValue = new Vector3(value.x, 0, value.y);
        }

        /// <summary>
        /// �� ������ �̵��� ó���մϴ�
        /// </summary>
        private void UpdateMovement()
        {
            if (!IsMove || isInAir)
                return;

            if (playerCtrl.PlayerInteract.IsTrampilineInside || playerCtrl.PlayerInteract.IsCanonInside)
                return;

            if (MovingInputValue != Vector3.zero) //������ �Է°��� �ִٸ�
            {
                anim.SetBool("isWalk", true);

                if (playerCtrl.PlayerInteract.IsLadderInside)
                {
                    var cameraYAxisRotation = Quaternion.Euler(0, playerCtrl.MainCamera.transform.eulerAngles.y, 0);

                    if (MovingInputValue.z > 0)
                    {
                        movingVector = Vector3.up * ladderUpDownSpeed;
                    }
                    else if (MovingInputValue.z < 0)
                    {
                        if (IsGrounded)
                            playerCtrl.PlayerInteract.IsLadderInside = false;

                        movingVector = Vector3.down * ladderUpDownSpeed;
                    }
                    else
                    {
                        movingVector = Vector3.zero;
                    }

                    Look(Quaternion.LookRotation(cameraYAxisRotation * MovingInputValue));

                    transform.Translate(movingVector * Time.deltaTime);
                    //characterController.Move(movingVector * Time.deltaTime);
                }
                else
                {
                    var cameraYAxisRotation = Quaternion.Euler(0, playerCtrl.MainCamera.transform.eulerAngles.y, 0);

                    //var tmp = movingVector.y;
                    movingVector = cameraYAxisRotation * MovingInputValue;
                    movingVector *= moveSpeed * currentDashSpeed;
                    //movingVector.y = tmp;

                    Look(Quaternion.LookRotation(cameraYAxisRotation * MovingInputValue));

                    transform.Translate(movingVector * Time.deltaTime);
                }
            }
            else //������ �Է°��� ���ٸ�
            {
                //movingVector = Vector3.zero;
                anim.SetBool("isWalk", false);
            }
        }

        private void CountFallDamage()
        {
            if (isInAir && IsGrounded)
            {
                if (maxPositionY - Mathf.Round(transform.position.y) >= fallDamageValue)
                {
                    Entity.DamageMessage dmgMsg = new Entity.DamageMessage();
                    dmgMsg.damager = gameObject;
                    dmgMsg.dmgAmount = 1;
                    dmgMsg.hitNormal = transform.position;
                    dmgMsg.hitPoint = transform.position;
                    gameObject.GetComponent<PlayerHealth>().ApplyDamage(dmgMsg);
                }

                maxPositionY = Mathf.Round(transform.position.y);
                anim.SetBool("isInAir", false);
                isInAir = false;
            }
            else if(!isInAir && !IsGrounded)
            {
                maxPositionY = Mathf.Round(transform.position.y);
                isInAir = true;
            }
        }

        private void UpdateGravity()
        {
            // ��ٸ� �Ǵ� Ʈ���޸�, ������ Ÿ�� ������ �߷��� �ۿ����� �ʴ´�.
            if (playerCtrl.PlayerInteract.IsLadderInside || playerCtrl.PlayerInteract.IsTrampilineInside
                || playerCtrl.PlayerInteract.IsCanonInside)
                rigid.useGravity = false;
            else
                rigid.useGravity = true;
        }

        public void InitializeMovingVector() => movingVector = new Vector3(0f, -0.1f, 0f);
        #endregion

        #region Dash
        /// <summary>
        /// �뽬(���� ����Ʈ)Ű�� ������ �� �߻��ϴ� Dash �̺�Ʈ�̴�.
        /// </summary>
        /// <param name="context"> �ݹ� �̺�Ʈ </param>
        private void Dash(InputAction.CallbackContext context)
        {
            if (!IsGrounded || playerCtrl.PlayerInteract.IsLadderInside)
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

        private void SetupDashEvent()
        {
            // Dash �̺�Ʈ �߰�
            InputActionMap playerActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
            InputAction dashAction = playerActionMap.FindAction("Dash");
            dashAction.performed += Dash;
            dashAction.canceled += Dash;
        }
        #endregion

        private void OnDestroy()
        {
            InputActionMap playerActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
            InputAction dashAction = playerActionMap.FindAction("Dash");
            dashAction.performed -= Dash;
            dashAction.canceled -= Dash;
        }
    }
}