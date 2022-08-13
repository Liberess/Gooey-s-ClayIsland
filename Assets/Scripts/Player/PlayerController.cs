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

        /*        private void OnTriggerEnter(Collider other)
                {
                    if (other.TryGetComponent(out Hun.Obstacle.Portal portal))
                    {
                        if (portal.EftType == Hun.Obstacle.Portal.EffectType.Enter)
                            TeleportPlayerTransform(portal.TargetPos);
                        else
                            PlayerInteract.OnWalkedThroughPortal(portal);
                    }

                    if (other.TryGetComponent(out IObstacle obstacle))
                        obstacle.OnEnter();

                    if (other.TryGetComponent(out Hun.Item.IItem item))
                        item.OnEnter();

                    if (other.TryGetComponent(out ClayBlock clayBlock))
                        clayBlock.OnEnter();
                }

                private void OnTriggerExit(Collider other)
                {
                    if (other.TryGetComponent(out IObstacle obstacle))
                        obstacle.OnExit();

                    if (other.TryGetComponent(out ClayBlock clayBlock))
                        clayBlock.OnExit();
                }*/

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out ClayBlock clayBlock))
            {
                clayBlock.OnEnter();

                if (clayBlock.ClayBlockType != ClayBlockType.Ice)
                {
                    var root = PlayerMouthful.MouthfulRoot;
                    if (Physics.Raycast(root.position, root.forward, 1f, LayerMask.GetMask("ClayBlock")))
                        PlayerInteract.SetSlipIceState(false);
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider.TryGetComponent(out ClayBlock clayBlock))
                clayBlock.OnExit();
        }
    }
}