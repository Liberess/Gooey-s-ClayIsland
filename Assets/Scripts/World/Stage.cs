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
            //UI반영
        }

        /// <summary>
        /// 플레이어 스폰시 호출되는 메서드
        /// </summary>
        /// <param name="player">플레이어 오브젝트</param>
        private void OnPlayerSpawned(Player.PlayerController player)
        {
            //player.Life = player.maxLife;
            player.PlayerDiedEvent += OnPlayerDied;
        }

        /// <summary>
        /// 플레이어 사망시 호출되는 메서드
        /// </summary>
        private void OnPlayerDied()
        {
            isGameOver = true;
        }
    }
}