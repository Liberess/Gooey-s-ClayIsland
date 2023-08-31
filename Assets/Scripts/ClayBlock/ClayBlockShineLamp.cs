using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayBlockShineLamp : ClayBlock
{
    [SerializeField, Range(0f, 5f)] private float moveSpeed = 2f;
    
    private Animator anim;
    private SphereCollider sphereCol;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sphereCol = GetComponent<SphereCollider>();
    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void OnStay()
    {

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
        //anim.SetTrigger("DoMouthful");
        sphereCol.enabled = false;

        if (clayBlockType == ClayBlockType.Sand)
            GetComponent<Rigidbody>().useGravity = false;

        var playerMouthRoot = FindObjectOfType<Hun.Player.PlayerMouthful>().MouthfulRoot;

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                playerMouthRoot.transform.position, moveSpeed * Time.deltaTime);
            
            float newScale = Mathf.Lerp(transform.localScale.x, 0f, moveSpeed * 2f * Time.deltaTime);
            transform.localScale = new Vector3(newScale, newScale, newScale);

            float newRotate = 3.0f * moveSpeed;
            transform.Rotate(newRotate, newRotate, newRotate, Space.Self);
            
            float distance = Vector3.Distance(transform.position, playerMouthRoot.transform.position);
            if (distance <= 0.0001f)
            {
                transform.position = playerMouthRoot.transform.position;
                //transform.localRotation = Quaternion.Euler(-360f, -360f, -360f);
                transform.localRotation = Quaternion.Euler(-360f, -360f, -360f);
                transform.eulerAngles = new Vector3(-360f, -360f, -360f);
                break;
            }
            
            yield return new WaitForEndOfFrame();
        }

        sphereCol.enabled = true;
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
        //anim.SetTrigger("DoSpit");

        var playerMouthRoot = FindObjectOfType<Hun.Player.PlayerMouthful>().MouthfulRoot;
        transform.position = playerMouthRoot.position;
        sphereCol.enabled = false;

        if (clayBlockType == ClayBlockType.Sand)
            GetComponent<Rigidbody>().useGravity = false;

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                targetPos, moveSpeed * Time.deltaTime);
            
            float newScale = Mathf.Lerp(transform.localScale.x, 1f, moveSpeed * 2f * Time.deltaTime);
            transform.localScale = new Vector3(newScale, newScale, newScale);

            float newRotate = 3.0f * moveSpeed;
            transform.Rotate(newRotate, newRotate, newRotate, Space.Self);
            
            float distance = Vector3.Distance(transform.position, targetPos);
            if (distance <= 0.0001f)
            {
                transform.position = targetPos;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.Euler(Vector3.zero);
                break;
            }
            
            yield return new WaitForEndOfFrame();
        }

        sphereCol.enabled = true;

        if (clayBlockType == ClayBlockType.Sand)
            GetComponent<Rigidbody>().useGravity = true;
        
        var colliders = Physics.OverlapSphere(transform.position, 1f,
            LayerMask.GetMask("ShineTower"));

        foreach(var tower in colliders)
        {
            yield return null;
            gameObject.transform.position += new Vector3(0, 0.5f, 0);
            Destroy(tower);
            Destroy(this, 0.5f);
            Hun.Manager.GameManager.Instance.StageClear();
        }

        yield return null;
    }
}