using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _fpsGUIText;
    private int _frameCount;
    private float _elapsedTime;
    void Update()
    {
        _frameCount++;
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= 1f)
        {
            _fpsGUIText.text = "FPS: " + _frameCount.ToString();
            _frameCount = 0;
            _elapsedTime = 0f;
        }
    }

    public int GetFrameCount()
    {
        return _frameCount;
    }
}
