using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawECGScript : MonoBehaviour
{
    public int width = 50; // Width of the texture
    public int height = 10; // Height of the texture
    public float speed = 1f; // Speed at which the wave moves

    public float pWaveMultiplier;
    public float qWaveMultiplier;
    public float rWaveMultiplier;
    public float sWaveMultiplier;
    public float tWaveMultiplier;

    [SerializeField] private int lineThickness = 3;

    [Header("Update Control")]
    [SerializeField] private int framesPerUpdate = 10; // 1 = every frame, 10 = every 10 frames
    [SerializeField] private Color _backgroundColor = new Color(0f, 0f, 0f, 0.3f);

    public RawImage ecgDisplay; // Reference to the RawImage component

    private Texture2D texture;
    private float time;
    private int currentX;
    private Renderer rend;

    private int frameCounter;
    private float accumulatedDeltaTime;

    public Color textureColor = Color.green;

    void Awake()
    {
        // Initialize the texture
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // Assign the texture to the RawImage component
        ecgDisplay.texture = texture;

        // Clear the texture initially
        ClearTexture();

        // Draw new ECG data

        rend = GetComponent<Renderer>();

        framesPerUpdate = Mathf.Max(1, framesPerUpdate);
    }

    public void InitialiseDisplay()
    {
        DrawECGOnce();
    }

    void Update()
    {
        frameCounter++;
        accumulatedDeltaTime += Time.deltaTime;

        if (frameCounter < framesPerUpdate)
            return;

        int stepsToRun = frameCounter;
        float stepDeltaTime = accumulatedDeltaTime / stepsToRun;

        frameCounter = 0;
        accumulatedDeltaTime = 0f;

        for (int i = 0; i < stepsToRun; i++)
        {
            // Advance time
            time += stepDeltaTime * speed;

            // Draw new ECG data
            DrawECGStep();
        }

        // Apply the changes to the texture
        texture.Apply();
    }

    void ClearTexture()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, _backgroundColor);
            }
        }
        texture.Apply();
    }

    void DrawThickPixel(int x, int y, Color color)
    {
        int halfThickness = lineThickness / 2;

        for (int offsetX = -halfThickness; offsetX <= halfThickness; offsetX++)
        {
            for (int offsetY = -halfThickness; offsetY <= halfThickness; offsetY++)
            {
                int drawX = x + offsetX;
                int drawY = y + offsetY;

                if (drawX >= 0 && drawX < width && drawY >= 0 && drawY < height)
                {
                    texture.SetPixel(drawX, drawY, color);
                }
            }
        }
    }

    void DrawECGStep()
    {
        // Generate a new ECG value
        float t = time % 1f; // Simulate one heartbeat cycle per second
        float yValue = SimulateECG(t);
        int y = Mathf.FloorToInt((yValue + 1f) * 0.5f * (height - 1));
        y = Mathf.Clamp(y, 0, height - 1);

        // Draw the ECG value at the current position
        if (y >= 0 && y < height)
        {
            // texture.SetPixel(currentX, y, textureColor); // Set the ECG pixel
            DrawThickPixel(currentX, y, textureColor);
        }

        // Clear the previous column before moving on to the next
        ClearPreviousColumn();

        // Move to the next x position
        currentX = (currentX + 1) % width;
    }

    void DrawECGOnce()
    {
        int previousY = -1;

        for (int x = 0; x < width; x++)
        {
            float t = (float)x / (width - 1);   // 0 to 1 across the width
            float yValue = SimulateECG(t);

            int y = Mathf.FloorToInt((yValue + 1f) * 0.5f * (height - 1));
            y = Mathf.Clamp(y, 0, height - 1);

            //texture.SetPixel(x, y, textureColor);
            DrawThickPixel(x, y, textureColor);

            // Optional: join gaps between points so it looks like a line
            if (previousY != -1)
            {
                DrawVerticalLine(x, previousY, y, textureColor);
            }

            previousY = y;
        }

        texture.Apply();
    }

    void DrawVerticalLine(int x, int y1, int y2, Color color)
    {
        int minY = Mathf.Min(y1, y2);
        int maxY = Mathf.Max(y1, y2);

        for (int y = minY; y <= maxY; y++)
        {
            //texture.SetPixel(x, y, color);
            DrawThickPixel(x, y, color);
        }
    }

    void ClearPreviousColumn()
    {
        int halfThickness = lineThickness / 2;
        int clearXCenter = (currentX + 1) % width;

        for (int offsetX = -halfThickness; offsetX <= halfThickness; offsetX++)
        {
            int clearX = clearXCenter + offsetX;

            while (clearX < 0)
                clearX += width;

            clearX %= width;

            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(clearX, y, _backgroundColor);
            }
        }
    }

    float SimulateECG(float t)
    {
        // P Wave: small upward wave
        float pWave = pWaveMultiplier * Mathf.Sin(2 * Mathf.PI * (t - 0.2f) * 10) * Mathf.Exp(-((t - 0.2f) * 30) * ((t - 0.2f) * 30));

        // QRS Complex: sharp peak and trough
        float qWave = qWaveMultiplier * Mathf.Exp(-((t - 0.35f) * 50) * ((t - 0.35f) * 50));
        float rWave = rWaveMultiplier * Mathf.Exp(-((t - 0.4f) * 100) * ((t - 0.4f) * 100));
        float sWave = sWaveMultiplier * Mathf.Exp(-((t - 0.45f) * 50) * ((t - 0.45f) * 50));

        // T Wave: smaller, longer upward wave
        float tWave = tWaveMultiplier * Mathf.Sin(2 * Mathf.PI * (t - 0.6f) * 5) * Mathf.Exp(-((t - 0.6f) * 20) * ((t - 0.6f) * 20));

        // Combined ECG signal
        return pWave + qWave + rWave + sWave + tWave;
    }
}