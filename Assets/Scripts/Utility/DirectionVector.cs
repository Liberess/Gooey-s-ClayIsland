using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionType { Forward, Back, Left, Right }

[System.Serializable]
public class DirectionVector
{
    public Vector3[] dirVectors;

    public Vector3 GetVector(DirectionType _dirType) => dirVectors[(int)_dirType];
    public void SetVectors(Vector3[] _vectors) => dirVectors = _vectors;
}