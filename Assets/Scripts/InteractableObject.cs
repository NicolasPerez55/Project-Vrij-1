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
    public bool inactive = false;

    private void Start()
    {
        cutsceneHandler = FindFirstObjectByType<CutsceneHandler>();
        scene = FindFirstObjectByType<SceneController>();
    }

    public void interactedWith()
    {
        if (!inactive)
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
                scene.realPlayer.transform.position = new Vector2(218, -5.088044f);
                scene.realPlayer.gameObject.SetActive(false);
                scene.currentCutscene = 2;
                cutsceneHandler.changeCamera(new Vector2(155.65f, -2.588075f), 3f); //-1.588075 -4.088044
                List<Vector2> destinations = new List<Vector2>() { Vector2.zero, new Vector2(218, -5.088044f), new Vector2(218, -4.088044f) };
                List<float> timers = new List<float>() { 1.5f, 0.5f, 1.5f};
                List<float> zooms = new List<float>() { 0.5f, 0, 3};
                List<float> moveSpeed = new List<float>() { 0, 0, 1};
                List<float> zoomSpeed = new List<float>() { 1.6f, 0, 1.6f};
                cutsceneHandler.startCutscene(timers, destinations, zooms, moveSpeed, zoomSpeed);
                break;
            default:
                break;
        }
    }

    private void liftInteraction()
    {
        List<Vector2> destinations = new List<Vector2>();
        List<float> timers = new List<float>();
        List<float> zooms = new List<float>();
        List<float> moveSpeed = new List<float>();
        List<float> zoomSpeed = new List<float>();

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
                scene.hasUsedFirstLift = true;
                scene.realPlayer.transform.position = new Vector2(213.5f, 80.91189f);
                scene.realPlayer.gameObject.SetActive(false);

                scene.currentCutscene = 2;
                cutsceneHandler.changeCamera(new Vector2(213.5f, -4), 3f);
                destinations = new List<Vector2>() { new Vector2(213.5f, 10), new Vector2(213.5f, 82), Vector2.zero, Vector2.zero }; //[new Vector2(5, 5), new Vector2(12, 5), new Vector2(12, 15)]
                timers = new List<float>() { 2.3f, 12f, 1.5f, 3 };
                zooms = new List<float>() { 0, 9, 0, 3 };
                moveSpeed = new List<float>() { 6, 6, 0, 0 };
                zoomSpeed = new List<float>() { 0, 0.45f, 0, 1.4f };
                cutsceneHandler.startCutscene(timers, destinations, zooms, moveSpeed, zoomSpeed);
                break;
            case 3:
                scene.hasUsedSecondLift = true;
                scene.realPlayer.transform.position = new Vector2(256.07f, 109.9119f);
                scene.realPlayer.gameObject.SetActive(false);

                scene.currentCutscene = 2;
                cutsceneHandler.changeCamera(new Vector2(256.07f, 84.9119f), 4.2f);
                destinations = new List<Vector2>() { new Vector2(256.07f, 110.9119f), Vector2.zero }; //[new Vector2(5, 5), new Vector2(12, 5), new Vector2(12, 15)]
                timers = new List<float>() { 9f, 1.5f};
                zooms = new List<float>() { 7, 0};
                moveSpeed = new List<float>() { 3, 0};
                zoomSpeed = new List<float>() { 0.3f, 0};
                cutsceneHandler.startCutscene(timers, destinations, zooms, moveSpeed, zoomSpeed);
                break;
            default:
                break;
        }
    }

}
