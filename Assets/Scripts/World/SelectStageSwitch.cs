using UnityEngine;
using Hun.Manager;

public class SelectStageSwitch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            UIManager.Instance.SetSelectStageUI(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            UIManager.Instance.SetSelectStageUI(false);
    }
}