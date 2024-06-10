using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneHandler : MonoBehaviour
{
    [SerializeField] private GameObject cameraPoint;
    [SerializeField] private SceneController scene;

    public float cutsceneTimer = 0;

    public void startCutscene()
    {

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
            case 2: //Cutscene of door #1
                break;
            case 3: //Cutscene of lift #1
                break;
            case 4: //Cutscene of lift #2
                break;
            default:
                break;
        }
    }

    public void changeCamera(Vector2 cameraPos, float cameraSize)
    {
        cameraPoint.transform.position = cameraPos;
        cameraPoint.GetComponentInChildren<Camera>().orthographicSize = cameraSize;
    }
}
