using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Manager
{
    public enum GameState
    {
        Main = 0,
        Lobby,
        Stage
    }

    [System.Serializable]
    public class GameData
    {
        [Header("== Game Property ==")]
        public GameState gameState;
        public int coin;
    }
}