using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawSinewaveScript : MonoBehaviour
{
    public int width = 50;
    public int height = 10;
    public float frequency = 1f;
    public float amplitude = 0.5f;
    public float speed = 1f;

    public RawImage ecgDisplay;

    [SerializeField] private int lineThickness = 3;
    [SerializeField] private Color lineColor = Color.green;

    [Header("Update Control")]
    [SerializeField] private int framesPerUpdate = 10;   // 1 = every frame, 10 = every 10 frames

    private Texture2D texture;
    private float time;
    private Color[] pixels;
    private int frameCounter;
    private float accumulatedDeltaTime;

    void Start()
    {
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        ecgDisplay.texture = texture;
        pixels = new Color[width * height];

        DrawECG();
    }

    void Update()
    {
        frameCounter++;
        accumulatedDeltaTime += Time.deltaTime;

        if (frameCounter < framesPerUpdate)
            return;

        time += accumulatedDeltaTime * speed;

        frameCounter = 0;
        accumulatedDeltaTime = 0f;

        DrawECG();
    }

    void DrawECG()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black;
        }

        for (int x = 0; x < width; x++)
        {
            float yValue = Mathf.Sin((x + time * 100f) * frequency * Mathf.PI / 180f) * amplitude;
            int y = Mathf.FloorToInt((yValue + 1) * 0.5f * (height - 1));
            y = Mathf.Clamp(y, 0, height - 1);

            DrawThickPixel(x, y, lineColor);
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }

    void DrawThickPixel(int x, int y, Color color)
    {
        int halfThickness = lineThickness / 2;

        for (int offset = -halfThickness; offset <= halfThickness; offset++)
        {
            int drawY = y + offset;

            if (drawY >= 0 && drawY < height)
            {
                pixels[x + drawY * width] = color;
            }
        }
    }
}