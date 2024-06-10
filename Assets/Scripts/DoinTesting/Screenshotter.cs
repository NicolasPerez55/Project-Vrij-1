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
            if (pixel.g == 0 && pixel.r == 255 && pixel.b == 0)
            {
                redPixelCount++;
            }
        }

        int whitePixelCount = 0;
        foreach (Color32 pixel in pixels)
        {
            if (pixel.r > 0.99 && pixel.g > 0.99 && pixel.b > 0.99)

            {
                whitePixelCount++;
            }
        }

        float pixelRatio = whitePixelCount / (redPixelCount + 1.0f);
        Debug.Log(pixelRatio);

        if (pixelRatio < 1f)
        {
            if (puzzleID == 1)
            {
                minigameManager.CoupleComplete();
            }
        }
    }
}
