using UnityEngine;

namespace Hun.World
{
    public class Stage : MonoBehaviour
    {
        public static Stage Instance { get; private set; }

        [SerializeField] private int stageNum;
        public int StageNum { get => stageNum; }

        [SerializeField] private float stageTimer;
        private float waitingTime = 1f;
        private float curTime;
        public float CurTime { get => curTime; }

        private bool isGameOver = false;

        private GameObject player;
        private Hun.Player.PlayerHealth playerHealth;

        Manager.GameManager gameManager;
        Manager.UIManager uiManager;

        public int SweetCandy { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);

            Player.PlayerController.PlayerSpawnedEvent += OnPlayerSpawned;
        }

        private void Start()
        {
            gameManager = Manager.GameManager.Instance;
            uiManager = Manager.UIManager.Instance;

            player = GameObject.FindWithTag("Player");
            playerHealth = player.GetComponent<Hun.Player.PlayerHealth>();

            curTime = stageTimer + waitingTime;
        }

        private void Update()
        {
            CountTimer();
        }

        private void CountTimer()
        {
            if (gameManager.IsClear)
                return;

            curTime -= Time.deltaTime;
            uiManager.SetStageTimerUI((int)(curTime - 1));

            if (curTime <= 0)
            {
                Entity.DamageMessage dmgMsg = new Entity.DamageMessage();
                dmgMsg.damager = gameObject;
                dmgMsg.dmgAmount = 3;
                //dmgMsg.hitNormal = transform.position;
                //dmgMsg.hitPoint = transform.position;
                playerHealth.ApplyDamage(dmgMsg);
            }
        }

        private void UpdateTimerUI()
        {

        }

        public void GetCandy(int value)
        {
            SweetCandy += value;
            Debug.Log(SweetCandy);
            //UI�ݿ�
        }

        /// <summary>
        /// �÷��̾� ������ ȣ��Ǵ� �޼���
        /// </summary>
        /// <param name="player">�÷��̾� ������Ʈ</param>
        private void OnPlayerSpawned(Player.PlayerController player)
        {
            //player.Life = player.maxLife;
            player.PlayerDiedEvent += OnPlayerDied;
        }

        /// <summary>
        /// �÷��̾� ����� ȣ��Ǵ� �޼���
        /// </summary>
        private void OnPlayerDied()
        {
            isGameOver = true;
        }
    }
}