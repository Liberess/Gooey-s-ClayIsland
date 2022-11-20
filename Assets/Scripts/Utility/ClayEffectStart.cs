using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayEffectStart : MonoBehaviour
{
    [SerializeField] GameObject clayEffect;
    // Start is called before the first frame update
    void Start()
    {
        clayEffect.SetActive(true);
        //Invoke("effectOut", 2f);
    }

    // Update is called once per frame
    private void effectOut()
    {
        clayEffect.SetActive(false);
    }
}
