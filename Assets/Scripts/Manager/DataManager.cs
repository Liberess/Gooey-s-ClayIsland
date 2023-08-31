using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hun.Obstacle;
using UnityEngine;

namespace Hun.Manager
{
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; private set; }

        private readonly string GameDataFileName = "/GameData.json";

        [SerializeField] private GameData mGameData = null;
        public GameData GameData
        {
            get
            {
                if (mGameData == null)
                {
                    LoadGameData();
                    SaveGameData();
                }

                return mGameData;
            }
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadGameData();
        }

        #region Init, Save, Load Data

        public void InitializedGameData()
        {
            mGameData.isNewGame = false;
            
            mGameData.gameState = GameState.Main;

            mGameData.stageSaveFiles = new List<GameData.StageProperty>();
            
            mGameData.sfx = 50f;
            mGameData.bgm = 50f;
        }

        public void LoadGameData()
        {
            string filePath = Application.persistentDataPath + GameDataFileName;
            if (File.Exists(filePath))
            {
                string code = File.ReadAllText(filePath);
                byte[] bytes = System.Convert.FromBase64String(code);
                string fromJsonData = System.Text.Encoding.UTF8.GetString(bytes);
                mGameData = JsonUtility.FromJson<GameData>(fromJsonData);
            }
            else
            {
                mGameData = new GameData();
                File.Create(Application.persistentDataPath + GameDataFileName);
                InitializedGameData();
            }
        }

        public void SaveGameData()
        {
            if (mGameData == null)
                return;

            string filePath = Application.persistentDataPath + GameDataFileName;
            string toJsonData = JsonUtility.ToJson(mGameData);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(toJsonData);
            string code = System.Convert.ToBase64String(bytes);
            File.WriteAllText(filePath, code);
        }

        /*private void SaveTime()
        {
            mGameData.lastLogInTime = DateTime.Now;
            mGameData.lastLogInTimeStr = mGameData.lastLogInTime.ToString();
        }*/

        #endregion

        public GameData.StageProperty GetStageSaveFile(int stageIndex)
        {
            var data = mGameData.stageSaveFiles.Find(x => x.id == stageIndex);
            if (data != null)
                return data;
            
            data = new GameData.StageProperty(stageIndex, 0, 0, new int[3], float.MaxValue, 0f, false);
            mGameData.stageSaveFiles.Add(data);
            mGameData.stageSaveFiles = mGameData.stageSaveFiles.OrderBy(x => x.id).ToList();
            return data;
        }

        public float GetBestPlayTime(int stageIndex)
        {
            if (stageIndex >= 0 && stageIndex < mGameData.stageSaveFiles.Count)
                return mGameData.stageSaveFiles[stageIndex].bestRecord;
            
            return -1;
        }

        private void OnApplicationQuit() => SaveGameData();
    }
}