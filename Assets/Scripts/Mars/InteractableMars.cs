
using UnityEngine;
using UnityEngine.Events;

public class InteractableMars : MonoBehaviour
{
    [Header("Interaction")]
    public UnityEvent onInteract;
    public bool onlyOnce;

    [Header("Interaction Prompt")]
    public GameObject interactPrompt;

    private bool used;
    private bool playerInRange;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !used)
        {
            playerInRange = true;

            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    public virtual void Interact(Transform interactor)
    {
        if (!playerInRange || (onlyOnce && used))
            return;

        used = true;
        onInteract.Invoke();

        if (onlyOnce && interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}
