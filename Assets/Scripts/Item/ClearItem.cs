using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Item
{
    public class ClearItem : MonoBehaviour, IItem
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
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;

            yield return new WaitForSeconds(1f);

            UseItem();
        }

        public void UseItem()
        {
            Manager.GameManager.Instance.GetClearObject();
            Destroy(gameObject);
        }
    }
}