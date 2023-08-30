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
        [System.Serializable]
        public class StageProperty
        {
            public int id;
            public int coin;
            public int prismPiece;
            public int[] sweetCandy; // 스테이지 달성률
            public float bestRecord; // 스테이지 최단 기록
            public float totalPlayTime; // 스테이지 전체 기록
            public bool isSaved;
            
            public StageProperty(int id, int coin, int prismPiece, int[] sweetCandy, float bestRecord, float totalPlayTime, bool isSaved)
            {
                this.id = id;
                this.coin = coin;
                this.prismPiece = prismPiece;
                this.sweetCandy = sweetCandy;
                this.bestRecord = bestRecord;
                this.totalPlayTime = totalPlayTime;
                this.isSaved = isSaved;
            }
        }

        [Header("== Game Property ==")]
        public bool isNewGame = true;
        
        public GameState gameState;

        public int currentStageIndex;

        public List<StageProperty> stageSaveFiles = new List<StageProperty>();
        
        public float bgm;
        public float sfx;
    }
}