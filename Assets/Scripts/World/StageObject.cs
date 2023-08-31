using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Hun.Manager;

namespace Hun.Obstacle
{
    public class StageObject : MonoBehaviour
    {
        [SerializeField] private string stageSceneName;
        [SerializeField] private string stageName;
        [SerializeField] private int stageNum;

        //[Header("== Material ==")]
        //[SerializeField] private Material lockMat;
        //[SerializeField] private Material OpenMat;

        [Header("== Canvas ==")]
        [SerializeField] private GameObject stageInfoUI;
        [SerializeField] private TextMeshProUGUI stageNameTxt;
        [SerializeField] private TextMeshProUGUI stageNumTxt;
        [SerializeField] private TextMeshProUGUI stageClearSecTxt;

        [SerializeField] private Transform cameraTr;

        //private Renderer objRenderer;

        private bool isPlayVFX = false;

        private void Awake()
        {
            //objRenderer = GetComponentInChildren<Renderer>();
            cameraTr = FindObjectOfType<Hun.Camera.MainCamera>().transform;
        }

        private void Start()
        {
            stageInfoUI.SetActive(false);
            stageNameTxt.text = stageName;
            stageNumTxt.text = stageNum.ToString();

            var stageData = DataManager.Instance.GetStageSaveFile(stageNum - 1);
            if (stageData.isSaved)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(stageData.bestRecord);
                SetStageBestRecordUI(timeSpan);
            }
            else
            {
                stageClearSecTxt.text = "--:--";
            }
            
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

            if (Input.GetKeyDown("space") && stageInfoUI.activeSelf && !isPlayVFX)
            {
                isPlayVFX = true;
                DataManager.Instance.GameData.currentStageIndex = stageNum - 1;
                VFXManager.Instance.ClayFadeOut();
                StartCoroutine(LoadScene(stageSceneName));
            }
        }

        private IEnumerator LoadScene(string stageName, float delay = 2.0f)
        {
            yield return new WaitForSeconds(delay);
            DataManager.Instance.GameData.gameState = GameState.Stage;
            GameManager.Instance.LoadScene(stageSceneName);
        }

        //private void LoadedsceneEvent(Scene scene, LoadSceneMode mode)
        //{
        //    CheckShineLamp();
        //}

        public void CheckShineLamp()
        {
            int value = Manager.DataManager.Instance.GameData.stageSaveFiles[0].prismPiece;

            //if (requirementShineLampNum <= value)
            //{
            //    //objRenderer.material = OpenMat;
            //    isOpen = true;
            //}
        }

        private void SetStageBestRecordUI(TimeSpan timeStamp)
        {
            /*string dayStr = timeStamp.Days > 0 ? string.Concat(timeStamp.Days, "일") : "";
            string hourStr = timeStamp.Hours > 0 ? string.Concat(timeStamp.Hours, "시간") : "";
            string minutesStr = timeStamp.Minutes > 0 ? string.Concat(timeStamp.Minutes, "분") : "";
            string secondsStr = timeStamp.Seconds >= 0 ? string.Concat(timeStamp.Seconds, "초") : "";

            stageClearSecTxt.text = string.Concat(dayStr, ":", hourStr, ":", minutesStr, ":", secondsStr);*/

            stageClearSecTxt.text = Mathf.RoundToInt((float)timeStamp.TotalSeconds).ToString();
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