using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class SubtitleController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _subtitleTextField;
    [SerializeField] private GameObject _subtitlePanel;

    [Header("Subtitle Timing")]
    [SerializeField] private int _maxCharsPerChunk = 45;
    [SerializeField] private float _secondsPerChunk = 2.5f;
    [SerializeField] private float _minimumWaitTime = 2f;

    private Coroutine currentCoroutine;
    private Queue<string> subtitleQueue = new Queue<string>();
    private bool isDisplaying = false;

    private void Awake()
    {
        _subtitlePanel.SetActive(false);
    }

    private void OnEnable()
    {
        Events.DisplaySubtitles.Subscribe(OnSubtitleRequested);
        Events.AudioSkip.Subscribe(SkipSubtitles);
    }

    private void OnDisable()
    {
        Events.DisplaySubtitles.Unsubscribe(OnSubtitleRequested);
        Events.AudioSkip.Unsubscribe(SkipSubtitles);
    }

    private void SkipSubtitles() { 
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            _subtitleTextField.text = "";
            _subtitlePanel.SetActive(false);
        }
    }

private void OnSubtitleRequested(string fullSubtitle)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        _subtitlePanel.SetActive(true);
        subtitleQueue.Clear();    
        _subtitleTextField.text = "";  

        foreach (var chunk in SplitIntoChunks(fullSubtitle, _maxCharsPerChunk))
            subtitleQueue.Enqueue(chunk);

        currentCoroutine = StartCoroutine(ProcessQueue());
    }


    private IEnumerator ProcessQueue()
    {
        isDisplaying = true;

        while (subtitleQueue.Count > 0)
        {
            string nextChunk = subtitleQueue.Dequeue();
            _subtitleTextField.text = nextChunk;

            bool isLastChunk = subtitleQueue.Count == 0;

            float waitTime = _secondsPerChunk;

            if (isLastChunk)
            {
                float fillRatio = nextChunk.Length / (float)_maxCharsPerChunk;
                waitTime = _secondsPerChunk * fillRatio;
                waitTime = Mathf.Max(waitTime, _minimumWaitTime);
            }

            yield return new WaitForSeconds(waitTime);
        }

        _subtitlePanel.SetActive(false);
        _subtitleTextField.text = "";
        isDisplaying = false;
        Events.SubtitlesSkip.Publish();
    }

    private List<string> SplitIntoChunks(string text, int maxChars)
    {
        List<string> chunks = new List<string>();
        if (string.IsNullOrWhiteSpace(text))
            return chunks;

        string[] words = text.Split(' ');
        StringBuilder current = new StringBuilder();

        foreach (string word in words)
        {
            if (current.Length + word.Length + 1 > maxChars)
            {
                chunks.Add(current.ToString());
                current.Clear();
            }

            if (current.Length > 0)
                current.Append(" ");

            current.Append(word);
        }

        if (current.Length > 0)
            chunks.Add(current.ToString());

        return chunks;
    }
}
