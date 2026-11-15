using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PSEPLevelingMinigame : Minigame
{
    [Header("Level UI")]
    public RectTransform levelBubble;
    public TMP_Text statusText;

    [Header("Levelling")]
    public float adjustmentSpeed = 0.8f;
    public float bubbleRange = 100f;
    public float levelTolerance = 0.06f;

    private float tiltX;
    private float tiltY;

    protected override void OnOpened()
    {
        // Start with the instrument slightly misaligned
        tiltX = Random.Range(-0.7f, 0.7f);
        tiltY = Random.Range(-0.7f, 0.7f);

        UpdateVisuals();
    }

    private void Update()
    {
        if (!IsOpen)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.leftArrowKey.isPressed)
            horizontal -= 1f;

        if (Keyboard.current.rightArrowKey.isPressed)
            horizontal += 1f;

        if (Keyboard.current.upArrowKey.isPressed)
            vertical += 1f;

        if (Keyboard.current.downArrowKey.isPressed)
            vertical -= 1f;

        tiltX = Mathf.Clamp(
            tiltX + horizontal * adjustmentSpeed * Time.deltaTime,
            -1f, 1f
        );

        tiltY = Mathf.Clamp(
            tiltY + vertical * adjustmentSpeed * Time.deltaTime,
            -1f, 1f
        );

        CheckLevel();
        UpdateVisuals();
    }

    private void CheckLevel()
    {
        bool isLevel =
            Mathf.Abs(tiltX) <= levelTolerance &&
            Mathf.Abs(tiltY) <= levelTolerance;

        if (isLevel)
        {
            Complete();
        }
    }

    private void UpdateVisuals()
    {
        if (levelBubble != null)
        {
            levelBubble.anchoredPosition = new Vector2(
                tiltX * bubbleRange,
                tiltY * bubbleRange
            );
        }

        if (statusText != null)
        {
            bool isLevel =
                Mathf.Abs(tiltX) <= levelTolerance &&
                Mathf.Abs(tiltY) <= levelTolerance;

            if (isLevel)
            {
                statusText.text = "Instrument successfully levelled!";
            }
            else
            {
                statusText.text =
                    "Adjust instrument using arrow keys.\n" +
                    "Horizontal tilt: " + tiltX.ToString("0.00") +
                    "\nVertical tilt: " + tiltY.ToString("0.00");
            }
        }
    }
}
