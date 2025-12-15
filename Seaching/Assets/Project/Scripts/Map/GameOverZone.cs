using System;
using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    public event Action OnGameOver;
    private bool isGameOver = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;

        if (other.CompareTag("Player"))
        {
            OnGameOver?.Invoke();
        }
    }
}