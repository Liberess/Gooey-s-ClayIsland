using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Hun.Manager;
using UnityEngine;
using Hun.Obstacle;

namespace Hun.Player
{
    public class PlayerInteract : MonoBehaviour
    {
        private GameManager gameMgr;
        private PlayerController playerCtrl;

        private Portal interactivePortal = null;
        private CarriableStageObject interactiveCarriableObject = null;

        [SerializeField] private Transform rayPos;

        public bool isBlockedForward;
        public bool isBlockedForwardBorder;

        public bool IsInteracting { get; private set; }
        public bool IsCarryingObject { get; private set; }

        //사다리를 타고 있는지?
        public bool IsLadderInside { get; set; }

        //얼음 위에서 미끄러지고 있는 상태인지?
        public bool IsSlipIce { get; private set; }

        public bool IsCanonInside { get; private set; }
        public bool IsTrampilineInside { get; private set; }

        private Vector3 originVec;
        private Vector3 startSlipVec;

        private RaycastHit forwardRayHit;
        private Collider[] forwardCols;
        private RaycastHit[] forwardBorderRayHits;

        private static readonly int IsSlide = Animator.StringToHash("isSlide");

        private void Awake()
        {
            playerCtrl = GetComponent<PlayerController>();
        }

        private void Start()
        {
            IsSlipIce = false;

            IsInteracting = false;
            IsCarryingObject = false;

            IsCanonInside = false;
            IsLadderInside = false;
            IsTrampilineInside = false;

            gameMgr = GameManager.Instance;

            StartCoroutine(FindInterativeCarriableStageObject());
        }

        private void FixedUpdate()
        {
            CheckBlockedForward();
            CheckGround();
        }

        private void CheckGround()
        {
            if (isBlockedForward)
            {
                if (playerCtrl.PlayerMovement.PlayerState != PlayerState.mouthfulInClay &&
                    playerCtrl.PlayerMovement.PlayerState != PlayerState.spitInClay)
                    playerCtrl.PlayerMovement.Anim.SetBool(IsSlide, false);
                IsSlipIce = false;
                return;
            }

            originVec = transform.GetChild(0).position + (transform.up * 0.3f) + (transform.GetChild(0).forward * 0.3f);

#if UNITY_EDITOR
            Debug.DrawRay(originVec, (-transform.up * 0.5f), Color.red);
#endif

            //발 밑에 오브젝트가 있는지 판단한다.
            RaycastHit hit;
            if (Physics.Raycast(originVec, (-transform.up * 0.5f), out hit, 0.5f, LayerMask.GetMask("ClayBlock")))
            {
                if (hit.collider.TryGetComponent(out ClayBlock clayBlock))
                {
                    //현재 슬라이딩 중인데, 다음 발 밑이 얼음이 아니라면 멈춰야 한다.
                    if (IsSlipIce && clayBlock.ClayBlockType != ClayBlockType.Ice)
                    {
                        playerCtrl.PlayerMovement.Anim.SetBool(IsSlide, false);
                        IsSlipIce = false;
                    }
                    else
                    {
                        SlidingFlow(clayBlock);
                    }
                }
            }
            /*else
            {
                if (IsSlipIce)
                {
                    playerCtrl.PlayerMovement.Anim.SetBool(IsSlide, false);
                    IsSlipIce = false;
                }
            }*/
        }

        private void SlidingFlow(ClayBlock clayBlock)
        {
            if (clayBlock == null)
                return;

            //대포나 트램펄린을 타고 있지 않으며 슬라이딩 중이 아닌데,
            //다음 발 밑이 얼음이라면 슬라이딩을 진행한다.
            if (!IsCanonInside && !IsTrampilineInside && !IsSlipIce &&
                clayBlock.ClayBlockType == ClayBlockType.Ice)
            {
                if (!playerCtrl.PlayerMovement.IsMoveProgressing)
                {
                    playerCtrl.PlayerMovement.Anim.SetBool(IsSlide, true);
                    IsSlipIce = true;
                    
                    startSlipVec = transform.position;
                    startSlipVec.x = Mathf.FloorToInt(startSlipVec.x);
                    startSlipVec.z = Mathf.FloorToInt(startSlipVec.z);
                }
                
                Vector3 targetPos = clayBlock.transform.position;
                targetPos.y = transform.position.y;

                Vector3 dir = GetForwardDirection();
                if (dir != Vector3.zero && !playerCtrl.PlayerMovement.IsMoveProgressing)
                    playerCtrl.PlayerMovement.SetMoveProgress(targetPos, dir);
            }
        }

        /// <summary>
        /// 캐릭터가 바라보는 방향을 기준으로 방향 벡터를 정규화한다.
        /// </summary>
        private Vector3 GetForwardDirection()
        {
            Vector3 forward = playerCtrl.PlayerMovement.PlayerBody.transform.forward;

            playerCtrl.PlayerMovement.PlayerBody.transform.rotation =
                Quaternion.LookRotation(forward.normalized);
            
            float currentX = forward.x;
            float currentZ = forward.z;
            
            float posX = (Mathf.Abs(currentX) >= 0.9f) ? forward.x : 0.0f;
            float posZ = (Mathf.Abs(currentZ) >= 0.9f) ? forward.z : 0.0f;

            Vector3 dir = new Vector3(posX, 0f, posZ);
            if (dir == Vector3.zero)
            {
                posX = Mathf.Abs(currentX) > Mathf.Abs(currentZ) ? (int)Math.Round(forward.x, MidpointRounding.AwayFromZero) : 0f;
                posZ = Mathf.Abs(currentX) <= Mathf.Abs(currentZ) ? (int)Math.Round(forward.z, MidpointRounding.AwayFromZero) : 0f;
                dir = new Vector3(posX, 0f, posZ);
            }

            return dir;
        }

        /// <summary>
        /// 플레이어의 전방이 막혀있는지 체크한다.
        /// </summary>
        private void CheckBlockedForward()
        {
            LayerMask mask = LayerMask.GetMask("ClayBlock") | LayerMask.GetMask("TemperObject") |
                             LayerMask.GetMask("BorderLine");

            forwardCols = Physics.OverlapSphere(
                rayPos.position + (transform.GetChild(0).forward * 0.3f), 0.3f, mask);

            isBlockedForward = forwardCols.Length > 0;

            if (IsSlipIce)
            {
                forwardBorderRayHits = Physics.RaycastAll(rayPos.position, transform.GetChild(0).forward, 1.0f, LayerMask.GetMask("BorderLine"));
                isBlockedForwardBorder = forwardBorderRayHits.Length > 0;
                if (isBlockedForwardBorder)
                {
                    playerCtrl.PlayerMovement.CancelMoveProgress();

                    Vector3 dir = startSlipVec - transform.position;
                    float roundX = (int)Math.Round(dir.x, MidpointRounding.AwayFromZero);
                    float roundZ = (int)Math.Round(dir.z, MidpointRounding.AwayFromZero);
                    dir.x = Mathf.Clamp(roundX, -1.0f, 1.0f);
                    dir.z = Mathf.Clamp(roundZ, -1.0f, 1.0f);
                    dir.y = 0.0f;
                    
                    playerCtrl.PlayerMovement.SetMoveProgress(startSlipVec, dir, false);
                }
            }
        }

        /// <summary>
        /// ��ٸ��� Ÿ�� �ִ���/���� ������ ���� ����
        /// </summary>
        public void SetLadderState(bool value) => IsLadderInside = value;

        /// <summary>
        /// Ʈ���޸��� Ÿ�� �ִ���/���� ������ ���� ����
        /// </summary>
        public void SetTrampilineState(bool value) => IsTrampilineInside = value;

        /// <summary>
        /// ������ Ÿ�� �ִ���/���� ������ ���� ����
        /// </summary>
        public void SetCanonState(bool value) => IsCanonInside = value;

        public void SetSlipIceState(bool value) => IsSlipIce = value;

        /// <summary>
        /// ���ͷ�Ʈ Ű(Enter) �Է½� �߻��ϴ� �޼���
        /// </summary>
        private void OnInteract()
        {
            if (IsInteracting)
            {
                if (IsCarryingObject)
                {
                    interactiveCarriableObject.transform.parent = null;
                    IsInteracting = false;
                }
            }
            else
            {
                if (interactiveCarriableObject != null)
                {
                    interactiveCarriableObject.transform.parent = transform;
                    IsInteracting = true;
                    IsCarryingObject = true;
                }
                else if (interactivePortal != null)
                {
                    if (interactivePortal.PortalType == PortalType.Stage)
                        interactivePortal.ActiveStagePortal();
                    else
                        playerCtrl.TeleportPlayerTransform(interactivePortal.transform);
                }
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
                if (hit.collider.TryGetComponent(out Hun.Item.PieceStar pieceStar))
                    pieceStar.UseItem();

                if (hit.collider.TryGetComponent(out BreakableWall breakableWall))
                    breakableWall.InteractWall();
            }
        }

        /// <summary>
        /// ��Ż�� �̵��� �߻��ϴ� �޼���
        /// </summary>
        public void OnWalkedThroughPortal(Portal portal) => interactivePortal = portal;

        /// <summary>
        /// �ֺ��� ��� ������ ������Ʈ�� ã�´�.
        /// </summary>
        private IEnumerator FindInterativeCarriableStageObject()
        {
            while (true)
            {
                if (!IsInteracting)
                {
                    interactiveCarriableObject = null;
                    var colliders = Physics.OverlapCapsule(transform.position + (Vector3.up * 0.5F),
                        transform.position + (Vector3.up * 1.5F), 1F);

                    foreach (var c in colliders)
                    {
                        if (c.TryGetComponent<CarriableStageObject>(out var o))
                        {
                            interactiveCarriableObject = o;
                        }
                    }
                }

                yield return new WaitForSeconds(0.1F);
            }
        }

        #region Trampiline

        /// <summary>
        /// Ʈ���޸��� ����� �� ������ ��ġ�� �̵��մϴ�.
        /// </summary>
        /// <returns></returns>
        /// <param name="poses"> ��ġ �� </param>
        public void JumpToPosByTrampiline(float force, Transform[] poses, bool isSuccese)
        {
            if (playerCtrl.PlayerMouthful.TargetClayBlock == null)
                playerCtrl.PlayerMovement.ChangeModel(PlayerState.spitInClay);
            else
                playerCtrl.PlayerMovement.ChangeModel(PlayerState.mouthfulInClay);

            StartCoroutine(TrampilineJump(force, poses, isSuccese));
        }

        private IEnumerator TrampilineJump(float force, Transform[] poses, bool isSuccese)
        {
            playerCtrl.PlayerMovement.Look(Quaternion.LookRotation(poses[3].forward));
            gameObject.GetComponent<CapsuleCollider>().isTrigger = true;

            transform.position = poses[0].transform.position;
            playerCtrl.PlayerMovement.Anim.SetBool("isTrampiline", true);

            yield return new WaitForSeconds(0.5F);

            int index = 1;
            while (index < poses.Length)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    poses[index].transform.position, Time.deltaTime * force);

                if (transform.position == poses[index].transform.position)
                    index++;

                if (index == 3 && !isSuccese)
                {
                    Rigidbody rigid = gameObject.GetComponent<Rigidbody>();
                    rigid.AddForce(
                        (playerCtrl.PlayerMovement.PlayerBody.transform.forward + (-transform.up * 0.5f)) * -1f,
                        ForceMode.Impulse);
                    gameObject.GetComponent<CapsuleCollider>().isTrigger = false;

                    yield return new WaitForSeconds(1F);

                    rigid.velocity = Vector3.zero;
                    break;
                }

                yield return new WaitForSeconds(0.001F);
            }

            IsTrampilineInside = false;
            gameObject.GetComponent<CapsuleCollider>().isTrigger = false;

            playerCtrl.PlayerMovement.Anim.SetBool("isTrampiline", false);
            if (playerCtrl.PlayerMouthful.TargetClayBlock == null)
                playerCtrl.PlayerMovement.ChangeModel(PlayerState.spit);
            else
                playerCtrl.PlayerMovement.ChangeModel(PlayerState.mouthful);
        }

        #endregion

        #region Canon

        /// <summary>
        /// ������ ����� �� ������ ��ġ�� �̵��մϴ�.
        /// </summary>
        /// <returns></returns>
        /// <param name="canonPos"> ���� ��ġ �� </param>
        /// <param name="destPos"> ���� ��ġ �� </param>
        public void FiredToPosByCanon(Transform canonPos, Vector3 destPos)
        {
            if (playerCtrl.PlayerMouthful.TargetClayBlock == null)
                playerCtrl.PlayerMovement.ChangeModel(PlayerState.spitInClay);
            else
                playerCtrl.PlayerMovement.ChangeModel(PlayerState.mouthfulInClay);

            StartCoroutine(CanonFired(canonPos, destPos));
        }

        IEnumerator CanonFired(Transform canonPos, Vector3 destPos)
        {
            playerCtrl.PlayerMovement.Look(Quaternion.LookRotation(canonPos.transform.forward));
            gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;

            Vector3 newPos = canonPos.position;
            newPos.y = newPos.y - 0.5f;
            transform.position = newPos;
            playerCtrl.PlayerMovement.Anim.SetBool("isCanon", true);

            yield return new WaitForSeconds(1.5F);

            while (transform.position != destPos)
            {
                transform.position = Vector3.MoveTowards
                    (transform.position, destPos, Time.deltaTime * playerCtrl.PlayerMovement.MoveSpeedInCanon);

                yield return new WaitForSeconds(0.001F);
            }

            IsCanonInside = false;
            gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;

            playerCtrl.PlayerMovement.Anim.SetBool("isCanon", false);
            if (playerCtrl.PlayerMouthful.TargetClayBlock == null)
                playerCtrl.PlayerMovement.ChangeModel(PlayerState.spit);
            else
                playerCtrl.PlayerMovement.ChangeModel(PlayerState.mouthful);
        }

        #endregion
    }
}