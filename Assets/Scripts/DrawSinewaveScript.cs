using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawSinewaveScript : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] public int _width = 50;
    [SerializeField] public int _height = 10;
    [SerializeField] public float _frequency = 1f;
    [SerializeField] public float _amplitude = 0.5f;
    [SerializeField] public float _speed = 1f;
    [SerializeField] private int _lineThickness = 3;
    [SerializeField] public RawImage _ecgDisplay;
    [SerializeField] private Color _lineColor = Color.green;

    [Header("Update Control")]
    [SerializeField] private float _secondsPerUpdate = 0.02f; // Time between redraws
    [SerializeField] private Color _backgroundColor = new Color(0f, 0f, 0f, 0.3f);

    private Texture2D _texture;
    private float _time;
    private float _updateTimer;
    private Color[] _pixels;

    void Awake()
    {
        _texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
        _texture.filterMode = FilterMode.Point;
        _texture.wrapMode = TextureWrapMode.Clamp;

        _ecgDisplay.texture = _texture;
        _pixels = new Color[_width * _height];

        _lineThickness = Mathf.Max(1, _lineThickness);
        _secondsPerUpdate = Mathf.Max(0.001f, _secondsPerUpdate);

        DrawECG();
    }

    void Update()
    {
        _updateTimer += Time.deltaTime;

        if (_updateTimer < _secondsPerUpdate)
            return;

        _time += _updateTimer * _speed;
        _updateTimer = 0f;

        DrawECG();
    }

    void DrawECG()
    {
        for (int i = 0; i < _pixels.Length; i++)
        {
            _pixels[i] = _backgroundColor;
        }

        for (int x = 0; x < _width; x++)
        {
            float yValue = Mathf.Sin((x + _time * 100f) * _frequency * Mathf.PI / 180f) * _amplitude;
            int y = Mathf.FloorToInt((yValue + 1f) * 0.5f * (_height - 1));
            y = Mathf.Clamp(y, 0, _height - 1);

            DrawThickPixel(x, y, _lineColor);
        }

        _texture.SetPixels(_pixels);
        _texture.Apply();
    }

    void DrawThickPixel(int x, int y, Color color)
    {
        int halfThickness = _lineThickness / 2;

        for (int offset = -halfThickness; offset <= halfThickness; offset++)
        {
            int drawY = y + offset;

            if (drawY >= 0 && drawY < _height)
            {
                _pixels[x + drawY * _width] = color;
            }
        }
    }
}