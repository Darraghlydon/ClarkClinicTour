using System.Collections;
using UnityEngine;

public class GraphicSwitcher : MonoBehaviour
{
    [SerializeField] private CanvasGroup _graphicA;
    [SerializeField] private CanvasGroup _graphicB;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _switchInterval = 2f;

    private bool _isShowingA = true;
    private Coroutine _autoSwitchRoutine;
    private Coroutine _fadeRoutine;

    private void OnEnable()
    {
        if (_graphicA == null || _graphicB == null)
        {
            Debug.LogError("GraphicSwitcher is missing CanvasGroup references.", this);
            return;
        }

        _isShowingA = true;

        _graphicA.alpha = 1f;
        _graphicB.alpha = 0f;

        _graphicA.gameObject.SetActive(true);
        _graphicB.gameObject.SetActive(true);

        _autoSwitchRoutine = StartCoroutine(AutoSwitch());
    }

    private void OnDisable()
    {
        if (_autoSwitchRoutine != null)
        {
            StopCoroutine(_autoSwitchRoutine);
            _autoSwitchRoutine = null;
        }

        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }
    }

    private IEnumerator AutoSwitch()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(_switchInterval);
            SwitchGraphic();
        }
    }

    public void SwitchGraphic()
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }

        if (_isShowingA)
        {
            _fadeRoutine = StartCoroutine(FadeBetween(_graphicA, _graphicB));
        }
        else
        {
            _fadeRoutine = StartCoroutine(FadeBetween(_graphicB, _graphicA));
        }

        _isShowingA = !_isShowingA;
    }

    private IEnumerator FadeBetween(CanvasGroup from, CanvasGroup to)
    {
        float timer = 0f;
        float fromStart = from.alpha;
        float toStart = to.alpha;

        while (timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / _fadeDuration;

            from.alpha = Mathf.Lerp(fromStart, 0f, t);
            to.alpha = Mathf.Lerp(toStart, 1f, t);

            yield return null;
        }

        from.alpha = 0f;
        to.alpha = 1f;
        _fadeRoutine = null;
    }
}