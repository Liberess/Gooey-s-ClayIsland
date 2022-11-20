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

        private Animator GameEndEffect;

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
        }

        private void Start()
        {
            
        }

        private void Update()
        {

        }


        public void GetCandy(int value)
        {
            SweetCandy += value;
            Debug.Log(SweetCandy);
            //UI�ݿ�
        }

        /// <summary>
        /// �÷��̾� ����� ȣ��Ǵ� �޼���
        /// </summary>
        private void OnPlayerDied()
        {
            isGameOver = true;
            GameEndEffect.SetTrigger("GameEnd");

        }
    }
}