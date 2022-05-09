using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObstacle
{
    public void OnEnter();
    public void OnInteract();
    public void OnExit();
}