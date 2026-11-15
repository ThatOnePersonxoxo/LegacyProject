using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Counts how many things have been done (3 pieces of debris, 2 cables...), shows it, then fires an event.
public class StepCounter : MonoBehaviour
{
    public int required = 3;
    public TMP_Text label;                          // optional counter text, e.g. on your HUD
    public string format = "Debris: {0}/{1}";       // {0} = done so far, {1} = required
    public UnityEvent onReached;

    private int count;

    private void Start()
    {
        UpdateLabel();
    }

    public void Add()
    {
        count++;
        UpdateLabel();

        if (count == required)
            onReached.Invoke();
    }

    private void UpdateLabel()
    {
        if (label != null)
            label.text = string.Format(format, count, required);
    }
}
