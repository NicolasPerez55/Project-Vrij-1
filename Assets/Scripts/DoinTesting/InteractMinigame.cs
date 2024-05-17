using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMinigame : MonoBehaviour
{
    [SerializeField] MinigameManager minigameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        minigameManager.StartMinigameOne();
        Destroy(gameObject);
    }
}
