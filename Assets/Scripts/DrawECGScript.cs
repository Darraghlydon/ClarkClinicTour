using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawECGScript : MonoBehaviour
{
    public int width = 500; // Width of the texture
    public int height = 100; // Height of the texture
    public float speed = 1f; // Speed at which the wave moves

    public float pWaveMultiplier;
    public float qWaveMultiplier;
    public float rWaveMultiplier;
    public float sWaveMultiplier;
    public float tWaveMultiplier;
    
    
    public RawImage ecgDisplay; // Reference to the RawImage component

    private Texture2D texture;
    private float time;
    private int currentX;

    public Color textureColor = Color.green;
    void Start()
    {
        // Initialize the texture
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // Assign the texture to the RawImage component
        ecgDisplay.texture = texture;

        // Clear the texture initially
        ClearTexture();
    }

    void Update()
    {
        // Advance time
        time += Time.deltaTime * speed;

        // Draw new ECG data
        DrawECG();

        // Apply the changes to the texture
        texture.Apply();
    }

    void ClearTexture()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }
        texture.Apply();
    }

    void DrawECG()
    {
        // Generate a new ECG value
        float t = time % 1f; // Simulate one heartbeat cycle per second
        float yValue = SimulateECG(t);
        int y = Mathf.FloorToInt((yValue + 1) * 0.5f * height);

        // Draw the ECG value at the current position
        if (y >= 0 && y < height)
        {
            texture.SetPixel(currentX, y, textureColor); // Set the ECG pixel
        }

        // Clear the previous column before moving on to the next
        ClearPreviousColumn();

        // Move to the next x position
        currentX = (currentX + 1) % width;
    }

    void ClearPreviousColumn()
    {
        int prevX = (currentX + 1) % width;
        for (int y = 0; y < height; y++)
        {
            texture.SetPixel(prevX, y, Color.black);
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
