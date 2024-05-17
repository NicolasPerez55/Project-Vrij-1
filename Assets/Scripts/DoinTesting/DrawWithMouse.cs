using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawWithMouse : MonoBehaviour
{
    private LineRenderer line;
    private Vector3 previousPos;
    private bool doneDrawing;

    [SerializeField] GameObject nextLine;
    [SerializeField] float minDistance = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 1;
        previousPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (doneDrawing) return;

        Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPos.z = 0;
        Color currentColor = GetPixelAtMousePos();

        if (Input.GetMouseButton(0) && IsDrawingOnWhite(currentColor))
        {
            if (Vector3.Distance(currentPos, previousPos) > minDistance)
            {

                if (previousPos == transform.position)
                {
                    line.SetPosition(0, currentPos);
                }
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, currentPos);
                previousPos = currentPos;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            // Instantiate(nextLine);
            // doneDrawing = true;
            Screenshotter.Instance.StartScreenshot();
        }
    }

    private Color GetPixelAtMousePos()
    {
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        Vector3 currentMousePos = Input.mousePosition;
        currentMousePos.z = 0;
        Color colorPixel = texture.GetPixel((int)currentMousePos.x, (int)currentMousePos.y);
        // Color colorPixel = texture.GetPixel((int)Input.mousePosition.x, (int)Input.mousePosition.y);
        return colorPixel;
    }

    private bool IsDrawingOnWhite(Color pixelColor)
    {

        //if (pixelColor.r > 0.9 && pixelColor.g > 0.9 && pixelColor.b > 0.9)
        //    return true;
        if (pixelColor == Color.white)
            return true;

        return false;
    }
}
