using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hun.Manager
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        public enum CursorType { Arrow, Hand }

        [SerializeField] private List<Texture2D> cursorImgList = new List<Texture2D>();

        public UnityAction CursorShowAction;
        public UnityAction CursorHideAction;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            CursorShowAction += CursorShow;
            CursorHideAction += CursorHide;
        }

        private void CursorShow()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void CursorHide()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ChangeCursor(CursorType cursorType)
        {
            if(cursorImgList.Count <= 0)
            {
                //Debug.LogWarning("CursorImgList is Empty!!");
                return;
            }

            if(cursorImgList[(int)cursorType] == null)
            {
                //Debug.LogWarning((int)cursorType + "번째 Cursor 이미지가 List에 존재하지 않습니다.");
                return;
            }   
            
            Cursor.SetCursor(cursorImgList[(int)cursorType], Vector2.zero, CursorMode.ForceSoftware);
        }
    }
}