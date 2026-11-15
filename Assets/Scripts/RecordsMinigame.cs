
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchiveLoggingMinigame : Minigame
{
    [Header("Archive Records (Correct Chronological Order)")]
    [TextArea(2, 3)]
    public string[] archiveRecords =
    {
        "16 July 1969 | Apollo 11 launches from Earth.",
        "19 July 1969 | Apollo 11 enters lunar orbit.",
        "20 July 1969 | Eagle lands in the Sea of Tranquility.",
        "21 July 1969 | Neil Armstrong takes the first step onto the Moon.",
        "21 July 1969 | The Passive Seismic Experiment is deployed.",
        "24 July 1969 | Apollo 11 returns to Earth."
    };

    [Header("Record UI")]
    public Button[] recordButtons;
    public TMP_Text[] recordTexts;

    [Header("Controls")]
    public Button moveUpButton;
    public Button moveDownButton;
    public Button submitButton;

    [Header("Feedback")]
    public TMP_Text feedbackText;

    private string[] currentOrder;
    private int selectedIndex = -1;

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < recordButtons.Length; i++)
        {
            int index = i;
            recordButtons[i].onClick.AddListener(
                () => SelectRecord(index)
            );
        }

        moveUpButton.onClick.AddListener(MoveUp);
        moveDownButton.onClick.AddListener(MoveDown);
        submitButton.onClick.AddListener(CheckOrder);
    }

    protected override void OnOpened()
    {
        currentOrder = (string[])archiveRecords.Clone();

        // Shuffle the records
        for (int i = currentOrder.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            string temp = currentOrder[i];
            currentOrder[i] = currentOrder[randomIndex];
            currentOrder[randomIndex] = temp;
        }

        // Avoid accidentally starting in the correct order
        if (currentOrder.Length > 1 &&
            IsCorrectOrder())
        {
            string temp = currentOrder[0];
            currentOrder[0] = currentOrder[1];
            currentOrder[1] = temp;
        }

        selectedIndex = -1;

        if (feedbackText != null)
            feedbackText.text = "Arrange the mission records chronologically.";

        RefreshRecords();
    }

    private void SelectRecord(int index)
    {
        selectedIndex = index;

        if (feedbackText != null)
            feedbackText.text = "Selected: " + currentOrder[index];

    }

    private void MoveUp()
    {
        if (selectedIndex <= 0)
            return;

        SwapRecords(selectedIndex, selectedIndex - 1);
        selectedIndex--;

        RefreshRecords();
    }

    private void MoveDown()
    {
        if (selectedIndex < 0 ||
            selectedIndex >= currentOrder.Length - 1)
            return;

        SwapRecords(selectedIndex, selectedIndex + 1);
        selectedIndex++;

        RefreshRecords();
    }

    private void SwapRecords(int a, int b)
    {
        string temp = currentOrder[a];
        currentOrder[a] = currentOrder[b];
        currentOrder[b] = temp;
    }

    private void RefreshRecords()
    {
        for (int i = 0; i < recordTexts.Length; i++)
        {
            recordTexts[i].text =
                (i + 1) + ". " + currentOrder[i];
        }
    }

    private void CheckOrder()
    {
        if (IsCorrectOrder())
        {
            if (feedbackText != null)
                feedbackText.text = "Archive successfully logged!";

            Complete();
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text =
                    "Incorrect sequence. Review the mission dates.";
        }
    }

    private bool IsCorrectOrder()
    {
        for (int i = 0; i < archiveRecords.Length; i++)
        {
            if (currentOrder[i] != archiveRecords[i])
                return false;
        }

        return true;
    }
}
