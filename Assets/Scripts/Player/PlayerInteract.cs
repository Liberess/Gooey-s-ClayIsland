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

        public bool IsInteracting { get; private set; }
        public bool IsCarryingObject { get; private set; }

        //사다리를 타고 있는지?
        public bool IsLadderInside { get; set; }

        //얼음 위에서 미끄러지고 있는 상태인지?
        [SerializeField] private bool m_IsSlipIce;
        public bool IsSlipIce { get => m_IsSlipIce; }
        
        public bool IsCanonInside { get; private set; }
        public bool IsTrampilineInside { get; private set; }

        //private RaycastHit[] rayHits;
        private Vector3 originVec;

        private Animator anim;

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            playerCtrl = GetComponent<PlayerController>();
        }

        private void Start()
        {
            m_IsSlipIce = false;
            
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
            originVec = transform.GetChild(0).position + (transform.up * 0.3f) + (transform.GetChild(0).forward * 0.6f);
            Debug.DrawRay(originVec, (-transform.up * 0.5f), Color.red);
            RaycastHit[] rayHits = Physics.RaycastAll(originVec, (-transform.up * 0.5f), 0.5f, LayerMask.GetMask("ClayBlock"));

            if (rayHits != null && rayHits.Length > 0)
            {
                for (int i = 0; i < rayHits.Length; i++)
                {
                    if (rayHits[i].collider.TryGetComponent(out ClayBlock clayBlock))
                    {
                        if (m_IsSlipIce && clayBlock.ClayBlockType != ClayBlockType.Ice)
                        {
                            m_IsSlipIce = false;
                            Debug.Log(rayHits[i].collider.name);
                            break;
                        }
                        
                        if (!m_IsSlipIce && clayBlock.ClayBlockType == ClayBlockType.Ice)
                        {
                            Debug.Log("ice");
                            m_IsSlipIce = true;
                            playerCtrl.PlayerMovement.AddMoveForce(transform.GetChild(0).forward.normalized);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Empty!");
                m_IsSlipIce = false;
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

        public void SetSlipIceState(bool value) => m_IsSlipIce = value;

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
                    // TODO: �������� �̵� ����
                    print("TODO: �������� �̵� ����");

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
        /// Ʈ���޸��� ����� �� ������ ��ġ�� �̵��մϴ�.
        /// </summary>
        /// <returns></returns>
        /// <param name="poses"> ��ġ �� </param>
        public void JumpToPosByTrampiline(float force, Transform[] poses, bool isSuccese)
        {
            //anim.SetBool("isJump", true);
            anim.SetBool("isWalk", false);
            StartCoroutine(TrampilineJump(force, poses, isSuccese));
        }

        IEnumerator TrampilineJump(float force, Transform[] poses, bool isSuccese)
        {
            playerCtrl.PlayerMovement.Look(Quaternion.LookRotation(poses[3].forward));
            gameObject.GetComponent<CapsuleCollider>().isTrigger = true;

            int index = 0;
            while (index < poses.Length)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    poses[index].transform.position, Time.deltaTime * force);

                if (transform.position == poses[index].transform.position)
                    index++;
                
                if(index == 2 && !isSuccese)
                {
                    Rigidbody rigid = gameObject.GetComponent<Rigidbody>();
                    rigid.AddForce((transform.forward + (-transform.up * 0.5f)) * -1f, ForceMode.Impulse);
                    gameObject.GetComponent<CapsuleCollider>().isTrigger = false;

                    yield return new WaitForSeconds(1F);

                    rigid.velocity = Vector3.zero;
                    break;
                }

                yield return new WaitForSeconds(0.001F);
            }

            IsTrampilineInside = false;
            gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
            //anim.SetBool("isJump", false);
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
            //anim.SetBool("isFired", true);
            anim.SetBool("isWalk", false); //Test

            StartCoroutine(CanonFired(canonPos, destPos));
        }

        IEnumerator CanonFired(Transform canonPos, Vector3 destPos)
        {
            playerCtrl.PlayerMovement.Look(Quaternion.LookRotation(canonPos.transform.forward));
            gameObject.GetComponent<CapsuleCollider>().isTrigger = true;

            Vector3 newPos = canonPos.position;
            newPos.y = newPos.y - 0.5f;
            transform.position = newPos;

            yield return new WaitForSeconds(1F);

            while (transform.position != destPos)
            {
                transform.position = Vector3.MoveTowards
                    (transform.position, destPos, Time.deltaTime * playerCtrl.PlayerMovement.MoveSpeedInCanon);

                yield return new WaitForSeconds(0.001F);
            }

            IsCanonInside = false;
            gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
            //anim.SetBool("isFired", false);
        }
        #endregion
    }
}