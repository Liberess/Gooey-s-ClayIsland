using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hun.Obstacle;

namespace Hun.Player
{
    public class PlayerInteract : MonoBehaviour
    {
        private PlayerController playerCtrl;

        private Portal interactivePortal = null;
        private CarriableStageObject interactiveCarriableObject = null;

        public bool IsInteracting { get; private set; }
        public bool IsCarryingObject { get; private set; }

        public bool IsCanonInside { get; private set; }
        public bool IsLadderInside { get; set; }
        public bool IsTrampilineInside { get; private set; }

        private Animator anim;

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            playerCtrl = GetComponent<PlayerController>();
        }

        private void Start()
        {
            IsInteracting = false;
            IsCarryingObject = false;

            IsCanonInside = false;
            IsLadderInside = false;
            IsTrampilineInside = false;

            StartCoroutine(FindInterativeCarriableStageObject());
        }

        /// <summary>
        /// 사다리에 타고 있는지/있지 않은지 상태 설정
        /// </summary>
        public void SetLadderState(bool value) => IsLadderInside = value;

        /// <summary>
        /// 트램펄린에 타고 있는지/있지 않은지 상태 설정
        /// </summary>
        public void SetTrampilineState(bool value) => IsTrampilineInside = value;

        /// <summary>
        /// 대포에 타고 있는지/있지 않은지 상태 설정
        /// </summary>
        public void SetCanonState(bool value) => IsCanonInside = value;

        /// <summary>
        /// 인터랙트 키(Enter) 입력시 발생하는 메서드
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
                    // TODO: 스테이지 이동 수정
                    print("TODO: 스테이지 이동 수정");

                    if (interactivePortal.PortalType == PortalType.Stage)
                        interactivePortal.ActiveStagePortal();
                    else
                        playerCtrl.TeleportPlayerTransform(interactivePortal.transform);
                }
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
                if (hit.collider.TryGetComponent(out Hun.Item.PieceStar pieceStar))
                    pieceStar.UseItem();

                if (hit.collider.TryGetComponent(out BreakableWall breakableWall))
                    breakableWall.InteractWall();
            }
        }

        /// <summary>
        /// 포탈로 이동시 발생하는 메서드
        /// </summary>
        public void OnWalkedThroughPortal(Portal portal) => interactivePortal = portal;

        /// <summary>
        /// 주변의 운반 가능한 오브젝트를 찾는다.
        /// </summary>
        IEnumerator FindInterativeCarriableStageObject()
        {
            while (true)
            {
                if (!IsInteracting)
                {
                    interactiveCarriableObject = null;
                    var colliders = Physics.OverlapCapsule(transform.position + (Vector3.up * 0.5F), transform.position + (Vector3.up * 1.5F), 1F);

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
        /// 트램펄린에 닿았을 때 지정한 위치로 이동합니다.
        /// </summary>
        /// <returns></returns>
        /// <param name="poses"> 위치 값 </param>
        public void JumpToPosByTrampiline(float force, Transform[] poses)
        {
            //anim.SetBool("isJump", true);
            anim.SetBool("isWalk", false);
            StartCoroutine(TrampilineJump(force, poses));
        }

        IEnumerator TrampilineJump(float force, Transform[] poses)
        {
            playerCtrl.PlayerMovement.Look(Quaternion.LookRotation(poses[3].forward));

            int index = 0;
            while (index < poses.Length)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    poses[index].transform.position, Time.deltaTime * force);

                if (transform.position == poses[index].transform.position)
                    index++;

                yield return new WaitForSeconds(0.001F);
            }

            IsTrampilineInside = false;
            //anim.SetBool("isJump", false);
        }
        #endregion

        #region Canon
        /// <summary>
        /// 대포에 닿았을 때 지정한 위치로 이동합니다.
        /// </summary>
        /// <returns></returns>
        /// <param name="canonPos"> 대포 위치 값 </param>
        /// <param name="destPos"> 목적 위치 값 </param>
        public void FiredToPosByCanon(Transform canonPos, Vector3 destPos)
        {
            //anim.SetBool("isFired", true);
            anim.SetBool("isWalk", false); //Test

            StartCoroutine(CanonFired(canonPos, destPos));
        }

        IEnumerator CanonFired(Transform canonPos, Vector3 destPos)
        {
            playerCtrl.PlayerMovement.Look(Quaternion.LookRotation(canonPos.transform.forward));

            Vector3 newPos = canonPos.transform.position;
            newPos.y = newPos.y - 0.5f;
            transform.position = newPos;

            yield return new WaitForSeconds(1F);

            while (transform.position != destPos)
            {
                transform.position = Vector3.MoveTowards
                    (transform.position, destPos, Time.deltaTime * playerCtrl.PlayerMovement.MoveSpeed);

                yield return new WaitForSeconds(0.001F);
            }

            IsCanonInside = false;
            //anim.SetBool("isFired", false);
        }
        #endregion
    }
}