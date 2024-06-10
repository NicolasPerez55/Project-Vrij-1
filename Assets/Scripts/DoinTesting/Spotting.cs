using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotting : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] GameObject player;
    [SerializeField] PlayerController playerController;
    public int currentSpawnPoint = 0;
    IEnumerator PlayerCaught()
    {
        // Police Drones Fly By
        // Fade to Black
        playerController.speed = 0;
        yield return new WaitForSeconds(1);
        player.transform.position = spawnPoints[currentSpawnPoint].position; // TP's player
        playerController.speed = 5;
        // Screen Fades back in
    }

    public void PlayerSpotted()
    {
        StartCoroutine(PlayerCaught());
    }
}
