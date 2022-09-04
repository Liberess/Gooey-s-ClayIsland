using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeClayBlockTileMaterial : MonoBehaviour
{
    [SerializeField] private Material[] materials = new Material[9];

    [ContextMenu("Randomize ClayBlock Tile Material")]
    public void Randomize()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            for(int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                int randIndex = Random.Range(0, materials.Length);
                transform.GetChild(i).GetChild(j).GetComponentInChildren<MeshRenderer>().
                    material = materials[randIndex];
            }
        }
    }
}