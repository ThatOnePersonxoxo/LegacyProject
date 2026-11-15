using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction = transform.position - player.position;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}