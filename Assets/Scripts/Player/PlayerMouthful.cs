using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Player
{
    public class PlayerMouthful : MonoBehaviour
    {
        private PlayerController playerCtrl;

        [Header("== Mouthful Property ==")]
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

        private Animator anim;

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            playerCtrl = GetComponent<PlayerController>();
        }

        private void Start()
        {
            lastMouthfulTime = Time.time;
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
                StartCoroutine(CheckMouthfulAnimState());

                if (Physics.Raycast(mouthfulRoot.position, mouthfulRoot.forward,
                    out hitBlock, mouthfulDistance, LayerMask.GetMask("ClayBlock")))
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
                var targetVec = mouthfulRoot.position + mouthfulRoot.forward * 1f;
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

            if (Physics.Raycast(mouthfulRoot.position, mouthfulRoot.forward,
                out hit, mouthfulDistance, LayerMask.GetMask("ClayBlock")))
            {
                if (hit.collider.TryGetComponent<ClayBlock>(out targetClayBlock))
                {
                    if (targetClayBlock.IsMouthful)
                    {
                        targetClayBlock.OnMouthful();
                        targetClayBlock.transform.SetParent(transform);
                    }

                    if (targetClayBlock.ClayBlockType == ClayBlockType.Apple)
                        targetClayBlock = null;
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

            playerCtrl.PlayerMovement.SetMovement(false);

            anim.SetBool("isWalk", false);

            while (true)
            {
                yield return delay;

                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
                    break;
            }

            playerCtrl.PlayerMovement.SetMovement(true);
        }
        #endregion
    }
}