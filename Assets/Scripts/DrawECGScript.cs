using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawECGScript : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private Color _backgroundColor = new Color(0f, 0f, 0f, 0f);
    [SerializeField] private int _width = 50; // Width of the texture
    [SerializeField] private int _height = 10; // Height of the texture
    [SerializeField] private float _speed = 1f; // Speed at which the wave moves through its cycle

    [SerializeField] private float _pWaveMultiplier;
    [SerializeField] private float _qWaveMultiplier;
    [SerializeField] private float _rWaveMultiplier;
    [SerializeField] private float _sWaveMultiplier;
    [SerializeField] private float _tWaveMultiplier;

    [SerializeField] private int _lineThickness = 3;
    [SerializeField] private RawImage _ecgDisplay; // Reference to the RawImage component
    [SerializeField] private Color _textureColor = Color.green;

    [Header("Update Control")]
    [SerializeField] private float _secondsPerStep = 0.02f; // Time between ECG column updates
    [SerializeField] private int _maxStepsPerFrame = 10;    // Prevent too many catch-up steps in one frame

    private Texture2D _texture;
    private float _time;
    private float _stepTimer;
    private int _currentX;
    private int _previousY;
    private Renderer _rend;
    private bool _hasPreviousPoint;


    void Awake()
    {
        // Initialize the texture
        _texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
        _texture.filterMode = FilterMode.Point;
        _texture.wrapMode = TextureWrapMode.Clamp;

        // Assign the texture to the RawImage component
        _ecgDisplay.texture = _texture;

        // Clear the texture initially
        ClearTexture();

        _rend = GetComponent<Renderer>();

        _lineThickness = Mathf.Max(1, _lineThickness);
        _secondsPerStep = Mathf.Max(0.001f, _secondsPerStep);
        _maxStepsPerFrame = Mathf.Max(1, _maxStepsPerFrame);
    }

    public void InitialiseDisplay()
    {
        DrawECGOnce();
    }

    void Update()
    {
        _stepTimer += Time.deltaTime;

        int stepsThisFrame = 0;

        while (_stepTimer >= _secondsPerStep && stepsThisFrame < _maxStepsPerFrame)
        {
            _stepTimer -= _secondsPerStep;

            // Advance time
            _time += _secondsPerStep * _speed;

            // Draw new ECG data
            DrawECGStep();

            stepsThisFrame++;
        }

        // Apply the changes to the texture
        if (stepsThisFrame > 0)
        {
            _texture.Apply();
        }
    }

    void ClearTexture()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _texture.SetPixel(x, y, _backgroundColor);
            }
        }
        _texture.Apply();
    }

    void DrawThickPixel(int x, int y, Color color)
    {
        int halfThickness = _lineThickness / 2;

        for (int offsetX = -halfThickness; offsetX <= halfThickness; offsetX++)
        {
            for (int offsetY = -halfThickness; offsetY <= halfThickness; offsetY++)
            {
                int drawX = x + offsetX;
                int drawY = y + offsetY;

                if (drawX >= 0 && drawX < _width && drawY >= 0 && drawY < _height)
                {
                    _texture.SetPixel(drawX, drawY, color);
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
        float t = _time % 1f; // Simulate one heartbeat cycle per second
        float yValue = SimulateECG(t);
        int y = Mathf.FloorToInt((yValue + 1f) * 0.5f * (_height - 1));
        y = Mathf.Clamp(y, 0, _height - 1);

        int drawX = _currentX;
        int prevX = (_currentX - 1 + _width) % _width;

        // Draw the ECG value at the current position
        DrawThickPixel(drawX, y, _textureColor);

        // Join this point to the previous one so jumps do not appear isolated
        if (_hasPreviousPoint)
        {
            DrawLine(prevX, _previousY, drawX, y, _textureColor);
        }

        _previousY = y;
        _hasPreviousPoint = true;

        // Clear the previous column before moving on to the next
        ClearPreviousColumn();

        // Move to the next x position
        _currentX = (_currentX + 1) % _width;
    }

    void DrawECGOnce()
    {
        int previousDrawY = -1;

        for (int x = 0; x < _width; x++)
        {
            float t = (float)x / (_width - 1);   // 0 to 1 across the width
            float yValue = SimulateECG(t);

            int y = Mathf.FloorToInt((yValue + 1f) * 0.5f * (_height - 1));
            y = Mathf.Clamp(y, 0, _height - 1);

            DrawThickPixel(x, y, _textureColor);

            // join gaps between points so it looks like a line
            if (previousDrawY != -1)
            {
                DrawLine(x - 1, previousDrawY, x, y, _textureColor);
            }

            previousDrawY = y;
        }

        _texture.Apply();
    }

    void ClearPreviousColumn()
    {
        int halfThickness = _lineThickness / 2;
        int clearXCenter = (_currentX + 1) % _width;

        for (int offsetX = -halfThickness; offsetX <= halfThickness; offsetX++)
        {
            int clearX = clearXCenter + offsetX;

            while (clearX < 0)
            {
                clearX += _width;
            }

            clearX %= _width;

            for (int y = 0; y < _height; y++)
            {
                _texture.SetPixel(clearX, y, _backgroundColor);
            }
        }
    }

    float SimulateECG(float t)
    {
        // P Wave: small upward wave
        float pWave = _pWaveMultiplier * Mathf.Sin(2 * Mathf.PI * (t - 0.2f) * 10) * Mathf.Exp(-((t - 0.2f) * 30) * ((t - 0.2f) * 30));

        // QRS Complex: sharp peak and trough
        float qWave = _qWaveMultiplier * Mathf.Exp(-((t - 0.35f) * 50) * ((t - 0.35f) * 50));
        float rWave = _rWaveMultiplier * Mathf.Exp(-((t - 0.4f) * 100) * ((t - 0.4f) * 100));
        float sWave = _sWaveMultiplier * Mathf.Exp(-((t - 0.45f) * 50) * ((t - 0.45f) * 50));

        // T Wave: smaller, longer upward wave
        float tWave = _tWaveMultiplier * Mathf.Sin(2 * Mathf.PI * (t - 0.6f) * 5) * Mathf.Exp(-((t - 0.6f) * 20) * ((t - 0.6f) * 20));

        // Combined ECG signal
        return pWave + qWave + rWave + sWave + tWave;
    }
}