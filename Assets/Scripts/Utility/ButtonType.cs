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
        Quit,
        Option
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
            Debug.Log("OnClickButton");

            switch (currentType)
            {
                case BtnType.Continue:
                    GameManager.Instance.ContinueGame();
                    break;
                case BtnType.NewGame:
                    GameManager.Instance.NewGame();
                    break;
                case BtnType.Quit:
                    GameManager.Instance.QuitGame();
                    break;
                case BtnType.Option:
                    GameManager.Instance.OptionControl();
                    break;
                default:
                    break;
            }
        }
    }
}