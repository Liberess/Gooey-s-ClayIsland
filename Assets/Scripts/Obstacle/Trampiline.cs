using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampiline : MonoBehaviour, IObstacle
{
    private Hun.Player.Player player;

    [SerializeField] private Transform[] poses;

    private void Start()
    {
        player = FindObjectOfType<Hun.Player.Player>();
    }

    public void OnEnter()
    {
        player.SetTrampiline(true);
        OnInteract();
    }

    public void OnExit()
    {

    }

    public void OnInteract()
    {
        player.JumpToPosByTrampiline(poses);
    }
}
