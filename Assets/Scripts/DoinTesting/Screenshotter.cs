using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Screenshotter : MonoBehaviour
{
    [SerializeField] MinigameManager minigameManager;
    [SerializeField] int puzzleID;
    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();

        Color32[] pixels = texture.GetPixels32();
        CheckPixelCount(pixels);
        Destroy(texture);
    }

    public void StartScreenshot()
    {
        StartCoroutine(RecordFrame());
    }

    private void CheckPixelCount(Color32[] pixels)
    {
        int redPixelCount = 0;
        foreach (Color32 pixel in pixels)
        {
            if (pixel.g < 0.1f && pixel.r > 0.9f && pixel.b < 0.1f)
            {
                redPixelCount++;
            }
        }
        Debug.Log("Red Pixels: " + redPixelCount);

        int whitePixelCount = 0;
        foreach (Color32 pixel in pixels)
        {
            if (pixel.r > 250 && pixel.g > 250 && pixel.b > 250)

            {
                whitePixelCount++;
            }
        }
        Debug.Log("White Pixels: " + whitePixelCount);

        float pixelRatio = whitePixelCount / (redPixelCount + 1.0f);
        Debug.Log(pixelRatio);

        if (pixelRatio < 1f)
        {
            switch (puzzleID)
            {
                case 1:
                    minigameManager.TreeComplete();
                    break;
                case 2:
                    minigameManager.EyeComplete();
                    break;
                case 3:
                    minigameManager.CoupleComplete();
                    break;
                case 4:
                    minigameManager.CatComplete();
                    break;
                default:
                    break;
            }
        }
    }
}
