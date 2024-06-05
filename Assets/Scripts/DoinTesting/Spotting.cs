using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotting : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] GameObject player;
    public static event Action onPlayerSpotted;
    public int currentSpawnPoint = 0;

    public void teleportPlayer()
    {
        player.transform.position = spawnPoints[currentSpawnPoint].position;
    }
}
