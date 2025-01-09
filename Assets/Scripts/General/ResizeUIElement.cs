using UnityEngine;

public class ResizeUIElement : MonoBehaviour
{
    private Canvas canvas;
    public Vector2 referenceResolution = new Vector2(500, 800); // 基準解像度
    public float matchWidthOrHeight = 0; // 0は幅、1は高さに基づくスケール

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        //UpdateCanvasScale();
    }

    private void Update()
    {
        //UpdateCanvasScale();
    }

    private void UpdateCanvasScale()
    {
        float targetAspectRatio = referenceResolution.x / referenceResolution.y;
        float screenAspectRatio = (float)Screen.width / Screen.height;

        float scaleFactor = 1.0f;
        if (screenAspectRatio >= targetAspectRatio)
        {
            // 画面が基準よりもワイド
            scaleFactor = Screen.height / referenceResolution.y;
        }
        else
        {
            // 画面が基準よりも高い
            scaleFactor = Screen.width / referenceResolution.x;
        }

        // マッチング設定に基づいてスケールファクターを調整
        scaleFactor = Mathf.Lerp(Screen.width / referenceResolution.x, Screen.height / referenceResolution.y, matchWidthOrHeight);

        canvas.scaleFactor = scaleFactor;
        canvas.pixelPerfect = true;
    }
}