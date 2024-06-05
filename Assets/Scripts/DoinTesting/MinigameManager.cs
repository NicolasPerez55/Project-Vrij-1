using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] GameObject paintDoneObj, MinigameObj;
    [SerializeField] SceneController scene;
    public List<GameObject> lines = new List<GameObject>();

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
        foreach (GameObject go in lines)
        {
            Destroy(go);
        }
        paintDoneObj.SetActive(false);
        MinigameObj.SetActive(false);
        scene.couplePuzzleDone();
    }
}
