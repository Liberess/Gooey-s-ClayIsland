using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hun.Manager;

public class ClayBlockTile : ClayBlock
{
    private Hun.Player.PlayerController playerCtrl;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField, Range(0f, 5f)] private float moveSpeed = 2f;
    private GameObject currentTemperPrefab = null;
    private DirectionVector directionVector = new DirectionVector();

    [SerializeField] private bool isPlayerOver = false; //�÷��̾ �� ���� ����!

    private Animator anim;
    private BoxCollider boxCol;

    private void OnDrawGizmosSelected()
    {
        if (ClayBlockType != ClayBlockType.Ice)
            return;

        Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue };

        Vector3 defaultVec = transform.position + Vector3.up;

        Vector3[] newVecs =
            {
                defaultVec + Vector3.forward,
                defaultVec + Vector3.back,
                defaultVec + Vector3.left,
                defaultVec + Vector3.right
            };

        for (int i = 0; i < newVecs.Length; i++)
        {
            Gizmos.color = Color.Lerp(Color.white, colors[i], 0.5f);
            Gizmos.DrawSphere(newVecs[i], 0.5f);
        }
    }

    private void Awake()
    {
        playerCtrl = FindObjectOfType<Hun.Player.PlayerController>();

        anim = GetComponentInChildren<Animator>();
        boxCol = GetComponentInChildren<BoxCollider>();

        var defaultVec = boxCol.center + transform.position + Vector3.up;

        Vector3[] newVecs =
            {
                defaultVec + Vector3.forward,
                defaultVec + Vector3.back,
                defaultVec + Vector3.left,
                defaultVec + Vector3.right
            };

        directionVector.SetVectors(newVecs);
        directionVector.currentVectors[(int)DirectionType.Forward] = Vector3.back;
        directionVector.currentVectors[(int)DirectionType.Back] = Vector3.forward;
        directionVector.currentVectors[(int)DirectionType.Left] = Vector3.right;
        directionVector.currentVectors[(int)DirectionType.Right] = Vector3.left;
    }

    private void Update()
    {
        if (isPlayerOver && !playerCtrl.PlayerInteract.IsSlipIce)
        {
            if (!IsBlockedAround())
                return;

            Vector3 dirVec = Vector3.zero;

            for (int i = 0; i < directionVector.dirVectors.Length; i++)
            {
                if (Physics.Raycast(transform.position + Vector3.up,
                    directionVector.defaultVectors[i], 1f, ~targetLayer))
                    continue;

                Collider[] colliders = Physics.OverlapBox(directionVector.dirVectors[i],
                    boxCol.size / 2, Quaternion.identity, targetLayer);

                if (colliders != null && colliders.Length > 0)
                    dirVec = directionVector.defaultVectors[i];
            }

            if (dirVec != Vector3.zero && -playerCtrl.PlayerMovement.MovingInputValue == dirVec)
                playerCtrl.PlayerMovement.AddMoveForce(dirVec);
        }
    }

    public override void OnEnter()
    {
        switch (clayBlockType)
        {
            case ClayBlockType.Ice:
                isPlayerOver = true;

                if (playerCtrl.PlayerMovement.IsOverIce)
                    break;

                Vector3 dirVec = Vector3.zero;

                for (int i = 0; i < directionVector.dirVectors.Length; i++)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionVector.defaultVectors[i], out hit, 1f))
                    {
                        if (hit.collider.TryGetComponent(out ClayBlockTile clayBlockTile))
                        {
                            if (clayBlockTile.ClayBlockType == ClayBlockType.Ice)
                                continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    Collider[] colliders = Physics.OverlapBox(directionVector.dirVectors[i],
                        boxCol.size / 2, Quaternion.identity, targetLayer);

                    if (colliders != null && colliders.Length > 0)
                        dirVec = directionVector.currentVectors[i];
                }

                if (dirVec != Vector3.zero)
                    playerCtrl.PlayerMovement.AddMoveForce(dirVec);
                break;
            case ClayBlockType.Water:
                playerCtrl.PlayerMovement.InitializeMovingVector();
                playerCtrl.PlayerMovement.playerGravityY = 0.001f;
                break;
        }
    }

    public override void OnStay()
    {

    }

    public override void OnExit()
    {
        switch (clayBlockType)
        {
            case ClayBlockType.Ice:
                isPlayerOver = false;
                break;
            case ClayBlockType.Water:
                playerCtrl.PlayerMovement.playerGravityY = 1f;
                break;
        }
    }

    /// <summary>
    /// �ش� �� ���� 4������ �������� Ȯ���Ѵ�.
    /// </summary>
    private bool IsBlockedAround()
    {
        for (int i = 0; i < directionVector.dirVectors.Length; i++)
        {
            if (Physics.Raycast(transform.position + Vector3.up,
                    directionVector.defaultVectors[i], 1f, ~targetLayer))
                return true;
        }

        return false;
    }

    public override void OnMouthful()
    {
        if (!IsMouthful)
            return;

        base.OnMouthful();
        StopAllCoroutines();
        StartCoroutine(MouthfulCo());
    }

    private IEnumerator MouthfulCo()
    {
        anim.SetTrigger("DoMouthful");
        boxCol.enabled = false;

        if (clayBlockType == ClayBlockType.Sand)
            GetComponent<Rigidbody>().useGravity = false;

        var playerMouthRoot = FindObjectOfType<Hun.Player.PlayerMouthful>().MouthfulRoot;

        while (true)
        {
            float distance = Vector3.Distance(transform.position, playerMouthRoot.transform.position);
            if (distance <= 0.001f)
                break;

            transform.position = Vector3.MoveTowards(transform.position,
                playerMouthRoot.transform.position, moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        boxCol.enabled = true;
        gameObject.SetActive(false);

        yield return null;
    }

    public override void OnSpit(Vector3 targetPos)
    {
        if (!IsMouthful)
            return;

        base.OnSpit(targetPos);

        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(SpitCo(targetPos));
    }

    private IEnumerator SpitCo(Vector3 targetPos)
    {
        anim.SetTrigger("DoSpit");

        var playerMouthRoot = FindObjectOfType<Hun.Player.PlayerMouthful>().MouthfulRoot;
        transform.position = playerMouthRoot.position;
        boxCol.enabled = false;

        if (clayBlockType == ClayBlockType.Sand)
            GetComponent<Rigidbody>().useGravity = false;

        while (true)
        {
            float distance = Vector3.Distance(transform.position, targetPos);
            if (distance <= 0.001f)
                break;

            transform.position = Vector3.MoveTowards(transform.position,
                targetPos, moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        gameObject.transform.position = targetPos;
        boxCol.enabled = true;

        if (clayBlockType == ClayBlockType.Sand)
            GetComponent<Rigidbody>().useGravity = true;

        yield return null;
    }

    public override void OnFusion(ClayBlock blockA, ClayBlock blockB)
    {
        if (blockA == null || blockB == null)
            throw new System.Exception("blockA �Ǵ� blockB�� ���� null�Դϴ�.");

        var tempObj = Instantiate(currentTemperPrefab,
            blockB.transform.position, Quaternion.identity).GetComponent<ClayBlock>();
        tempObj.currentClayBlocks.Initialize();

        tempObj.currentClayBlocks[0] = blockA;
        tempObj.currentClayBlocks[1] = blockB;

        base.OnFusion(blockA, blockB); //Destroy
    }

    private bool IsSuccessGetTemperPrefab(ref bool isSuccess,
        ClayBlockType srcType, ClayBlockType destType, ref GameObject temperPrefab)
    {
        // ���� Ÿ���� ���̶�� ��ĥ �� ����.
        if (srcType == destType)
        {
            isSuccess = false;
            return false;
        }

        // ������ ���� ��ĥ �� �ִ�.
        if (srcType != ClayBlockType.Rainbow && destType != ClayBlockType.Rainbow)
        {
            isSuccess = false;
            return false;
        }

        // BlockB�� Ÿ�Կ� ���缭 ���� ��ġ�� �ռ��Ѵ�.
        if (srcType == ClayBlockType.Rainbow)
        {
            switch (destType)
            {
                case ClayBlockType.Grass:
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.Canon);
                    isSuccess = true;
                    return true;

                case ClayBlockType.Sand:
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.Trampoline);
                    isSuccess = true;
                    return true;

                case ClayBlockType.Ice:
                    Debug.Log("���ʺ� �߰�");
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.Trampoline);
                    isSuccess = true;
                    return true;
            }
        }
        else if (destType == ClayBlockType.Rainbow)
        {
            switch (srcType)
            {
                case ClayBlockType.Grass:
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.Canon);
                    isSuccess = true;
                    return true;

                case ClayBlockType.Sand:
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.Trampoline);
                    isSuccess = true;
                    return true;

                case ClayBlockType.Ice:
                    Debug.Log("���ʺ� �߰�");
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.Trampoline);
                    isSuccess = true;
                    return true;
            }
        }

        isSuccess = false;
        return false;
    }

    public bool IsSuccessFusion(ClayBlock blockA, ClayBlock blockB)
    {
        bool isSuccess = false;
        currentTemperPrefab = null;

        if(IsSuccessGetTemperPrefab(ref isSuccess, blockA.ClayBlockType,
            blockB.ClayBlockType, ref currentTemperPrefab))
        {
            if(isSuccess && currentTemperPrefab)
                OnFusion(blockA, blockB);
        }

        return isSuccess;
    }
}