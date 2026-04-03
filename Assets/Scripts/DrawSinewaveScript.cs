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
    [SerializeField] private float secondsPerUpdate = 0.02f; // Time between redraws
    [SerializeField] private Color _backgroundColor = new Color(0f, 0f, 0f, 0.3f);

    private Texture2D texture;
    private float time;
    private Color[] pixels;
    private float updateTimer;

    void Awake()
    {
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        ecgDisplay.texture = texture;
        pixels = new Color[width * height];

        lineThickness = Mathf.Max(1, lineThickness);
        secondsPerUpdate = Mathf.Max(0.001f, secondsPerUpdate);

        DrawECG();
    }

    void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer < secondsPerUpdate)
            return;

        time += updateTimer * speed;
        updateTimer = 0f;

        DrawECG();
    }

    void DrawECG()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = _backgroundColor;
        }

        for (int x = 0; x < width; x++)
        {
            float yValue = Mathf.Sin((x + time * 100f) * frequency * Mathf.PI / 180f) * amplitude;
            int y = Mathf.FloorToInt((yValue + 1f) * 0.5f * (height - 1));
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