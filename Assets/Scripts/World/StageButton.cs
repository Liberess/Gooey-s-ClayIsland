using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jun.World.Stage
{
    public class StageButton : MonoBehaviour
    {
        private void Start()
        {
            var btn = GetComponent<UnityEngine.UI.Button>();
            btn.onClick.AddListener(() => Hun.Manager.GameManager.Instance.LoadScene("s001_dinopia_1"));
        }
    }
}