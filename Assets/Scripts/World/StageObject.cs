using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hun.Obstacle
{
    public class StageObject : MonoBehaviour
    {
        [SerializeField] private string stageSceneName;
        [SerializeField] private string stageName;
        [SerializeField] private int stageNum;
        [SerializeField] private int requirementShineLampNum;

        //[Header("== Material ==")]
        //[SerializeField] private Material lockMat;
        //[SerializeField] private Material OpenMat;

        [Header("== Canvas ==")]
        [SerializeField] private GameObject stageInfoUI;
        [SerializeField] private TextMeshProUGUI stageNameTxt;
        [SerializeField] private TextMeshProUGUI stageNumTxt;
        [SerializeField] private TextMeshProUGUI stageClearSecTxt;

        [SerializeField] private Transform cameraTr;

        private Renderer objRenderer;

        private bool isOpen = false;

        private void Awake()
        {
            objRenderer = GetComponentInChildren<Renderer>();
            cameraTr = FindObjectOfType<Hun.Camera.MainCamera>().transform;
        }

        private void Start()
        {
            stageInfoUI.SetActive(false);
            stageNameTxt.text = stageName.ToString();
            stageNumTxt.text = stageNum.ToString();
            stageClearSecTxt.text = "--:--";

            CheckShineLamp();

            ////시작 시 이벤트를 등록해 줍니다.
            //SceneManager.sceneLoaded += LoadedsceneEvent;
        }

        private void Update()
        {
            if (stageInfoUI.activeSelf)
            {
                Vector3 dir = -(cameraTr.position - stageInfoUI.transform.position);
                dir.x = 0f; 

                stageInfoUI.transform.rotation = Quaternion.LookRotation(dir.normalized);
            }

            if (Input.GetKeyDown("space") && isOpen && stageInfoUI.activeSelf)
                Manager.GameManager.Instance.LoadScene(stageSceneName);
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
                //objRenderer.material = OpenMat;
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