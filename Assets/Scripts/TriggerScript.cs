using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    [SerializeField] private SceneController scene;
    [SerializeField] private int id = 0;
    private bool inactive = false;
    private bool waitingForPlayerGrounded = false;
    private bool powersCutsceneStarted = false;

    public void activate()
    {
        Debug.Log("Trigger activated");
        if (!inactive)
        {
            inactive = true;
            switch (id)
            {
                case 1: //Cutscene where player obtains powers
                    waitingForPlayerGrounded = true;
                    scene.currentCutscene = 3;
                    scene.realPlayer.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    scene.cutsceneHandler.changeCamera(new Vector2(scene.realPlayer.transform.position.x, scene.realPlayer.transform.position.y), 3f);
                    break;
                case 2: //Trigger for gaining full control over swapping
                    scene.canSwap = true;
                    scene.swapText.gameObject.SetActive(true);
                    break;
                case 3: //End Cinematic
                    break;
                default:
                    break;
            }
        }
    }
    private void Update()
    {
        if (waitingForPlayerGrounded && scene.realPlayer.IsGrounded())
        {
            Debug.Log("Ready to start powers cutscene");
            waitingForPlayerGrounded = false;
            powersCutsceneStarted = true;
            scene.realPlayer.animator.SetBool("isJumping", false);
            scene.realPlayer.animator.SetFloat("xVelocity", 0);
            scene.realPlayer.animator.SetFloat("yVelocity", 0);

            scene.currentCutscene = 2;
            //scene.cutsceneHandler.changeCamera(new Vector2(213, -4), 3f);
            List<Vector2> destinations = new List<Vector2>() { Vector2.zero, Vector2.zero }; //[new Vector2(5, 5), new Vector2(12, 5), new Vector2(12, 15)]
            List<float> timers = new List<float>() { 2, 2.5f};
            List<float> zooms = new List<float>() { 0, 1 };
            List<float> moveSpeed = new List<float>() { 0, 0 };
            List<float> zoomSpeed = new List<float>() { 0, 0.3f};
            scene.cutsceneHandler.startCutscene(timers, destinations, zooms, moveSpeed, zoomSpeed);
        }
        if (powersCutsceneStarted && scene.currentCutscene == 0)
        {
            powersCutsceneStarted = false;
            scene.currentCutscene = 1;
            scene.canSwap = true;
            scene.shiftTutorialPrompt.SetActive(true);
        }
    }
}
