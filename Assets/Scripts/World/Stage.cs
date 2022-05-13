using UnityEngine;

namespace Hun.World
{
    public class Stage : MonoBehaviour
    {
        private bool isGameOver = false;

        private void Awake()
        {
            Player.Player.PlayerSpawnedEvent += OnPlayerSpawned;
        }

        /// <summary>
        /// 플레이어 스폰시 호출되는 메서드
        /// </summary>
        /// <param name="player">플레이어 오브젝트</param>
        private void OnPlayerSpawned(Player.Player player)
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