using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] GameObject paintDoneObj, MinigameObj;
    [SerializeField] SceneController scene;
    public Screenshotter screenshotter;
    public List<GameObject> lines = new List<GameObject>();

    public void StartMinigameOne()
    {
        MinigameObj.SetActive(true);
    }

    public void CoupleComplete()
    {
        foreach (GameObject go in lines)
        {
            Destroy(go);
        }
        paintDoneObj.SetActive(false);
        MinigameObj.SetActive(false);
        scene.couplePuzzleDone();
    }

    public void TagComplete()
    {
        foreach (GameObject go in lines)
        {
            Destroy(go);
        }
        MinigameObj.SetActive(false);
    }
}
