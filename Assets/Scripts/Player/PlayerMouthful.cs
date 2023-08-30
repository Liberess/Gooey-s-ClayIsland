using Hun.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Player
{
    public class PlayerMouthful : MonoBehaviour
    {
        private PlayerController playerCtrl;
        private PlayerInteract playerInteract;
        private PlayerMovement playerMovement;

        [Header("== Mouthful Property ==")]
        [SerializeField] private Transform mouthfulRoot;
        public Transform MouthfulRoot { get => mouthfulRoot; }
        [SerializeField] private float mouthfulDistance = 1f;
        [SerializeField] private float spitRadius = 1f;

        [SerializeField] private ClayBlock targetClayBlock;
        public ClayBlock TargetClayBlock { get => targetClayBlock; }
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
                if (!playerCtrl.PlayerInteract.IsSlipIce && Time.time >= lastMouthfulTime + minTimeBetMouthful &&
                    !playerMovement.Anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Mouthful"))
                    return true;

                return false;
            }
        }

        private Animator anim;
        private static readonly int IsMouthful1 = Animator.StringToHash("isMouthful");

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            playerCtrl = GetComponent<PlayerController>();
            playerInteract = GetComponent<PlayerInteract>();
            playerMovement = GetComponent<PlayerMovement>();
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
            if (DataManager.Instance.GameData.gameState == GameState.Lobby)
                return;

            if (GameManager.Instance.IsClear || GameManager.Instance.IsFailed || GameManager.Instance.IsGameOver)
                return;

            if (!IsMouthful)
                return;

            if (!playerMovement.getIsGrounded)
                return;

            if (playerInteract.IsCanonInside || playerInteract.IsTrampilineInside)
                return;

            lastMouthfulTime = Time.time;

            if (!targetClayBlock)
            {
                Mouthful();
                StartCoroutine(CheckMouthfulAnimState());
                AudioManager.Instance.PlayOneShotSFX(ESFXName.Mouthful);
            }
            else //Fusion or Spit or Division
            {
                //Stone Block에 뱉기
                /*playerMovement.ChangeModel(PlayerState.spit);
                playerMovement.Anim.SetTrigger("isMouthful");
                StartCoroutine(CheckMouthfulAnimState());*/

                if(targetClayBlock)
                {
                    //�տ� ClayBlock�� �ִٸ� Fusion
                    if (Physics.Raycast(mouthfulRoot.position, mouthfulRoot.forward,
                        out hitBlock, mouthfulDistance, LayerMask.GetMask("ClayBlock")))
                    {
                        if (hitBlock.collider.TryGetComponent(out ClayBlockTile clayBlock))
                        {
                            if (clayBlock.IsSuccessFusion(targetClayBlock.
                                    GetComponent<ClayBlockTile>(), clayBlock))
                            {
                                playerMovement.ChangeModel(PlayerState.spit);
                                playerMovement.Anim.SetTrigger(IsMouthful1);
                                AudioManager.Instance.PlayOneShotSFX(ESFXName.Spit);

                                targetClayBlock = null;
                                UIManager.Instance.SetMouthfulUI();
                            }

                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                //�տ� TemperObject�� �ִٸ� Division
                if (Physics.Raycast(mouthfulRoot.position, mouthfulRoot.forward,
                    out hitBlock, mouthfulDistance, LayerMask.GetMask("TemperObject")))
                {
                    if (hitBlock.collider.TryGetComponent(out ClayBlock clayBlock))
                    {
                        playerMovement.ChangeModel(PlayerState.mouthful);
                        playerMovement.Anim.SetTrigger(IsMouthful1);
                        AudioManager.Instance.PlayOneShotSFX(ESFXName.Mouthful);

                        clayBlock.OnDivision();
                        UIManager.Instance.SetMouthfulUI(ClayBlockType.Toolbox);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                // �ٽ� ���� �� �տ� �ɸ��� �͵� ����, �Ʒ��� ClayBlock�� ������ ���� �� �ִ�.
                var targetVec = mouthfulRoot.position + mouthfulRoot.forward * 1f;
                if (Physics.Raycast(targetVec, Vector3.down * 1.2f, out hitBlock,
                    mouthfulDistance, LayerMask.GetMask("ClayBlock")))
                {
                    playerMovement.ChangeModel(PlayerState.spit);
                    playerMovement.Anim.SetTrigger(IsMouthful1);
                    AudioManager.Instance.PlayOneShotSFX(ESFXName.Spit);
                    StartCoroutine(CheckMouthfulAnimState());
                    Spit();
                }
            }
        }

        /// <summary>
        /// �տ� Ray�� ���� ClayBlock�� �ִٸ� �ӱݱ⸦ �Ѵ�.
        /// </summary>
        private void Mouthful()
        {
            playerMovement.Anim.SetTrigger(IsMouthful1);

            RaycastHit hit;
            if (Physics.Raycast(mouthfulRoot.position, mouthfulRoot.forward,
                out hit, mouthfulDistance, LayerMask.GetMask("ClayBlock")))
            {
                if (hit.collider.TryGetComponent(out ClayBlock clayBlock))
                {
                    if (clayBlock.IsMouthful)
                    {
                        playerMovement.ChangeModel(PlayerState.mouthful);
                        playerMovement.Anim.SetTrigger("isMouthful");
                        
                        clayBlock.OnMouthful();
                        clayBlock.transform.SetParent(transform);
                        targetClayBlock = clayBlock;
                        UIManager.Instance.SetMouthfulUI(targetClayBlock.ClayBlockType);
                    }

                    if (clayBlock.ClayBlockType == ClayBlockType.Apple)
                        targetClayBlock = null;
                }
            }
            else
            {
                if (Physics.Raycast(mouthfulRoot.position, mouthfulRoot.forward,
                    out hit, mouthfulDistance, LayerMask.GetMask("TemperObject")))
                {
                    if (hit.collider.TryGetComponent(out ClayBlock clayBlock))
                    {
                        playerMovement.ChangeModel(PlayerState.mouthful);
                        playerMovement.Anim.SetTrigger("isMouthful");

                        clayBlock.OnDivision();
                        UIManager.Instance.SetMouthfulUI(ClayBlockType.Toolbox);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// ClayBlock�� ��ġ�� �ʴ� ��쿡 ����´�.
        /// </summary>
        private void Spit()
        {
            if(targetClayBlock)
            {
                targetClayBlock.transform.SetParent(null);
                //var targetPos = transform.position + (Vector3.up * 0.5f + transform.forward * 1.5f);
                var targetPos = hitBlock.transform.position + Vector3.up * 1f;
                targetClayBlock.OnSpit(targetPos);
                targetClayBlock = null;
                UIManager.Instance.SetMouthfulUI();
                //currentClayBlock = null;
            }
        }

        private IEnumerator CheckMouthfulAnimState()
        {
            WaitForSeconds delay = new WaitForSeconds(0.01f);

            playerCtrl.PlayerMovement.SetMovement(false);

            playerMovement.Anim.SetBool("isWalk", false);

            while (true)
            {
                yield return delay;

                if (playerMovement.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    break;
            }

            playerCtrl.PlayerMovement.SetMovement(true);
        }

        public void SetTargetClayBlock(ClayBlock clayBlock) => targetClayBlock = clayBlock;
        #endregion
    }
}