using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawECGScript : MonoBehaviour
{
    public int width = 50; // Width of the texture
    public int height = 10; // Height of the texture
    public float speed = 1f; // Speed at which the wave moves through its cycle

    public float pWaveMultiplier;
    public float qWaveMultiplier;
    public float rWaveMultiplier;
    public float sWaveMultiplier;
    public float tWaveMultiplier;

    [SerializeField] private int lineThickness = 3;

    [Header("Update Control")]
    [SerializeField] private float secondsPerStep = 0.02f; // Time between ECG column updates
    [SerializeField] private int maxStepsPerFrame = 10;    // Prevent too many catch-up steps in one frame

    [Header("Appearance")]
    [SerializeField] private Color _backgroundColor = new Color(0f, 0f, 0f, 0f);

    public RawImage ecgDisplay; // Reference to the RawImage component
    public Color textureColor = Color.green;

    private Texture2D texture;
    private float time;
    private int currentX;
    private Renderer rend;

    private float stepTimer;

    private bool hasPreviousPoint;
    private int previousY;

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

        rend = GetComponent<Renderer>();

        lineThickness = Mathf.Max(1, lineThickness);
        secondsPerStep = Mathf.Max(0.001f, secondsPerStep);
        maxStepsPerFrame = Mathf.Max(1, maxStepsPerFrame);
    }

    public void InitialiseDisplay()
    {
        DrawECGOnce();
    }

    void Update()
    {
        stepTimer += Time.deltaTime;

        int stepsThisFrame = 0;

        while (stepTimer >= secondsPerStep && stepsThisFrame < maxStepsPerFrame)
        {
            stepTimer -= secondsPerStep;

            // Advance time
            time += secondsPerStep * speed;

            // Draw new ECG data
            DrawECGStep();

            stepsThisFrame++;
        }

        // Apply the changes to the texture
        if (stepsThisFrame > 0)
        {
            texture.Apply();
        }
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

    void DrawLine(int x0, int y0, int x1, int y1, Color color)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            DrawThickPixel(x0, y0, color);

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
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

        int drawX = currentX;
        int prevX = (currentX - 1 + width) % width;

        // Draw the ECG value at the current position
        DrawThickPixel(drawX, y, textureColor);

        // Join this point to the previous one so jumps do not appear isolated
        if (hasPreviousPoint)
        {
            DrawLine(prevX, previousY, drawX, y, textureColor);
        }

        previousY = y;
        hasPreviousPoint = true;

        // Clear the previous column before moving on to the next
        ClearPreviousColumn();

        // Move to the next x position
        currentX = (currentX + 1) % width;
    }

    void DrawECGOnce()
    {
        int previousDrawY = -1;

        for (int x = 0; x < width; x++)
        {
            float t = (float)x / (width - 1);   // 0 to 1 across the width
            float yValue = SimulateECG(t);

            int y = Mathf.FloorToInt((yValue + 1f) * 0.5f * (height - 1));
            y = Mathf.Clamp(y, 0, height - 1);

            DrawThickPixel(x, y, textureColor);

            // Optional: join gaps between points so it looks like a line
            if (previousDrawY != -1)
            {
                DrawLine(x - 1, previousDrawY, x, y, textureColor);
            }

            previousDrawY = y;
        }

        texture.Apply();
    }

    void ClearPreviousColumn()
    {
        int halfThickness = lineThickness / 2;
        int clearXCenter = (currentX + 1) % width;

        for (int offsetX = -halfThickness; offsetX <= halfThickness; offsetX++)
        {
            int clearX = clearXCenter + offsetX;

            while (clearX < 0)
            {
                clearX += width;
            }

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