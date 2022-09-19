using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionType { Forward, Back, Left, Right }

[System.Serializable]
public class DirectionVector
{
    public Vector3[] dirVectors;
    public Vector3[] currentVectors = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
    public Vector3[] defaultVectors = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

    public Vector3 GetVector(DirectionType _dirType) => dirVectors[(int)_dirType];

    public void SetVectors(Vector3[] _vectors) => dirVectors = _vectors;
}