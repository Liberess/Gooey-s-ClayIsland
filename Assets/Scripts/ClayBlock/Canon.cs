using UnityEngine;
using Hun.Player;

namespace Hun.Obstacle
{
    public class Canon : ClayBlock
    {
        private PlayerController player;

        [SerializeField] private Animator anim;

        private Vector3 destPos;

        private GameObject curUser; 

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
            player = FindObjectOfType<PlayerController>();
        }

        public override void OnEnter()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                if (hit.distance < 1.0f)
                {
                    return;
                }

                anim.SetTrigger("isUsed");
                player.PlayerInteract.SetCanonState(true);
                player.PlayerMovement.SetOverIceState(false);

                destPos = hit.transform.position;
                destPos.y = transform.position .y - 0.5f;

                if (!hit.transform.gameObject.CompareTag("Trampiline"))
                {
                    float tempY = transform.rotation.eulerAngles.y;
                    Debug.Log(tempY);
                    if (-10 <= tempY && tempY <= 10)
                    {
                        destPos.x = transform.position.x;
                        destPos.z -= 1f;
                    }  
                    else if (80 <= tempY && tempY <= 100)
                    {
                        destPos.z = transform.position.z;
                        destPos.x -= 1f;
                    }   
                    else if (170 <= tempY && tempY <= 190)
                    {
                        destPos.x = transform.position.x;
                        destPos.z += 1f;
                    }  
                    else if (260 <= tempY && tempY <= 280)
                    {
                        destPos.z = transform.position.z;
                        destPos.x += 1f;
                    }
                }

                player.PlayerInteract.FiredToPosByCanon(transform, destPos);
            }

            Debug.Log(destPos);
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red, 3);
        }

        public override void OnStay()
        {

        }

        public override void OnExit()
        {

        }

        public override void OnMouthful()
        {
            if (!IsMouthful)
                return;

            base.OnMouthful();

            gameObject.SetActive(false);
        }

        public override void OnSpit(Vector3 targetPos)
        {
            if (!IsMouthful)
                return;

            // player오브젝트가 targetPos를 바라보는 방향으로 회전
            Vector3 dir = targetPos - player.gameObject.transform.position;
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

            gameObject.transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(dir);

            gameObject.SetActive(true);
        }
    }
}
