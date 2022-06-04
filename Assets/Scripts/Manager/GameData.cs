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

    public enum GameSaveFile
    {
        Save1 = 0,
        Save2,
        Save3,
    }

    [System.Serializable]
    public class GameData
    {
        [Header("== Game Property ==")]
        public GameState gameState;

        [System.Serializable]
        public struct GameProperty
        {
            public int coin;
            public int prismPiece;
            public float playTime;
            public bool isSaved;
        }

        public GameProperty[] gameSaveFiles = new GameProperty[3];
    }
}