using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        private Transform PlayerBody => playerCtrl.PlayerMovement.PlayerBody.transform;

        private bool isBlockedForward;
        private bool isBlockedForwardBorder;

        public bool IsInteracting { get; private set; }
        public bool IsCarryingObject { get; private set; }

        //사다리를 타고 있는지?
        public bool IsLadderInside { get; set; }

        //얼음 위에서 미끄러지고 있는 상태인지?
        public bool IsSlipIce { get; private set; }
        public bool IsCanonInside { get; private set; }
        public bool IsTrampilineInside { get; private set; }

        private bool isCheckForward = false;

        private Vector3 originVec;
        private Vector3 startSlipVec;

        private RaycastHit forwardRayHit;
        private Collider[] forwardCols;
        private RaycastHit[] forwardBorderRayHits;

        int closestIndex = -1;
        int secondClosestIndex = -1;

        float closestDist = float.MaxValue;
        float secondClosestDist = float.MaxValue;

        private ClayBlockTile targetIceBlock;

        private Rigidbody rigid;

        private static readonly int IsSlide = Animator.StringToHash("isSlide");

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
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
            HandleSlipIce();
        }

        private void EnabledCheckForward() => isCheckForward = true;

        private void HandleSlipIce()
        {
            //현재 얼음에서 미끄러지는 중이라면
            if (IsSlipIce)
            {
                //만약 정면이 막혔고 정면을 감지할 수 있다면, 얼음에서 멈춘다.
                if (isBlockedForward && isCheckForward)
                {
                    if (IsNotInClayState())
                        playerCtrl.PlayerMovement.Anim.SetBool(IsSlide, false);
                    startSlipVec = Vector3.zero;
                    IsSlipIce = false;
                }
                else
                {
                    if (playerCtrl.PlayerMovement.IsRotateComplete)
                    {
                        //다음 발 밑이 얼음이 아니라면 멈추는 동작을 한다.
                        originVec = PlayerBody.position + (transform.up * 0.3f) + (PlayerBody.forward * 0.3f);

                        //발 밑에 오브젝트가 있는지 판단한다.
                        if (Physics.Raycast(originVec, (-transform.up * 0.5f), out var hit, 0.5f,
                                LayerMask.GetMask("ClayBlock")))
                        {
                            if (hit.collider.TryGetComponent(out ClayBlock clayBlock))
                            {
                                //다음 발 밑이 얼음이 아니라면 멈춰야 한다.
                                if (clayBlock.ClayBlockType != ClayBlockType.Ice)
                                {
                                    playerCtrl.PlayerMovement.Anim.SetBool(IsSlide, false);
                                    playerCtrl.PlayerMovement.SetOverIceState(false);
                                    IsSlipIce = false;
                                }
                            }
                        }
                    }
                }
            }
            //만약 미끄러지는 중이 아니거나 얼음 위가 아니라면 수행한다.
            else
            {
                if (!playerCtrl.PlayerMovement.IsMoveProgressing /*|| !playerCtrl.PlayerMovement.IsOverIce*/)
                {
                    //만약 앞이 막혀있는데 대각선으로 이동을 한다면
                    if (playerCtrl.PlayerMovement.IsDiagonalInput)
                    {
                        Vector3 targetPos = Vector3.zero;
                        Vector3 dir = Vector3.zero;

                        FindClosestIceBlockPosition(ref targetPos, ref dir);

                        //Debug.DrawRay(rayPos.position, (targetPos - rayPos.position).normalized, Color.magenta, 5f);
                        //Debug.Log($"targetPos = {targetPos}, dir = {dir}");
                        float distance = Vector3.Distance(transform.position, targetPos);
                        if (distance <= 0.75f)
                        {
                            //Debug.DrawRay(rayPos.position, dir, Color.blue, 5f);
                            if (targetPos != Vector3.zero && dir != Vector3.zero)
                            {
                                isCheckForward = false;
                                SlipFlow(targetIceBlock);
                            }
                        }
                    }
                    //만약 앞이 막혀있는데 카메라를 회전하여 대각선 방향으로 움직인다면
                    else if (playerCtrl.PlayerMovement.MovingInputValue != Vector3.zero)
                    {
                        Vector3 targetPos = Vector3.zero;
                        Vector3 dir = Vector3.zero;

                        FindClosestIceBlockPosition(ref targetPos, ref dir);
                        targetPos.y = transform.position.y;
                        //Debug.DrawRay(rayPos.position, (targetPos - rayPos.position).normalized, Color.red, 5f);
                        float distance = Vector3.Distance(transform.position, targetPos);
                        //Debug.Log($"cur : {transform.position}, target : {targetPos}, distance : {distance}, dir : {dir}");
                        if (distance <= 0.75f)
                        {
                            if (targetPos != Vector3.zero && dir != Vector3.zero)
                            {
                                playerCtrl.PlayerMovement.Anim.SetBool(IsSlide, true);
                                IsSlipIce = true;

                                isCheckForward = false;
                                Invoke(nameof(EnabledCheckForward), 0.5f);

                                //최종 얼음 블럭을 향해 이동을 해야 하기 때문에,
                                //현재 서있던 블럭에서 해당 얼음 블럭을 향한 방향 벡터를 구한다.
                                if (Physics.Raycast(transform.position, -transform.up,
                                        out var groundHit, 0.5f, LayerMask.GetMask("ClayBlock")))
                                {
                                    dir = (targetPos - groundHit.transform.position).normalized;
                                    dir.y = 0.0f;

                                    SaveStartSlidingPosition(groundHit.transform.position);
                                }

                                if (dir != Vector3.zero)
                                {
                                    //Debug.DrawRay(rayPos.position, dir, Color.blue, 5f);
                                    playerCtrl.PlayerMovement.SetMoveProgress(targetPos, dir);
                                }
                            }
                        }
                    }
                    //위의 둘 다 아니고 바닥에 얼음이 있다면 미끄러져야 한다.
                    //공중에서 얼음으로 떨어졌을 때의 상황이다.
                    else if (playerCtrl.PlayerMovement.IsInAir)
                    {
                        //발 밑에 오브젝트가 있는지 판단한다.
                        if (Physics.Raycast(transform.position, (-transform.up * 0.5f), out var hit, 0.26f,
                                LayerMask.GetMask("ClayBlock")))
                        {
                            if (hit.collider.TryGetComponent(out ClayBlock clayBlock))
                            {
                                if (clayBlock.ClayBlockType == ClayBlockType.Ice)
                                    SlipFlow(clayBlock, 0.2f);
                            }
                        }
                    }
                }
            }
        }

        private void SlipFlow(ClayBlock clayBlock, float delay = 0.0f)
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
                    Invoke(nameof(EnabledCheckForward), 1.0f);
                }

                Vector3 targetPos = clayBlock.transform.position;
                Vector3 dir = GetForwardDirection();

                if (playerCtrl.PlayerMovement.IsDiagonalInput)
                {
                    FindClosestIceBlockPosition(ref targetPos, ref dir);
                }
                else
                {
                    if (Physics.Raycast(transform.position, -transform.up,
                            out var groundHit, 0.5f, LayerMask.GetMask("ClayBlock")))
                    {
                        SaveStartSlidingPosition(groundHit.transform.position);
                    }
                }

                //Debug.DrawRay(rayPos.position, dir, Color.green, 5f);

                targetPos.y = transform.position.y;
                if (dir != Vector3.zero && !playerCtrl.PlayerMovement.IsMoveProgressing)
                    playerCtrl.PlayerMovement.SetMoveProgress(targetPos, dir);
            }
        }

        /// <summary>
        /// 대각선 입력을 받아서 얼음을 타는 동작을 할때
        /// 가장 가까운(올바른) 얼음 블럭을 찾아서 해당 위치로 targetPos, dir 세팅
        /// </summary>
        /// <param name="targetPos">Target 얼음 블럭의 위치</param>
        /// <param name="dir">Forward Vector값</param>
        private void FindClosestIceBlockPosition(ref Vector3 targetPos, ref Vector3 dir)
        {
            if (gameMgr.IceBlockList.Count > 0)
            {
                closestIndex = -1;
                secondClosestIndex = -1;

                closestDist = float.MaxValue;
                secondClosestDist = float.MaxValue;

                for (int i = 0; i < gameMgr.IceBlockList.Count; i++)
                {
                    Vector3 blockPos = gameMgr.IceBlockList[i].transform.position;
                    float curDist = Vector3.Distance(transform.position, blockPos);

                    if (Physics.Raycast(rayPos.position, (blockPos - transform.position).normalized,
                            out var hit, 1.5f, LayerMask.GetMask("ClayBlock")))
                    {
                        if (!hit.collider.TryGetComponent(out ClayBlockTile clayTile) ||
                            clayTile.ClayBlockType != ClayBlockType.Ice)
                            continue;

                        if (curDist < closestDist)
                        {
                            secondClosestIndex = closestIndex;
                            secondClosestDist = closestDist;

                            closestIndex = i;
                            closestDist = curDist;
                        }
                        else if (curDist < secondClosestDist)
                        {
                            secondClosestIndex = i;
                            secondClosestDist = curDist;
                        }
                    }
                }

                if (closestIndex >= 0)
                {
                    //만약 원래 목표와 가장 가까운 목표의 위치가 동일하다면
                    //얼음 위에서 가장 가까운 얼음을 찾은 것이기에 올바른 값이 아니다
                    //그러므로 두 번째로 가까운 목표를 찾아야 한다
                    if (playerCtrl.PlayerMovement.IsOverIce &&
                        targetPos == gameMgr.IceBlockList[closestIndex].transform.position)
                    {
                        if (secondClosestIndex >= 0)
                        {
                            targetPos = gameMgr.IceBlockList[secondClosestIndex].transform.position;
                            targetIceBlock = gameMgr.IceBlockList[secondClosestIndex];
                        }
                    }
                    else
                    {
                        targetPos = gameMgr.IceBlockList[closestIndex].transform.position;
                        targetIceBlock = gameMgr.IceBlockList[closestIndex];
                    }
                }

                targetPos.y = transform.position.y;

                //최종 얼음 블럭을 향해 이동을 해야 하기 때문에,
                //현재 서있던 블럭에서 해당 얼음 블럭을 향한 방향 벡터를 구한다.
                if (GetGroundPosition(out Vector3 groundPos))
                {
                    if (targetPos.x == groundPos.x && targetPos.z == groundPos.z)
                    {
                        targetPos = gameMgr.IceBlockList[secondClosestIndex].transform.position;
                        targetIceBlock = gameMgr.IceBlockList[secondClosestIndex];

                        //시작 지점이 없다는 것은 벽에 막혀서 멈췄다가
                        //BorderLine을 향해 미끄러졌다는 것이다.
                        //그러므로 새로운 startSlipVec을 현재 땅으로 잡는다.
                        if (startSlipVec == Vector3.zero)
                        {
                            startSlipVec = groundPos;
                            startSlipVec.y = transform.position.y;
                        }

                        dir = (targetPos - startSlipVec).normalized;
                    }
                    else
                    {
                        dir = (targetPos - groundPos).normalized;
                    }

                    dir.y = 0.0f;
                    SaveStartSlidingPosition(groundPos);
                }
            }
        }

        private bool GetGroundPosition(out Vector3 groundPos)
        {
            if (Physics.Raycast(transform.position, -transform.up,
                    out var groundHit, 0.5f, LayerMask.GetMask("ClayBlock")))
            {
                groundPos = groundHit.transform.position;
                return true;
            }

            groundPos = Vector3.zero;
            return false;
        }

        private void SaveStartSlidingPosition(Vector3 targetPos)
        {
            //만약 이동중이 아니었다면, 새로 이동을 시작하는 원점을 저장한다.
            if (!playerCtrl.PlayerMovement.IsMoveProgressing)
            {
                startSlipVec = targetPos;
                startSlipVec.x = Mathf.FloorToInt(startSlipVec.x);
                startSlipVec.z = Mathf.FloorToInt(startSlipVec.z);
            }
        }

        /// <summary>
        /// 캐릭터가 바라보는 방향을 기준으로 방향 벡터를 정규화한다.
        /// </summary>
        private Vector3 GetForwardDirection()
        {
            Vector3 forward = PlayerBody.forward;

            PlayerBody.rotation = Quaternion.LookRotation(forward.normalized);

            float currentX = forward.x;
            float currentZ = forward.z;

            float posX = (Mathf.Abs(currentX) >= 0.9f) ? forward.x : 0.0f;
            float posZ = (Mathf.Abs(currentZ) >= 0.9f) ? forward.z : 0.0f;

            Vector3 dir = new Vector3(posX, 0f, posZ);
            if (dir == Vector3.zero)
            {
                posX = Mathf.Abs(currentX) > Mathf.Abs(currentZ)
                    ? (int)Math.Round(forward.x, MidpointRounding.AwayFromZero)
                    : 0f;
                posZ = Mathf.Abs(currentX) <= Mathf.Abs(currentZ)
                    ? (int)Math.Round(forward.z, MidpointRounding.AwayFromZero)
                    : 0f;
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
                rayPos.position + (PlayerBody.forward * 0.3f), 0.3f, mask);
            isBlockedForward = forwardCols.Length > 0;

            if (IsSlipIce || playerCtrl.PlayerMovement.IsOverIce)
            {
                forwardBorderRayHits = Physics.RaycastAll(rayPos.position, PlayerBody.forward, 1.0f,
                    LayerMask.GetMask("BorderLine"));
                isBlockedForwardBorder = forwardBorderRayHits.Length > 0;
                if (isBlockedForwardBorder && isCheckForward)
                {
                    isCheckForward = false;

                    playerCtrl.PlayerMovement.CancelMoveProgress();

                    Vector3 dir = startSlipVec;
                    Vector3 targetPos = Vector3.zero;

                    playerCtrl.PlayerMovement.Anim.SetBool(IsSlide, true);
                    IsSlipIce = true;

                    //얼음이 아닌 블럭 위에서 미끄러졌는데, BorderLine에 닿은 경우
                    if (startSlipVec != Vector3.zero)
                    {
                        dir = (startSlipVec - transform.position).normalized;
                        dir.y = 0.0f;
                    }
                    //처음 시작 지점이 없는 상태에서 미끄러져서 되돌아갈 때
                    else
                    {
                        FindClosestIceBlockPosition(ref targetPos, ref dir);
                    }

                    Invoke(nameof(EnabledCheckForward), 1f);
                    //Debug.DrawRay(transform.position, dir, Color.cyan, 5f);
                    playerCtrl.PlayerMovement.SetMoveProgress(startSlipVec, dir, false);
                }
            }
        }

        private bool IsNotInClayState()
        {
            PlayerState state = playerCtrl.PlayerMovement.PlayerState;
            return state != PlayerState.mouthfulInClay && state != PlayerState.spitInClay;
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
            AudioManager.Instance.PlayOneShotSFX(ESFXName.Trampiline);

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
            AudioManager.Instance.PlayOneShotSFX(ESFXName.Canon);

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