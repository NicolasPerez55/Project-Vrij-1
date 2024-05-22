using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraffitiScript : MonoBehaviour
{
    public SceneController scene;

    public int type = 0; //Currently not used, was used in my original demo for the graffiti types
    public bool defaultSpawn = false;

    private void Awake()
    {
        scene = FindFirstObjectByType<SceneController>();
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8) //'graffiti player' layer
        {
            if (type == 3) scene.hitFire(); //Forces player into real mode
            else if (type == 2) //Warps the player to the next object designated a "warp"
            {
                scene.warpPlayer(gameObject);
            }
            else if (type == 4) //Beats the game
            {
                scene.gameWon = true;
                scene.pauseGame();
            }
        }
    }
}
