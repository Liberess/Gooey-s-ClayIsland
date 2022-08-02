using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Item
{
    public class PrismPiece : MonoBehaviour, IItem
    {
        public void OnEnter()
        {
            StartCoroutine(OnEnterCo());
        }

        public void OnExit()
        {

        }

        private IEnumerator OnEnterCo()
        {
            GetComponentInChildren<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;

            yield return new WaitForSeconds(1f);

            UseItem();
        }

        public void UseItem()
        {
            Manager.GameManager.Instance.StageClear();
            Destroy(gameObject);
        }
    }
}