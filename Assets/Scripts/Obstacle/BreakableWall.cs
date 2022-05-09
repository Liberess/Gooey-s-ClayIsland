using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Hun.Obstacle
{
    public class BreakableWall : MonoBehaviour
    {
        [SerializeField, Range(0, 10)] private int breakCount = 3;
        private int currentBreakCount = 0;

        private TextMeshProUGUI countTxt;

        private void Awake()
        {
            countTxt = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            currentBreakCount = 0;

            countTxt.text = "클릭 : " + (breakCount - currentBreakCount).ToString();
        }

        public void InteractWall()
        {
            ++currentBreakCount;

            countTxt.text = "클릭 : " + (breakCount - currentBreakCount).ToString();

            if (currentBreakCount >= breakCount)
                DestroyWall();
        }

        private void DestroyWall()
        {
            Destroy(gameObject);
        }
    }
}