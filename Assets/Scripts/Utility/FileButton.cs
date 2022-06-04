using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hun.Manager;

namespace Hun.Utility
{
    public class FileButton : MonoBehaviour
    {
        [SerializeField] private GameSaveFile saveFileNum;

        public void OnMouseDown()
        {
            Debug.Log("OnMouseDown");

            GameManager.Instance.StartGame(saveFileNum);
        }
    }
}
