using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hun.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private DataManager dataMgr;

        public GameSaveFile gameSaveFile;

        public int Coin { get; private set; }
        public float PlayTime { get; private set; }
        public int SceneIndex { get; private set; }

        [Space(10), Header("== Game Menu UI =="), Space(5)]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject quitPanel;
        [SerializeField] private GameObject pausePanel;

        [Space(10), Header("== Clay Block Object Prefabs =="), Space(5)]
        [SerializeField] private List<GameObject> clayBlockTilePrefabList = new List<GameObject>();
        public GameObject GetClayBlockTilePrefab(ClayBlockType clayBlockType)
            => clayBlockTilePrefabList[(int)clayBlockType];
        [SerializeField] private List<GameObject> temperObjPrefabList = new List<GameObject>();
        public GameObject GetTemperPrefab(TemperObjectType type)
            => temperObjPrefabList[(int)type];

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if(Instance != this)
                Destroy(gameObject);
        }

        private void Start()
        {
            dataMgr = DataManager.Instance;

            SceneIndex = SceneManager.GetActiveScene().buildIndex;

            Coin = 0;
        }

        private void Update()
        {
            // Press Spacebar
            if (SceneIndex == 0)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    mainPanel.SetActive(false);
                    menuPanel.SetActive(true);
                }
            }

            // Option Panel Control
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (dataMgr.GameData.gameState == GameState.Lobby)
                {
                    QuitControl();
                    UIManager.Instance.SetSelectStageUI(false);
                }
                else
                {
                    OptionControl();
                }
            }
        }

        /// <summary>
        /// 옵션창을 컨트롤하는 함수이다.
        /// </summary>
        public void OptionControl()
        {
            if(pausePanel == null)
            {
                var parent = GameObject.Find("== UI ==").transform.Find("GameCanvas");
                pausePanel = parent.Find("PausePanel").gameObject;
            }

            if(pausePanel.activeSelf)
            {
                Time.timeScale = 1f;
                pausePanel.SetActive(false);
            }
            else
            {
                pausePanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        public void QuitControl()
        {
            if (quitPanel == null)
            {
                var parent = GameObject.Find("== UI ==").transform.Find("GameCanvas");
                quitPanel = parent.Find("QuitPanel").gameObject;
            }

            if (quitPanel.activeSelf)
                quitPanel.SetActive(false);
            else
                quitPanel.SetActive(true);
        }

        public void LoadScene(string sceneName)
        {
            if (sceneName.Contains("Stage"))
                dataMgr.GameData.gameState = GameState.Stage;
            else if(sceneName.Contains("Lobby"))
                dataMgr.GameData.gameState = GameState.Lobby;

            Time.timeScale = 1f;
            LoadingManager.LoadScene(sceneName);
        }

        public void LevelStart()
        {

        }

        public void GetCoin(int value)
        {
            Coin += value;
            UIManager.Instance.SetCoinUI(Coin);
        }

        public void StageClear()
        {
            dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].coin += Coin;
            dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].playTime += PlayTime;
            dataMgr.GameData.gameState = GameState.Lobby;
            LoadScene("WorldMapScene");
        }

        #region Game Load & Quit

        public void StartGame(GameSaveFile saveFileNum)
        {
            gameSaveFile = saveFileNum;

            if (dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].isSaved == false)
            {
                NewGame();
            }
            else
            {
                ContinueGame();
            }
        }

        /// <summary>
        /// 새로운 게임을 생성하는 함수이다.
        /// </summary>
        public void NewGame()
        {
            dataMgr.GameData.gameState = GameState.Lobby;
            LoadScene("WorldMapScene");
        }

        /// <summary>
        /// 지난 게임을 이어하는 함수이다.
        /// </summary>
        public void ContinueGame()
        {
            dataMgr.GameData.gameState = GameState.Lobby;
            LoadScene("WorldMapScene");
        }

        /// <summary>
        /// 게임을 종료시키는 함수이다.
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}