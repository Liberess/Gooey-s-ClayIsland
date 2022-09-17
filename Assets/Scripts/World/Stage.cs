using UnityEngine;

namespace Hun.World
{
    public class Stage : MonoBehaviour
    {
        public static Stage Instance { get; private set; }

        [SerializeField] private int stageNum;
        public int StageNum { get => stageNum; }

        private bool isGameOver = false;

        public int SweetCandy { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);

            Player.PlayerController.PlayerSpawnedEvent += OnPlayerSpawned;
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