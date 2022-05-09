using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IObstacle
{
    private Jun.Player.Player player;

    private void Start()
    {
        player = FindObjectOfType<Jun.Player.Player>();
    }

    public void OnEnter()
    {
        player.SetLadderState(true);
    }

    public void OnInteract()
    {

    }

    public void OnExit()
    {
        player.SetLadderState(false);
    }
}