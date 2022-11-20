using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hun.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private DataManager dataMgr;
        private UIManager uiManager;

        private GameObject player;
        private Hun.Player.PlayerHealth playerHealth;

        public GameSaveFile gameSaveFile;

        public int Coin { get; private set; }
        public float PlayTime { get; private set; }
        public int SceneIndex { get; private set; }
        public bool IsClear { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsFailed { get; private set; }
        public bool IsGamePlay { get; private set; }

        [Space(10), Header("== Game Menu UI =="), Space(5)]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject quitPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject clearPanel;
        [SerializeField] private GameObject failedPanel;
        [SerializeField] private GameObject gameOverPanel;

        [Space(10), Header("== Clay Block Object Prefabs =="), Space(5)]
        [SerializeField] private List<GameObject> clayBlockTilePrefabList = new List<GameObject>();
        public GameObject GetClayBlockTilePrefab(ClayBlockType clayBlockType)
            => clayBlockTilePrefabList[(int)clayBlockType];
        [SerializeField] private List<GameObject> temperObjPrefabList = new List<GameObject>();
        public GameObject GetTemperPrefab(TemperObjectType type)
            => temperObjPrefabList[(int)type];

        [Space(15)]
        [SerializeField] private Animator GameEndEffect;

        //월드 관련 변수
        [Space(10), Header("== Game Stage =="), Space(5)]
        [SerializeField] private int stageNum;
        [SerializeField] private float stageTimer;
        private float waitingTime = 1f;
        private float curTime;
        public float CurTime { get => curTime; }

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
            uiManager = UIManager.Instance;

            player = GameObject.FindWithTag("Player");
            if(player)
            {
                playerHealth = player.GetComponent<Hun.Player.PlayerHealth>();
            }

            SceneIndex = SceneManager.GetActiveScene().buildIndex;

            Coin = 0;
            IsClear = false;
            IsFailed = false;
            IsGameOver = false;

            curTime = stageTimer + waitingTime;
        }

        private void Update()
        {
            CountTimer();

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
            if(IsGamePlay && Input.GetKeyDown(KeyCode.Escape))
            {
                if (dataMgr.GameData.gameState == GameState.Lobby)
                {
                    QuitControl();
                    uiManager.SetSelectStageUI(false);
                }
                else
                {
                    OptionControl();
                }
            }

            if (!IsGamePlay)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (IsClear)
                        LoadScene("LobbyScene" + stageNum);
                    else if (IsFailed)
                        LoadScene("1-" + stageNum);
                    else if (IsGameOver)
                        LoadScene("LobbyScene" + stageNum);
                }

            }
        }

        private void CountTimer()
        {
            if (IsClear)
                return;

            curTime -= Time.deltaTime;
            uiManager.SetStageTimerUI((int)(curTime - 1));

            if (curTime <= 0)
            {
                Entity.DamageMessage dmgMsg = new Entity.DamageMessage();
                dmgMsg.damager = gameObject;
                dmgMsg.dmgAmount = 3;
                //dmgMsg.hitNormal = transform.position;
                //dmgMsg.hitPoint = transform.position;
                playerHealth.ApplyDamage(dmgMsg);
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
            /*
            if (sceneName.Contains("Stage"))
                dataMgr.GameData.gameState = GameState.Stage;
            else if(sceneName.Contains("Lobby"))
                dataMgr.GameData.gameState = GameState.Lobby;
            */

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
            StartCoroutine(OnGameResultPanel());

            /*dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].sweetCandy[curStage.StageNum] = curStage.SweetCandy;
            if (dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].bestRecord[curStage.StageNum] > curStage.CurTime)
                dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].bestRecord[curStage.StageNum] = curStage.CurTime;

            dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].coin += Coin;
            dataMgr.GameData.gameSaveFiles[(int)gameSaveFile].playTime += PlayTime;
            dataMgr.GameData.gameState = GameState.Lobby;*/

            //LoadScene("LobbyScene" + stageNum);
        }

        public void PlayerDie()
        {
            IsFailed = true;
            StartCoroutine(OnGameResultPanel());
        }

        public void GameOver()
        {
            IsGameOver = true;
            StartCoroutine(OnGameResultPanel());
        }

        private IEnumerator OnGameResultPanel()
        {
            WaitForSeconds delay = new WaitForSeconds(2f);

            GameEndEffect.SetTrigger("GameEnd");

            yield return delay;

            if (IsClear)
                clearPanel.SetActive(true);
            else if (IsFailed)
                failedPanel.SetActive(true);
            else if (IsGameOver)
                gameOverPanel.SetActive(true);

            yield return delay;

            //Text 출력


            IsGamePlay = false;

            yield return null;
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