using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hun.Utility;
using MoreMountains.Tools;

namespace Hun.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private DataManager dataMgr;
        private UIManager uiManager;
        private SceneController sceneController;
        private TimelineController timelineController;

        private GameObject player;
        private Hun.Player.PlayerHealth playerHealth;

        public GameSaveFile gameSaveFile;

        public int Coin { get; private set; }
        public float PlayTime { get; private set; }
        public int SceneIndex { get; private set; }
        public string SceneName { get; private set; }
        public bool IsGamePlay { get; private set; }

        public bool IsClear { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsFailed { get; private set; }

        [Space(10), Header("== Game Menu UI =="), Space(5)]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject quitPanel;
        [SerializeField] private GameObject pausePanel;

        [Space(10), Header("== Game Result UI =="), Space(5)]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private GameObject playerObjInPanel;
        [SerializeField] private Animator playerObjInPanelAnim;
        [SerializeField] private GameObject[] resultTxt; // 0 : clear, 1 : failed, 2 : game over
        [SerializeField] private GameObject ButtonTxt;

        [Space(10), Header("== Clay Block Object Prefabs =="), Space(5)]
        [SerializeField] private List<GameObject> clayBlockTilePrefabList = new List<GameObject>();
        public GameObject GetClayBlockTilePrefab(ClayBlockType clayBlockType)
            => clayBlockTilePrefabList[(int)clayBlockType];
        [SerializeField] private List<GameObject> temperObjPrefabList = new List<GameObject>();
        public GameObject GetTemperPrefab(TemperObjectType type)
            => temperObjPrefabList[(int)type];

        //월드 관련 변수
        [Space(10), Header("== Game Stage =="), Space(5)]
        [SerializeField] private float stageTimer;

        private float waitingTime = 1f;
        private float curTime;
        private bool isEndTimer = false;
        public float CurTime { get => curTime; }
        public List<ClayBlockTile> IceBlockList { get; private set; } = new List<ClayBlockTile>();

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
            sceneController = FindObjectOfType<SceneController>();
            timelineController = FindObjectOfType<TimelineController>();

            SceneName = SceneManager.GetActiveScene().name;
            SceneIndex = SceneManager.GetActiveScene().buildIndex;

            SetupIceBlockList();
            
            if (dataMgr.GameData.gameState != GameState.Stage)
                return;

            player = GameObject.FindWithTag("Player");
            if(player)
            {
                playerHealth = player.GetComponent<Hun.Player.PlayerHealth>();
            }

            if (resultPanel)
            {
                playerObjInPanel = resultPanel.transform.GetChild(0).gameObject;
                resultTxt = new GameObject[3];
                resultPanel.SetActive(false);

                for (int i = 0; i < resultTxt.Length; i++)
                {
                    resultTxt[i] = resultPanel.transform.GetChild(1).GetChild(i).gameObject;
                }

                ButtonTxt = resultPanel.transform.GetChild(2).gameObject;
            }

            if(playerObjInPanel)
                playerObjInPanelAnim = playerObjInPanel.GetComponent<Animator>();

            if (mainPanel != null)
                mainPanel.SetActive(true);

            if (resultPanel != null)
                resultPanel.SetActive(false);

            Coin = 0;

            IsClear = false;
            IsFailed = false;
            IsGameOver = false;

            curTime = stageTimer + waitingTime;
            uiManager.SetStageTimerUI((int)(curTime - 1));

            isEndTimer = false;
        }

        private void Update()
        {
            if (dataMgr.GameData.gameState == GameState.Main)
                return;

            if(dataMgr.GameData.gameState == GameState.Lobby)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    //LobbyControl();
                    OptionControl();
                }
            }
            else
            {
                CountTimer();

                // Option Panel Control
                if (IsGamePlay && Input.GetKeyDown(KeyCode.Escape))
                {
                    OptionControl();
                }
            }

            if (!IsGamePlay)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (IsClear)
                    {
                        GoToLobby();
                    }
                    else if (IsFailed)
                    {
                        LoadScene(SceneManager.GetActiveScene().name);
                    }
                    else if (IsGameOver)
                    {
                        GoToLobby();
                    }
                }
            }
        }

        private void SetupIceBlockList()
        {
            var blocks = FindObjectsOfType<ClayBlockTile>();
            IceBlockList.Clear();
            IceBlockList = blocks.ToList().FindAll(e => e.ClayBlockType == ClayBlockType.Ice);
        }

        public ClayBlockTile InstantiateClayBlockTile(ClayBlockType clayBlockType)
        {
            ClayBlockTile tile = Instantiate(clayBlockTilePrefabList[(int)clayBlockType]).GetComponent<ClayBlockTile>();
            return tile;
        }

        public void GetCountTime(float time) => curTime += time;

        private void CountTimer()
        {
            if (IsClear || isEndTimer)
                return;

            if (timelineController.PlayableDirector.state == UnityEngine.Playables.PlayState.Playing)
                return;

            uiManager.SetStageTimerUI((int)(curTime - 1));

            curTime -= Time.deltaTime;
            PlayTime += Time.deltaTime;

            if (curTime <= 0)
            {
                isEndTimer = true;
                Entity.DamageMessage dmgMsg = new Entity.DamageMessage();
                dmgMsg.damager = gameObject;
                dmgMsg.dmgAmount = 3;
                //dmgMsg.hitNormal = transform.position;
                //dmgMsg.hitPoint = transform.position;
                GameOver();
            }
        }

        /// <summary>
        /// �ɼ�â�� ��Ʈ���ϴ� �Լ��̴�.
        /// </summary>
        public void OptionControl()
        {
            if(pausePanel == null)
            {
                var parent = GameObject.Find("==== UI ====").transform.Find("GameCanvas");
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

        public void LobbyControl()
        {
            Debug.Log("LobbyControl");
            if (pausePanel == null)
            {
                var parent = GameObject.Find("==== UI ====").transform.Find("PauseCanvas");
                pausePanel = parent.Find("PausePanel").gameObject;
            }

            if (pausePanel.activeSelf)
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

        public void LoadScene(string sceneName)
        {
            /*
            if (sceneName.Contains("Stage"))
                dataMgr.GameData.gameState = GameState.Stage;
            else if(sceneName.Contains("Lobby"))
                dataMgr.GameData.gameState = GameState.Lobby;
            */

            Time.timeScale = 1f;
            IsGamePlay = true;
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

        /// <summary>
        /// 게임이 종료되었을 때 결과에 따른 화면창 출력
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnGameResultPanel()
        {
            WaitForSeconds delay = new WaitForSeconds(2f);

            //init
            playerObjInPanel.SetActive(false);
            ButtonTxt.SetActive(false);
            foreach (GameObject txt in resultTxt)
            {
                txt.SetActive(false);
            }

            mainPanel.SetActive(false);
            VFXManager.Instance.ClayFadeOut();
            //GameEndEffect.SetTrigger("GameEnd");

            yield return delay;

            //플레이어 모델 등장
            resultPanel.SetActive(true);
            playerObjInPanel.SetActive(true);

            if (IsClear)
            {
                playerObjInPanelAnim.SetTrigger("Clear");

                var data = dataMgr.GetStageSaveFile(dataMgr.GameData.currentStageIndex);
                data.isSaved = true;
                if (data.bestRecord >= PlayTime)
                    data.bestRecord = PlayTime;
                data.totalPlayTime += PlayTime;
                
                dataMgr.SaveGameData();

                yield return delay;
                resultTxt[0].SetActive(true);
            }
            else if (IsFailed)
            {
                playerObjInPanelAnim.SetTrigger("Failed");
                yield return delay;
                resultTxt[1].SetActive(true);
            }
            else if (IsGameOver)
            {
                playerObjInPanelAnim.SetTrigger("GameOver");
                yield return delay;
                resultTxt[2].SetActive(true);
            }

            yield return delay;

            //Text 출력
            ButtonTxt.SetActive(true);

            IsGamePlay = false;

            yield return null;
        }

        #region Game Load & Quit

        public void StartGame(GameSaveFile saveFileNum)
        {
            gameSaveFile = saveFileNum;

            if (dataMgr.GameData.stageSaveFiles[(int)gameSaveFile].isSaved == false)
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
            LoadScene("LobbyScene1");
        }

        /// <summary>
        /// ���� ������ �̾��ϴ� �Լ��̴�.
        /// </summary>
        public void ContinueGame()
        {
            //dataMgr.GameData.gameState = GameState.Lobby;

            OptionControl();
            //LoadScene("LobbyScene");
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            IsGamePlay = true;
            LoadingManager.LoadScene(SceneName);
        }

        public void GoToLobby()
        {
            string sceneName = "LobbyDeco";

            Time.timeScale = 1f;
            IsGamePlay = false;
            dataMgr.GameData.gameState = GameState.Lobby;
            AudioManager.Instance.StopBGM();
            LoadingManager.LoadScene(sceneName);
        }
       
        /// <summary>
        /// ������ �����Ű�� �Լ��̴�.
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}