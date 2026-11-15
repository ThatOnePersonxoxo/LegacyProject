using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 3f;

    private Interactable currentInteractable;

    void Update()
    {
        CheckForInteractable();

        if (currentInteractable != null &&
            Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    void CheckForInteractable()
    {
        Interactable newInteractable = null;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            newInteractable =
                hit.collider.GetComponentInParent<Interactable>();
        }

        if (currentInteractable != newInteractable)
        {
            if (currentInteractable != null &&
                currentInteractable.interactionPrompt != null)
            {
                currentInteractable.interactionPrompt.SetActive(false);
            }

            currentInteractable = newInteractable;

            if (currentInteractable != null &&
                currentInteractable.interactionPrompt != null)
            {
                currentInteractable.interactionPrompt.SetActive(true);
            }
        }
    }
}