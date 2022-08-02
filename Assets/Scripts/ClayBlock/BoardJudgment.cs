using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class BoardJudgment : MonoBehaviour
    {
        private BoardJudgmentObj boardJudgmentObj;

        private void Start()
        {
            boardJudgmentObj = gameObject.transform.parent.GetComponent<BoardJudgmentObj>();
        }

        private void OnTriggerEnter(Collider other)
        {
            boardJudgmentObj.SetTriggerState(false);
        }

        private void OnTriggerExit(Collider other)
        {
            boardJudgmentObj.SetTriggerState(true);
        }
    }
}
