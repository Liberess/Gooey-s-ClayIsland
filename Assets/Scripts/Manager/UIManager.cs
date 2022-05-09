using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hun.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("== UI ==")]
        [SerializeField] private GameObject selectStagePanel;
        [SerializeField] private TextMeshProUGUI lifeTxt;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI heartTxt;
        [SerializeField] private TextMeshProUGUI clearObjCountTxt;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if(DataManager.Instance.GameData.gameState == GameState.Stage)
                UpdateStageUI();
        }

        [ContextMenu("Auto Setup UI")]
        private void AutoSetupUI()
        {
            var parentCanvas = GameObject.Find("GameCanvas").transform;

            lifeTxt = parentCanvas.Find("LifeImg").GetChild(0).GetComponent<TextMeshProUGUI>();
            coinTxt = parentCanvas.Find("CoinImg").GetChild(0).GetComponent<TextMeshProUGUI>();
            heartTxt = parentCanvas.Find("HeartImg").GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        public void UpdateStageUI()
        {
            SetCoinUI(GameManager.Instance.Coin);
        }

        public void SetClearObjectCountUI(int value)
        {
            clearObjCountTxt.text = "x" + value;
        }

        public void SetCoinUI(int value)
        {
            coinTxt.text = "x" + value;
        }

        public void SetHeartUI(int value)
        {
            heartTxt.text = "x" + value;
        }

        public void SetLifeUI(int value)
        {
            lifeTxt.text = "x" + value;
        }

        public void SetSelectStageUI(bool value)
        {
            selectStagePanel.SetActive(value);
        }
    }
}