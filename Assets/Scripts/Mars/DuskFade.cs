using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Fades the screen down to dusk, then shows a message. Hook Play() to the rover's "On Interact".
public class DuskFade : MonoBehaviour
{
    [Header("Screen fade")]
    public Image overlay;                       // a full-screen Image on your Canvas (turn Raycast Target off)
    public Color duskColor = new Color(0.04f, 0.05f, 0.12f);
    [Range(0f, 1f)]
    public float maxAlpha = 0.8f;
    public float fadeSeconds = 5f;

    [Header("Sky light (optional)")]
    public Light sun;                           // your Directional Light
    public float sunEndIntensity = 0.15f;
    public Color sunEndColor = new Color(1f, 0.45f, 0.25f);

    [Header("Message")]
    public TMP_Text messageText;
    [TextArea(2, 4)]
    public string message = "My battery is low and its getting dark";
    public float textFadeSeconds = 2f;
    public float holdSeconds = 6f;
    public bool keepTextVisible = true;         // leave the message on screen at the end

    [Header("Other")]
    public MonoBehaviour[] disableWhileRunning; // FirstPersonController and PlayerInteract
    public bool restoreControlAtEnd = false;    // leave off if this is the end of the level
    public UnityEvent onFinished;

    private bool running;

    private void Start()
    {
        if (overlay != null)
        {
            Color c = duskColor;
            c.a = 0f;
            overlay.color = c;
        }

        if (messageText != null)
            messageText.alpha = 0f;
    }

    public void Play()
    {
        if (!running)
            StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        running = true;

        foreach (MonoBehaviour script in disableWhileRunning)
            script.enabled = false;

        if (messageText != null)
        {
            messageText.text = message;
            messageText.alpha = 0f;
        }

        float startIntensity = sun != null ? sun.intensity : 0f;
        Color startColor = sun != null ? sun.color : Color.white;

        // Fade down to dusk
        float t = 0f;
        while (t < fadeSeconds)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / fadeSeconds);

            if (overlay != null)
            {
                Color c = duskColor;
                c.a = maxAlpha * k;
                overlay.color = c;
            }

            if (sun != null)
            {
                sun.intensity = Mathf.Lerp(startIntensity, sunEndIntensity, k);
                sun.color = Color.Lerp(startColor, sunEndColor, k);
            }

            yield return null;
        }

        // Fade the message in
        t = 0f;
        while (t < textFadeSeconds && messageText != null)
        {
            t += Time.deltaTime;
            messageText.alpha = Mathf.Clamp01(t / textFadeSeconds);
            yield return null;
        }

        yield return new WaitForSeconds(holdSeconds);

        if (!keepTextVisible && messageText != null)
        {
            t = 0f;
            while (t < textFadeSeconds)
            {
                t += Time.deltaTime;
                messageText.alpha = 1f - Mathf.Clamp01(t / textFadeSeconds);
                yield return null;
            }
        }

        if (restoreControlAtEnd)
        {
            foreach (MonoBehaviour script in disableWhileRunning)
                script.enabled = true;
        }

        running = false;
        onFinished.Invoke();
    }
}
