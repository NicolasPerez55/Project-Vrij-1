using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpawn : MonoBehaviour
{
    [SerializeField] int checkpoint;
    private Spotting spotting;

    void Start()
    {
        spotting = FindObjectOfType<Spotting>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (checkpoint >= spotting.currentSpawnPoint)
        {
            spotting.currentSpawnPoint++;
        }
    }
}
