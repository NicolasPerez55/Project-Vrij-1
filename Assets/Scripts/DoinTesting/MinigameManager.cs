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
        scene.cutsceneHandler.changeCamera(MinigameObj.transform.position, 2.5f);
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
        scene.puzzleHandler.couplePuzzleDone();
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
