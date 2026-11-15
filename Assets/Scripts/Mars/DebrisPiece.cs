using UnityEngine;

// One piece of debris (parachute, backshell, airbag fabric). Pieces can be found in any order.
public class DebrisPiece : Interactable
{
    public StepCounter counter;     // the debris counter (0/3)
    public string label = "Debris";

    [TextArea(2, 5)]
    public string description = "Describe this piece of debris here.";

    private bool collected;

    public override void Interact(Transform interactor)
    {
        // Only the first inspection counts toward the total
        if (collected)
            return;

        collected = true;

        if (counter != null)
            counter.Add();

        base.Interact(interactor);   // "On Interact", e.g. tint the piece to show it's done
    }
}
