using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    public UnityEvent onInteract;
    public GameObject interactionPrompt;

    void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    public void Interact()
    {
        onInteract.Invoke();
    }

    public virtual void Interact(Transform interactor)
    {
        onInteract.Invoke();
    }
}