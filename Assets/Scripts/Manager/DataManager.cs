using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Manager
{
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance;

        [SerializeField] private GameData mGameData;
        public GameData GameData { get => mGameData; }

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
            mGameData.gameState = GameState.Main;
        }
    }
}