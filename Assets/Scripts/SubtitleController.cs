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

    // Optional safety clamps
    [SerializeField] private float _minimumChunkWaitTime = 0.15f; // avoids 0-second waits
    [SerializeField] private float _minimumLastChunkWaitTime = 2f; // if you still want the last chunk to linger

    private Coroutine currentCoroutine;

    private struct SubtitleChunk
    {
        public string Text;
        public float Duration;

        public SubtitleChunk(string text, float duration)
        {
            Text = text;
            Duration = duration;
        }
    }

    private Queue<SubtitleChunk> subtitleQueue = new Queue<SubtitleChunk>();
    private bool isDisplaying = false;

    private void Awake()
    {
        _subtitlePanel.SetActive(false);
    }

    private void OnEnable()
    {
        // IMPORTANT: your event must now publish (string subtitle, float audioLengthSeconds)
        Events.DisplaySubtitles.Subscribe(OnSubtitleRequested);
        Events.SubtitlesSkip.Subscribe(SkipSubtitles);
    }

    private void OnDisable()
    {
        Events.DisplaySubtitles.Unsubscribe(OnSubtitleRequested);
        Events.SubtitlesSkip.Unsubscribe(SkipSubtitles);
    }

    private void SkipSubtitles()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        subtitleQueue.Clear();
        _subtitleTextField.text = "";
        _subtitlePanel.SetActive(false);
    }

    private void OnSubtitleRequested(string fullSubtitle, float audioLengthSeconds)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        _subtitlePanel.SetActive(true);
        subtitleQueue.Clear();
        _subtitleTextField.text = "";

        // Split into text chunks
        List<string> chunks = SplitIntoChunks(fullSubtitle, _maxCharsPerChunk);

        // Count total words across the full subtitle
        int totalWordCount = CountWords(fullSubtitle);

        if (totalWordCount <= 0 || audioLengthSeconds <= 0f)
        {
            // Fallback: just enqueue with a small default duration so something displays
            foreach (var c in chunks)
                subtitleQueue.Enqueue(new SubtitleChunk(c, _minimumChunkWaitTime));
        }
        else
        {
            float secondsPerWord = audioLengthSeconds / totalWordCount;

            // Queue each chunk with duration based on its word count
            for (int i = 0; i < chunks.Count; i++)
            {
                string chunkText = chunks[i];
                int chunkWordCount = CountWords(chunkText);

                float duration = chunkWordCount * secondsPerWord;
                duration = Mathf.Max(duration, _minimumChunkWaitTime);

                bool isLast = (i == chunks.Count - 1);
                if (isLast)
                    duration = Mathf.Max(duration, _minimumLastChunkWaitTime);

                subtitleQueue.Enqueue(new SubtitleChunk(chunkText, duration));
            }
        }

        currentCoroutine = StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isDisplaying = true;

        while (subtitleQueue.Count > 0)
        {
            SubtitleChunk next = subtitleQueue.Dequeue();
            _subtitleTextField.text = next.Text;

            yield return new WaitForSeconds(next.Duration);
        }

        _subtitlePanel.SetActive(false);
        _subtitleTextField.text = "";
        isDisplaying = false;
        Events.SubtitlesStop.Publish();
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

    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;

        // Split on whitespace, ignore empty entries
        return text.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
