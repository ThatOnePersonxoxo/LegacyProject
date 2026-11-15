using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransmissionDisplay : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Image progressFill;            // Image Type = Filled, Fill Method = Horizontal
    public TMP_Text counterText;          // "Images received: 1,234 / 11,000"
    public TMP_Text fileNameText;         // the file currently arriving
    public TMP_Text statusText;
    public RawImage previewImage;         // flips through the snapshots

    [Header("Content")]
    public Texture2D[] snapshots;         // a handful of pictures to flash by
    public int totalImages = 11000;
    public string filePrefix = "SURV1_";

    [Header("Timing")]
    public float seconds = 12f;                 // how long the transmission takes
    public float previewChangeSeconds = 0.4f;   // how often the preview picture changes
    public float closeAfterSeconds = 4f;        // how long the final screen stays up

    public UnityEvent onFinished;               // e.g. the Morse cutscene or QuestManager.CompleteCurrentStep

    private bool running;

    private void Start()
    {
        panel.SetActive(false);
    }

    // Hook this up to the picture minigame's "On Complete"
    public void Play()
    {
        if (!running)
            StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        running = true;
        panel.SetActive(true);

        if (statusText != null)
            statusText.text = "Receiving transmission...";

        float t = 0f;
        float previewTimer = 0f;

        while (t < seconds)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / seconds);
            int received = Mathf.RoundToInt(k * totalImages);

            if (progressFill != null)
                progressFill.fillAmount = k;

            if (counterText != null)
                counterText.text = "Images received: " + received.ToString("N0") + " / " + totalImages.ToString("N0");

            if (fileNameText != null)
                fileNameText.text = filePrefix + received.ToString("D5") + ".IMG";

            // Flash a random snapshot every so often
            previewTimer -= Time.deltaTime;
            if (previewTimer <= 0f && previewImage != null && snapshots.Length > 0)
            {
                previewImage.texture = snapshots[Random.Range(0, snapshots.Length)];
                previewTimer = previewChangeSeconds;
            }

            yield return null;
        }

        if (progressFill != null)
            progressFill.fillAmount = 1f;

        if (counterText != null)
            counterText.text = "Images received: " + totalImages.ToString("N0") + " / " + totalImages.ToString("N0");

        if (statusText != null)
            statusText.text = "Transmission complete.";

        yield return new WaitForSeconds(closeAfterSeconds);

        panel.SetActive(false);
        running = false;
        onFinished.Invoke();
    }
}
