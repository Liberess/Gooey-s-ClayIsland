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

    private Animator anim;
    private BoxCollider boxCol;

    private void Awake()
    {
        playerCtrl = FindObjectOfType<Hun.Player.PlayerController>();

        anim = GetComponentInChildren<Animator>();
        boxCol = GetComponentInChildren<BoxCollider>();
    }

    public override void OnEnter()
    {
        switch (clayBlockType)
        {
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
            case ClayBlockType.Water:
                playerCtrl.PlayerMovement.playerGravityY = 1f;
                break;
        }
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

        // ���� ��ġ ���� ����
        Vector3 dir = blockB.transform.position - playerCtrl.gameObject.transform.position;
        dir = dir.normalized;

        if (0.5 < dir.x && dir.x <= 1)
        {
            dir = new Vector3(1, 0, 0);
        }
        else if (0 < dir.x && dir.x <= 0.5)
        {
            if (0 < dir.z)
                dir = new Vector3(0, 0, 1);
            else
                dir = new Vector3(0, 0, -1);
        }
        else if (-0.5 < dir.x && dir.x <= 0)
        {
            if (0 < dir.z)
                dir = new Vector3(0, 0, 1);
            else
                dir = new Vector3(0, 0, -1);
        }
        else if (-1 <= dir.x && dir.x <= -0.5)
        {
            dir = new Vector3(-1, 0, 0);
        }

        var tempRotation = Quaternion.LookRotation(dir);

        var tempObj = Instantiate(currentTemperPrefab,
            blockB.transform.position, tempRotation).GetComponent<ClayBlock>();
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
        if (srcType != ClayBlockType.Toolbox && destType != ClayBlockType.Toolbox)
        {
            isSuccess = false;
            return false;
        }

        // BlockB�� Ÿ�Կ� ���缭 ���� ��ġ�� �ռ��Ѵ�.
        if (srcType == ClayBlockType.Toolbox)
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
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.BouncyBall);
                    isSuccess = true;
                    return true;
            }
        }
        else if (destType == ClayBlockType.Toolbox)
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
                    currentTemperPrefab = GameManager.Instance.
                        GetTemperPrefab(TemperObjectType.BouncyBall);
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