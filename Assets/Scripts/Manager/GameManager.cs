using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hun.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private DataManager dataMgr;

        private GameObject player;
        private Hun.Player.PlayerHealth playerHealth;

        public GameSaveFile gameSaveFile;

        public int Coin { get; private set; }
        public float PlayTime { get; private set; }
        public int SceneIndex { get; private set; }
        public bool IsClear { get; private set; }
        public bool IsGamePlay { get; private set; }

        [Space(10), Header("== Game Menu UI =="), Space(5)]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject quitPanel;
        [SerializeField] private GameObject pausePanel;

        [SerializeField] private World.Stage curStage;

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
            
            IsGamePlay = true;
        }

        private void Start()
        {
            dataMgr = DataManager.Instance;

            player = GameObject.FindWithTag("Player");
            if(player)
            {
                playerHealth = player.GetComponent<Hun.Player.PlayerHealth>();
            }

            SceneIndex = SceneManager.GetActiveScene().buildIndex;

            Coin = 0;
            IsClear = false;
        }

        private void Update()
        {
            // Press Spacebar
            if (SceneIndex == 0)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    mainPanel.SetActive(false);
                    //menuPanel.SetActive(true);
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
        /// �ɼ�â�� ��Ʈ���ϴ� �Լ��̴�.
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
            if(Coin >= 11)
            {
                playerHealth.RestoreLife(1);
                Coin -= 11;
            }
            Debug.Log(Coin);
            UIManager.Instance.SetCoinUI(Coin);
        }

        public void StageClear()
        {
            IsClear = true;

            /*dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].sweetCandy[curStage.StageNum] = curStage.SweetCandy;
            if (dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].bestRecord[curStage.StageNum] > curStage.CurTime)
                dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].bestRecord[curStage.StageNum] = curStage.CurTime;

            dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].coin += Coin;
            dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].playTime += PlayTime;
            dataMgr.GameData.gameState = GameState.Lobby;*/
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
        /// ���ο� ������ �����ϴ� �Լ��̴�.
        /// </summary>
        public void NewGame()
        {
            dataMgr.GameData.gameState = GameState.Lobby;
            LoadScene("LobbyScene");
        }

        /// <summary>
        /// ���� ������ �̾��ϴ� �Լ��̴�.
        /// </summary>
        public void ContinueGame()
        {
            dataMgr.GameData.gameState = GameState.Lobby;
            LoadScene("LobbyScene");
        }

        /// <summary>
        /// ������ �����Ű�� �Լ��̴�.
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}