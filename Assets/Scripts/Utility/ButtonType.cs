using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hun.Manager;

namespace Hun.Utility
{
    public enum BtnType
    {
        Continue = 0,
        NewGame,
        Exit,
        Option,
        Restart,
        Lobby
    }

    public class ButtonType : MonoBehaviour
    {
        [SerializeField] private BtnType currentType;

        private void Start()
        {
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClickButton);
        }

        public void OnClickButton()
        {
            switch (currentType)
            {
                case BtnType.Continue:
                    GameManager.Instance.ContinueGame();
                    break;
                case BtnType.NewGame:
                    GameManager.Instance.NewGame();
                    break;
                case BtnType.Exit:
                    GameManager.Instance.ExitGame();
                    break;
                case BtnType.Option:
                    GameManager.Instance.OptionControl();
                    break;
                case BtnType.Restart:
                    GameManager.Instance.RestartGame();
                    break;
                case BtnType.Lobby:
                    GameManager.Instance.GoToLobby();
                    break;
                default:
                    break;
            }
        }
    }
}