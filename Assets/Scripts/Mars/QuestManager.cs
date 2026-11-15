using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public string objective;       // shown on screen, e.g. "Adjust the RADVS antennae"
        public UnityEvent onStart;     // e.g. turn on the antenna's Interactable
        public UnityEvent onComplete;  // e.g. play the radio signal effect
    }

    public TMP_Text objectiveText;
    public Step[] steps;

    private int current = -1;

    private void Start()
    {
        NextStep();
    }

    // Hook this up to a minigame's "On Complete" or an Interactable's "On Interact"
    public void CompleteCurrentStep()
    {
        if (current >= 0 && current < steps.Length)
        {
            steps[current].onComplete.Invoke();
        }
        NextStep();
    }

    private void NextStep()
    {
        current++;

        if (current < steps.Length)
        {
            objectiveText.text = steps[current].objective;
            steps[current].onStart.Invoke();
        }
        else
        {
            objectiveText.text = "All objectives complete!";
        }
    }
}
