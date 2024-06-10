using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public GameObject prompt;
    public int type = 0; //1 = door, 2 = lift
    public int index = 1; //Each object of a specific type gets a different index to separate them. Linked objects (doors, lifts) get adjacent indexes
    private CutsceneHandler cutsceneHandler;
    private SceneController scene;

    private void Start()
    {
        cutsceneHandler = FindFirstObjectByType<CutsceneHandler>();
        scene = FindFirstObjectByType<SceneController>();
    }

    public void interactedWith()
    {
        switch (type)
        {
            case 1: //door
                doorInteraction();
                break;
            case 2: //lift
                liftInteraction();
                break;
            default:
                break;
        }
    }

    //The empty cases for this and liftInteraction are in case we want the player to be able to go back through doors/lifts they've been through. I don't think the player needs to backtrack once they've gotten to the rooftops
    private void doorInteraction()
    {
        //Get the door after this one
        InteractableObject nextDoor;
        InteractableObject[] candidates = FindObjectsOfType<InteractableObject>();
        foreach (InteractableObject x in candidates)
        {
            if (x.type == 1 && (x.index == index + 1 || index == 2 && x.index == 1))
            {
                nextDoor = x;
            }
        }

        switch (index)
        {
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    }

    private void liftInteraction()
    {
        //Get the lift after this one
        InteractableObject nextLift;
        InteractableObject[] candidates = FindObjectsOfType<InteractableObject>();
        foreach (InteractableObject x in candidates)
        {
            if (x.type == 2 && x.index == index + 1)
            {
                nextLift = x;
            }
        }

        switch (index)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                break;
        }
    }

}
