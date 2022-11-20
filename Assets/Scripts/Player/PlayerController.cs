using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Hun.Player;

namespace Hun.Player
{
    public class PlayerController : MonoBehaviour
    {
        public Hun.Camera.MainCamera MainCamera { get; private set; }

        public PlayerHealth PlayerHealth { get; private set; }
        public PlayerInteract PlayerInteract { get; private set; }
        public PlayerMouthful PlayerMouthful { get; private set; }
        public PlayerMovement PlayerMovement { get; private set; }

        private Transform curCheckPoint;

        public static event UnityAction<PlayerController> PlayerSpawnedEvent;
        public event UnityAction PlayerDiedEvent;

        private void Awake()
        {
            PlayerHealth = GetComponent<PlayerHealth>();
            PlayerInteract = GetComponent<PlayerInteract>();
            PlayerMouthful = GetComponent<PlayerMouthful>();
            PlayerMovement = GetComponent<PlayerMovement>();
        }

        private void Start()
        {
            PlayerSpawnedEvent?.Invoke(this);

            if (MainCamera == null)
                MainCamera = FindObjectOfType<Hun.Camera.MainCamera>();
        }

        public void TeleportPlayerTransform(Transform targetPos)
        {
            PlayerMovement.enabled = false;
            transform.position = targetPos.position;
            PlayerMovement.enabled = true;
        }

        public void TeleportToCheckPoint()
        {
            TeleportPlayerTransform(curCheckPoint);
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (other.TryGetComponent(out Hun.Obstacle.Portal portal))
            //{
            //    if (portal.EftType == Hun.Obstacle.Portal.EffectType.Enter)
            //        TeleportPlayerTransform(portal.TargetPos);
            //    else
            //        PlayerInteract.OnWalkedThroughPortal(portal);
            //}

            //if (other.TryGetComponent(out IObstacle obstacle))
            //    obstacle.OnEnter();

            if (other.TryGetComponent(out Hun.Item.IItem item))
                item.OnEnter();

            //if (other.TryGetComponent(out ClayBlock clayBlock))
            //    clayBlock.OnEnter();
        }

        //private void OnTriggerExit(Collider other)
        //{
        //    if (other.TryGetComponent(out IObstacle obstacle))
        //        obstacle.OnExit();

        //    if (other.TryGetComponent(out ClayBlock clayBlock))
        //        clayBlock.OnExit();
        //}

        private void OnCollisionEnter(Collision collision)
        {
            if (!PlayerMovement.IsDie && collision.collider.TryGetComponent(out ClayBlock clayBlock))
                clayBlock.OnEnter();

            /*if (collision.collider.TryGetComponent(out ClayBlock clayBlock))
            {
                clayBlock.OnEnter();

                if (PlayerInteract.IsSlipIce && clayBlock.ClayBlockType != ClayBlockType.Ice)
                {
                    float distance = Vector3.Distance(transform.position + (transform.up * 0.5f),
                            clayBlock.transform.position);
                    if (distance <= 1f)
                    {
                        Debug.Log("�տ� ����");
                        PlayerInteract.SetSlipIceState(false);
                    }
                    else
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, Vector3.down,
                            out hit, 0.3f, LayerMask.GetMask("ClayBlock")))
                        {
                            if (hit.collider != null && hit.collider.TryGetComponent(out ClayBlock clay))
                            {
                                if (clay.ClayBlockType != ClayBlockType.Ice)
                                    PlayerInteract.SetSlipIceState(false);
                            }
                        }
                    }
                }
            }*/
        }

        private void OnCollisionStay(Collision collision)
        {
            /*         if (collision.collider.TryGetComponent(out ClayBlock clayBlock))
                         clayBlock.OnStay();*/
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!PlayerMovement.IsDie && collision.collider.TryGetComponent(out ClayBlock clayBlock))
                clayBlock.OnExit();
        }
    }
}