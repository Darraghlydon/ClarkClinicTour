using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawSinewaveScript : MonoBehaviour
{
    public int width = 500; // Width of the texture
    public int height = 100; // Height of the texture
    public float frequency = 1f; // Frequency of the sine wave
    public float amplitude = 0.5f; // Amplitude of the wave
    public float speed = 1f; // Speed at which the wave moves

    public RawImage ecgDisplay; // Reference to the RawImage component

    private Texture2D texture;
    private float time;

    void Start()
    {
        // Initialize the texture
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // Prevents blurring of the texture
        texture.wrapMode = TextureWrapMode.Clamp; // Prevents wrapping of the texture

        // Assign the texture to the RawImage component
        ecgDisplay.texture = texture;
    }

    void Update()
    {
        time += Time.deltaTime * speed;
        DrawECG();
    }

    void DrawECG()
    {
        Color[] pixels = new Color[width * height];

        // Clear the texture to black
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black;
        }

        // Draw the ECG waveform
        for (int x = 0; x < width; x++)
        {
            float yValue = Mathf.Sin((x + time * 100f) * frequency * Mathf.PI / 180f) * amplitude;
            int y = Mathf.FloorToInt((yValue + 1) * 0.5f * height);

            if (y >= 0 && y < height)
            {
                pixels[x + y * width] = Color.green;
            }
        }

        // Apply the changes to the texture
        texture.SetPixels(pixels);
        texture.Apply();
    }
}
