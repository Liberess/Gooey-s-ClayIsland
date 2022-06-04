using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hun.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private DataManager dataMgr;

        public int SceneIndex { get; private set; }

        public GameSaveFile gameSaveFile;
        public int Coin { get; private set; }
        public int PrismPiece { get; private set; }
        public float PlayTime { get; private set; }

        [SerializeField] private int clearObjCount;
        public int ClearObjCount
        {
            get => clearObjCount;
            private set
            {
                if (clearObjCount < 0)
                    clearObjCount = 0;
                else
                    clearObjCount = value;

                if(UIManager.Instance != null)
                    UIManager.Instance.SetClearObjectCountUI(clearObjCount);
            }
        }

        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject menuPanel;

        [SerializeField] private GameObject quitPanel;
        [SerializeField] private GameObject pausePanel;

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

            if (SceneIndex == 0)
            {
                ClearObjCount = 0;
            }

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
            SceneManager.LoadScene(sceneName);
        }

        public void LevelStart()
        {

        }

        public void GetCoin(int value)
        {
            Coin += value;
            UIManager.Instance.SetCoinUI(Coin);
        }

        public void GetClearObject()
        {
            //UIManager.Instance.SetClearObjectCountUI(++clearObjCount);

            ++clearObjCount;

            if(clearObjCount >= 2)
                StageClear();
        }

        public void StageClear()
        {
            dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].coin += Coin;
            dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].playTime += PlayTime;
            dataMgr.GameData.gameState = GameState.Lobby;
            LoadScene("LobbyScene");
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
            SceneManager.LoadScene("LobbyScene");
        }

        /// <summary>
        /// 지난 게임을 이어하는 함수이다.
        /// </summary>
        public void ContinueGame()
        {
            dataMgr.GameData.gameState = GameState.Lobby;
            SceneManager.LoadScene("LobbyScene");
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