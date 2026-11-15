using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform player;

    public Teleporter linkedTeleporter;

    [Header("Player Controller")]
    public CharacterController characterController;

    public void Teleport()
    {
        if (player == null || linkedTeleporter == null)
        {
            Debug.LogWarning(
                name + ": Player or Linked Teleporter is missing!"
            );
            return;
        }

        Transform destination = linkedTeleporter.transform;

        if (characterController != null)
            characterController.enabled = false;

        player.SetPositionAndRotation(
            destination.position,
            destination.rotation
        );

        if (characterController != null)
            characterController.enabled = true;
    }
}
