
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AntennaMinigame : Minigame
{
    [Header("UI")]
    public Slider antennaA;
    public Slider antennaB;
    public Image signalMeter;
    public Button lockInButton;
    public TMP_Text feedbackText;

    [Header("Rules")]
    public float tolerance = 0.05f;

    private float targetA;
    private float targetB;

    protected override void Start()
    {
        base.Start();

        antennaA.onValueChanged.AddListener(_ => UpdateMeter());
        antennaB.onValueChanged.AddListener(_ => UpdateMeter());
        lockInButton.onClick.AddListener(LockIn);

        if (feedbackText != null)
        {
            feedbackText.transform.parent.gameObject.SetActive(false);
        }
    }

    protected override void OnOpened()
    {
        targetA = Random.Range(0.15f, 0.85f);
        targetB = Random.Range(0.15f, 0.85f);

        antennaA.value = 0f;
        antennaB.value = 0f;

        if (feedbackText != null)
        {
            feedbackText.transform.parent.gameObject.SetActive(true);
            feedbackText.text = "Adjust the antennae for the strongest signal.";
        }

        UpdateMeter();
    }

    protected override void OnClosed()
    {
        if (feedbackText != null)
        {
            feedbackText.transform.parent.gameObject.SetActive(false);
        }
    }

    private void UpdateMeter()
    {
        float error =
            (Mathf.Abs(antennaA.value - targetA) +
             Mathf.Abs(antennaB.value - targetB)) / 2f;

        signalMeter.fillAmount = Mathf.Clamp01(1f - error * 2f);
    }

    private void LockIn()
    {
        bool aOk = Mathf.Abs(antennaA.value - targetA) <= tolerance;
        bool bOk = Mathf.Abs(antennaB.value - targetB) <= tolerance;

        if (aOk && bOk)
        {
            if (feedbackText != null)
            {
                feedbackText.transform.parent.gameObject.SetActive(false);
            }

            Complete();
        }
        else if (feedbackText != null)
        {
            feedbackText.text = "Signal too weak. Keep adjusting.";
        }
    }
}
