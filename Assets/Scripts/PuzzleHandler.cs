using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleHandler : MonoBehaviour
{
    //This class handles the puzzles on the level, which are solved and the effects of solving them
    [SerializeField] SceneController scene;

    public GameObject nextGraffitiSpot; //CHANGE THIS VARIABLE BASED ON PLAYER'S PROGRESS THROUGH THE LEVEL
    public int gameProgressStage = 0; //0 = no minigames, 1 = tree done, 2 = eye done, 3 = couple done, 4 = cat done

    //Couple stuff
    public GameObject couple;
    public Sprite happyCouple;
    public GameObject brokenHeart;
    public GameObject fullHeart;
    public MinigameManager coupleManager;
    public GameObject drawingMinigameCouple;
    public bool coupleMinigameCompleted = false;

    //cat stuff
    [SerializeField] private GameObject potato;
    [SerializeField] private GameObject unfilledCat;
    [SerializeField] private GameObject filledCat;
    [SerializeField] private MinigameManager catManager;
    [SerializeField] private GameObject drawingMinigameCat;
    [SerializeField] private bool catMinigameCompleted;

    public void startMinigame()
    {
        switch (gameProgressStage)
        {
            case 0: //Tree puzzle
                break;
            case 1: //Eye puzzle
                break;
            case 2: //Couple puzzle
                coupleManager.StartMinigameOne();
                //scene.cutsceneHandler.changeCamera(new Vector2(drawingMinigameCouple.transform.position.x, drawingMinigameCouple.transform.position.y + scene.cameraOffset), 5f);
                break;
            case 3: //Cat puzzle
                break;
            default:
                break;
        }
    }

    public void treePuzzleDone()
    {
        gameProgressStage += 1;
    }

    public void eyePuzzleDone()
    {
        gameProgressStage += 1;
    }

    public void couplePuzzleDone()
    {
        fullHeart.SetActive(true);
        couple.GetComponent<SpriteRenderer>().sprite = happyCouple;
        couple.GetComponent<BoxCollider2D>().isTrigger = true;
        nextGraffitiSpot.SetActive(false);
        scene.playerInMinigame = false;
        scene.cutsceneHandler.changeCamera(scene.realPlayer.transform.position, 3f);
        scene.currentCutscene = 0;
        coupleMinigameCompleted = true;
        //graffitiTutorialNote.SetActive(false);
        gameProgressStage += 1;
    }

    public void catPuzzleDone()
    {
        gameProgressStage += 1;
    }
}