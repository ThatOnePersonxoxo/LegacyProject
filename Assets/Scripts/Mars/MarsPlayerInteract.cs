using UnityEngine;

public class MarsPlayerInteract : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 3f;

    private Interactable currentInteractable;
    private InteractableMars currentMarsInteractable;

    void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
            else if (currentMarsInteractable != null)
            {
                currentMarsInteractable.Interact(transform);
            }
        }
    }

    void CheckForInteractable()
    {
        Interactable newInteractable = null;
        InteractableMars newMarsInteractable = null;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit,
            interactionDistance, Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide))
        {
            newInteractable =
                hit.collider.GetComponentInParent<Interactable>();

            if (newInteractable == null)
            {
                newMarsInteractable =
                    hit.collider.GetComponentInParent<InteractableMars>();
            }
        }

        if (currentInteractable != newInteractable ||
            currentMarsInteractable != newMarsInteractable)
        {
            // Hide old Moon prompt
            if (currentInteractable != null &&
                currentInteractable.interactionPrompt != null)
            {
                currentInteractable.interactionPrompt.SetActive(false);
            }

            // Hide old Mars prompt
            if (currentMarsInteractable != null &&
                currentMarsInteractable.interactPrompt != null)
            {
                currentMarsInteractable.interactPrompt.SetActive(false);
            }

            currentInteractable = newInteractable;
            currentMarsInteractable = newMarsInteractable;

            // Show new Moon prompt
            if (currentInteractable != null &&
                currentInteractable.interactionPrompt != null)
            {
                currentInteractable.interactionPrompt.SetActive(true);
            }

            // Show new Mars prompt
            if (currentMarsInteractable != null &&
                currentMarsInteractable.interactPrompt != null)
            {
                currentMarsInteractable.interactPrompt.SetActive(true);
            }
        }
    }
}
