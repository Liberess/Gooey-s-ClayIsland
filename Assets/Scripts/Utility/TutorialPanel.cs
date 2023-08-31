using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hun
{
    public class TutorialPanel : MonoBehaviour
    {
        [SerializeField] int pageNum = 4;
        [SerializeField] Image[] pages;

        [SerializeField] GameObject player;
        [SerializeField] GameObject DummyPlayer;

        int curPage = 0;

        private void Awake()
        {
            if(pages == null)
                pages = GetComponentsInChildren<Image>();

            if(player == null)
                player = GameObject.FindWithTag("Player");

            if (DummyPlayer == null)
                DummyPlayer = GameObject.Find("DummyPlayer");
        }

        void Start()
        {
            if (Manager.DataManager.Instance.GameData.isNewGame)
            {
                player.SetActive(false);
                DummyPlayer.SetActive(true);

                TurnTutorialPage(0);
            }
            else
            {
                player.SetActive(true);
                DummyPlayer.SetActive(false);

                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                TurnTutorialPageNext();
            }
        }

        public void TurnTutorialPageNext()
        {
            if (curPage >= pageNum)
            {
                TurnOffTutorialPage();
                return;
            }

            foreach (Image image in pages)
            {
                image.gameObject.SetActive(false);
            }

            pages[curPage].gameObject.SetActive(true);
            curPage += 1;
        }

        public void TurnTutorialPage(int page)
        {
            if(page >= pageNum)
            {
                TurnOffTutorialPage();
                return;
            }

            foreach(Image image in pages)
            {
                image.gameObject.SetActive(false);
            }

            pages[page].gameObject.SetActive(true);
            curPage = page;
        }

        private void TurnOffTutorialPage()
        {
            player.SetActive(true);
            DummyPlayer.SetActive(false);

            Manager.DataManager.Instance.GameData.isNewGame = false;

            gameObject.SetActive(false);
        }
    }
}