using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneHandler : MonoBehaviour
{
    [SerializeField] private GameObject cameraPoint;
    [SerializeField] private SceneController scene;

    public List<float> stageTimers = new List<float>(); //How long each stage of the cutscene lasts
    public List<Vector2> cameraDestinations = new List<Vector2>(); //The points that the camera will move towards, in order
    public List<float> cameraZooms = new List<float>();
    public List<float> cameraMoveSpeed = new List<float>();
    public List<float> cameraZoomSpeed = new List<float>();

    [SerializeField] private int totalStages = 0;
    public float cutsceneTimer = 0;
    [SerializeField] private int currentStage = 0;

    /*
     Stage Timers: How long each stage of the cutscene lasts before moving on to the next
    Camera Destinations = The camera position that the camera will be moving towards at this stage of the cutscene. If Vector2.Zero, the camera will not move and stay where it left off last stage, static
    Camera Zooms: Same thing, but with the zoom. If 0, camera does not zoom in or out.
    Move Speed = The speed at which the camera moves towards its destination during each stage
    Zoom Speed = The speed at which the camera zooms in or out during each stage

    All lists should be the same length. If not, the smallest list will be used to determine how many total stages the cutscene will have
    */
    public void startCutscene(List<float> timers, List<Vector2> destinations, List<float> zooms, List<float> moveSpeed, List<float> zoomSpeed)
    {
        Debug.Log("Cutscene started!");
        stageTimers = timers;
        cameraDestinations = destinations;
        cameraZooms = zooms;
        cameraMoveSpeed = moveSpeed;
        cameraZoomSpeed = zoomSpeed;

        totalStages = setStageCount();
        cutsceneTimer = stageTimers[0];
        currentStage = 0;
    }

    public void updateCutscene(int currentScene)
    {
        switch (currentScene)
        {
            case 0: //Standard gameplay, move camera to player
                if (scene.playerActive == 1)
                {
                    cameraPoint.transform.position = new Vector2(scene.realPlayer.transform.position.x, scene.realPlayer.transform.position.y + scene.cameraOffset);
                }
                else
                {
                    cameraPoint.transform.position = new Vector2(scene.graffitiPlayer.transform.position.x, scene.graffitiPlayer.transform.position.y + scene.cameraOffset);
                }
                break;
            case 1: //Player is in a minigame, don't move the camera
                break;
            case 2: //Camera-Moving Cutscene
                cutsceneTimer -= Time.deltaTime;
                if (cutsceneTimer <= 0)
                {
                    currentStage += 1;
                    if (currentStage >= totalStages) //end of the cutscene reached
                    {
                        scene.currentCutscene = 0;
                        if (scene.playerActive == 1)
                            scene.realPlayer.gameObject.SetActive(true);
                        else
                            scene.graffitiPlayer.gameObject.SetActive(true);
                        Debug.Log("Cutscene ended!");
                        //cameraPoint.GetComponentInChildren<Camera>().orthographicSize = 3f;
                    }
                    else
                    {
                        cutsceneTimer = stageTimers[currentStage];
                    }
                }
                if (scene.currentCutscene != 0)
                {
                    if (cameraDestinations[currentStage] != Vector2.zero)
                    {
                        if (cameraMoveSpeed[currentStage] == 0)
                        {
                            cameraPoint.transform.position = cameraDestinations[currentStage];
                        }
                        else
                            cameraPoint.transform.position = Vector2.MoveTowards(cameraPoint.transform.position, cameraDestinations[currentStage], cameraMoveSpeed[currentStage] * Time.deltaTime);
                    }
                    if (cameraZooms[currentStage] < cameraPoint.GetComponentInChildren<Camera>().orthographicSize && cameraZooms[currentStage] != 0) //size decreases, zooms in
                    {
                        cameraPoint.GetComponentInChildren<Camera>().orthographicSize -= cameraZoomSpeed[currentStage] * Time.deltaTime;
                    }
                    else if (cameraZooms[currentStage] > cameraPoint.GetComponentInChildren<Camera>().orthographicSize && cameraZooms[currentStage] != 0) //size increases, zooms out
                    {
                        cameraPoint.GetComponentInChildren<Camera>().orthographicSize += cameraZoomSpeed[currentStage] * Time.deltaTime;
                    }
                }
                break;
            default:
                break;
        }
    }

    private int setStageCount()
    {
        int result = stageTimers.Count;
        if (cameraDestinations.Count < result)
            result = cameraDestinations.Count;
        if (cameraMoveSpeed.Count < result)
            result = cameraMoveSpeed.Count;
        if (cameraZoomSpeed.Count < result)
            result = cameraZoomSpeed.Count;
        if (cameraZooms.Count < result)
            result = cameraZooms.Count;
        return result;
    }

    public void changeCamera(Vector2 cameraPos, float cameraSize)
    {
        cameraPoint.transform.position = cameraPos;
        cameraPoint.GetComponentInChildren<Camera>().orthographicSize = cameraSize;
    }
}
