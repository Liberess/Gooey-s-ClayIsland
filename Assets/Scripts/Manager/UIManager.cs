using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

namespace Hun.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("== UI ==")]
        [SerializeField] private GameObject selectStagePanel;
        [SerializeField] private TextMeshProUGUI lifeTxt;
        //[SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI clearObjCountTxt;
        [SerializeField] private TextMeshProUGUI stageTimerTxt;
        [SerializeField] private Image[] healthImgs = new Image[3];
        [SerializeField] private Image mouthfulImg;
        [SerializeField] private Sprite[] mouthfulSprites;

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

            var parent = parentCanvas.Find("HelathGrid");
            for (int i = 0; i < parent.childCount; i++)
                healthImgs[i] = parent.GetChild(i).GetChild(0).GetComponent<Image>();

            mouthfulImg = parentCanvas.Find("MouthfulImg").GetComponent<Image>();

            //coinTxt = parentCanvas.Find("CoinImg").GetChild(0).GetComponent<TextMeshProUGUI>();
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
            //coinTxt.text = "x" + value;
        }

        public void SetHeartUI(int value)
        {
            //heartTxt.text = "x" + value;

            if(value < healthImgs.Length)
                StartCoroutine(DecreaseHeartUICo(value));
 
/*            if(value <= 0)
            {
                for (int i = 0; i < healthImgs.Length; i++)
                    healthImgs[i].enabled = false;
            }
            else
            {
                for (int i = 0; i < value; i++)
                    healthImgs[i].enabled = true;

                if (value < healthImgs.Length)
                    healthImgs[value].enabled = false;
            }*/
        }

        private IEnumerator DecreaseHeartUICo(int index)
        {
            var targetImg = healthImgs[index];

            float time = 0f;

            while(true)
            {
                if (targetImg.fillAmount <= 0) break;

                time += Time.deltaTime;
                targetImg.fillAmount = Mathf.Lerp(1, 0, time);
                //targetImg.fillAmount -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield return null;
        }

        public void SetLifeUI(int value)
        {
            lifeTxt.text = value.ToString();
        }

        public void SetStageTimerUI(int value)
        {
            stageTimerTxt.text = value.ToString();
        }

        public void SetSelectStageUI(bool value)
        {
            selectStagePanel.SetActive(value);
        }

        public void SetMouthfulUI(ClayBlockType type = ClayBlockType.Empty)
        {
            if (type != ClayBlockType.Empty)
                mouthfulImg.GetComponent<Animator>().SetTrigger("doBounce");
            else
                mouthfulImg.GetComponent<Animator>().SetTrigger("doNull");

            switch(type)
            {
                default: mouthfulImg.sprite = mouthfulSprites[0]; break;
                case ClayBlockType.Grass: mouthfulImg.sprite = mouthfulSprites[1]; break;
                case ClayBlockType.Ice: mouthfulImg.sprite = mouthfulSprites[2]; break;
                case ClayBlockType.Sand: mouthfulImg.sprite = mouthfulSprites[3]; break;
                case ClayBlockType.ShineLamp: mouthfulImg.sprite = mouthfulSprites[4]; break;
                case ClayBlockType.Toolbox: mouthfulImg.sprite = mouthfulSprites[5]; break;
            }
        }
    }
}