using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CustomScreenshotter : MonoBehaviour
{
    private PlayerController playerController;
    private Texture2D tagTexture;
    [SerializeField] GameObject paintCanvas;
    private SceneController sceneController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        sceneController = FindObjectOfType<SceneController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // MAKE THIS INSTEAD A BUTTON A PLAYER CAN PRESS WHEN THEY'RE DONE DRAWING
        {
            MakeTag();
        }
    }

    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        tagTexture = ScreenCapture.CaptureScreenshotAsTexture();

        Color[] pixels = tagTexture.GetPixels(0, 0, tagTexture.width, tagTexture.height, 0);

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] == Color.white)
            {
                pixels[i] = new Color(0, 0, 0, 0);
            }
        }

        tagTexture.SetPixels(0, 0, tagTexture.width, tagTexture.height, pixels, 0);
        tagTexture.Apply();

        Sprite sprite = Sprite.Create(tagTexture, new Rect(0, 0, tagTexture.width, tagTexture.height), new Vector2(0.5f, 0.5f));
        SpriteRenderer prefabRenderer = playerController.customTagPrefab.GetComponent<SpriteRenderer>();
        prefabRenderer.sprite = sprite;
        sceneController.EndCustomTagCreation();
    }

    public void MakeTag()
    {
        StartCoroutine(RecordFrame());
    }
}
