using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupScript : MonoBehaviour
{
    public SceneController scene;
    public SpriteRenderer renderer;
    public BoxCollider2D collider;

    public bool disabled = false;

    public void collided()
    {
        Debug.Log("paintcan triggered");
        if (disabled == false)
        {
            collider.isTrigger = true;
            renderer.enabled = false;
            disabled = true;
            scene.getPaintcan();
        }
    }
}
