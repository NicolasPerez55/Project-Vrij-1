using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] GameObject paintDoneObj, MinigameObj;

    public static MinigameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            // You could also log a warning.
        }
    }

    public void StartMinigameOne()
    {
        MinigameObj.SetActive(true);
    }

    public void PaintingComplete()
    {
        paintDoneObj.SetActive(false);
        MinigameObj.SetActive(false);
    }
}
