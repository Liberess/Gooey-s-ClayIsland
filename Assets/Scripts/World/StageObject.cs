using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hun.Obstacle
{
    public class StageObject : MonoBehaviour
    {
        [SerializeField] private string stageName;
        [SerializeField] private int stageNum;
        [SerializeField] private int requirementShineLampNum;

        [Header("== Material ==")]
        [SerializeField] private Material lockMat;
        [SerializeField] private Material OpenMat;

        [Header("== Canvas ==")]
        [SerializeField] private GameObject stageInfoUI;
        [SerializeField] private TextMeshProUGUI stageInfoTxt;

        [SerializeField] private Transform cameraTr;

        private Renderer objRenderer;

        private bool isOpen = false;

        private void Awake()
        {
            objRenderer = GetComponentInChildren<Renderer>();
            stageInfoUI = GetComponentInChildren<Canvas>().gameObject;
            stageInfoTxt = stageInfoUI.GetComponentInChildren<TextMeshProUGUI>();
            cameraTr = GameObject.Find("Camera Group").transform.GetChild(1).transform;
        }

        private void Start()
        {
            stageInfoUI.SetActive(false);
            stageInfoTxt.text = "stage" + stageNum.ToString();

            CheckShineLamp();

            ////시작 시 이벤트를 등록해 줍니다.
            //SceneManager.sceneLoaded += LoadedsceneEvent;
        }

        private void Update()
        {
            if(stageInfoUI.activeSelf)
                stageInfoUI.transform.LookAt(cameraTr);

            if(Input.GetKeyDown("space") && isOpen && stageInfoUI.activeSelf)
                Manager.GameManager.Instance.LoadScene(stageName);
        }

        //private void LoadedsceneEvent(Scene scene, LoadSceneMode mode)
        //{
        //    CheckShineLamp();
        //}

        public void CheckShineLamp()
        {
            int value = Manager.DataManager.Instance.GameData.gameSaveFiles[0].prismPiece;

            if (requirementShineLampNum <= value)
            {
                objRenderer.material = OpenMat;
                isOpen = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                stageInfoUI.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                stageInfoUI.SetActive(false);
            }
        }
    }
}