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
        /// �÷��̾� ������ ȣ��Ǵ� �޼���
        /// </summary>
        /// <param name="player">�÷��̾� ������Ʈ</param>
        private void OnPlayerSpawned(Player.Player player)
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