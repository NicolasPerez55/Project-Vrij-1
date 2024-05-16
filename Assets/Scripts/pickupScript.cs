using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupScript : MonoBehaviour
{
    public SceneController scene;
    public SpriteRenderer renderer;
    public BoxCollider2D collider;

    public bool disabled = false;
    public string pickupType = "Paint Spray";

    public void collided()
    {
        if (disabled == false)
        {
            switch (pickupType)
            {
                case "Paint Spray":
                    {
                        collider.isTrigger = true;
                        renderer.enabled = false;
                        disabled = true;
                        scene.getPaintspray();
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
