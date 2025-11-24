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

    private Coroutine currentCoroutine;
    private Queue<string> subtitleQueue = new Queue<string>();
    private bool isDisplaying = false;

    private void Awake()
    {
        _subtitlePanel.SetActive(false);
    }

    private void OnEnable()
    {
        Events.DisplaySubtitle.Subscribe(OnSubtitleRequested);
        Events.AudioSkip.Subscribe(SkipSubtitles);
    }

    private void OnDisable()
    {
        Events.DisplaySubtitle.Unsubscribe(OnSubtitleRequested);
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
        // PRIORITY BEHAVIOR — new subtitle interrupts immediately
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        _subtitlePanel.SetActive(true);
        subtitleQueue.Clear();     // Clear older pending chunks
        _subtitleTextField.text = "";  // Clear UI now

        // Split and enqueue new chunks
        foreach (var chunk in SplitIntoChunks(fullSubtitle, _maxCharsPerChunk))
            subtitleQueue.Enqueue(chunk);

        // Start the new display coroutine
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
                // e.g., 50/200 = 0.25

                waitTime = _secondsPerChunk * fillRatio;

                // Optional safety clamp so tiny fragments still show briefly:
                Debug.Log(waitTime);
                waitTime = Mathf.Max(waitTime, 5f);
            }
            Debug.Log("Final Wait Time: " + waitTime);
            yield return new WaitForSeconds(waitTime);
        }

        _subtitlePanel.SetActive(false);
        _subtitleTextField.text = "";
        isDisplaying = false;
    }


    // Splits text without cutting words
    private List<string> SplitIntoChunks(string text, int maxChars)
    {
        List<string> chunks = new List<string>();
        if (string.IsNullOrWhiteSpace(text))
            return chunks;

        string[] words = text.Split(' ');
        StringBuilder current = new StringBuilder();

        foreach (string word in words)
        {
            // If adding the word exceeds max chars, commit the current chunk
            if (current.Length + word.Length + 1 > maxChars)
            {
                chunks.Add(current.ToString());
                current.Clear();
            }

            if (current.Length > 0)
                current.Append(" ");

            current.Append(word);
        }

        // Add the last chunk
        if (current.Length > 0)
            chunks.Add(current.ToString());

        return chunks;
    }
}
