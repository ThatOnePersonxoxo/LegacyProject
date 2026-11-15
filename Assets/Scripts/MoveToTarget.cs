using System.Collections;
using UnityEngine;

// Moves this object smoothly to a target position. Hook Go() up to any "On Complete" event.
public class MoveToTarget : MonoBehaviour
{
    public Transform target;          // e.g. an empty object on the shuttle computer's desk
    public float seconds = 2f;
    public bool detachFirst = true;   // unparent first so it doesn't stay stuck to the probe

    public void Go()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        if (detachFirst)
            transform.SetParent(null, true);

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float t = 0f;

        while (t < seconds)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / seconds);
            transform.position = Vector3.Lerp(startPosition, target.position, k);
            transform.rotation = Quaternion.Slerp(startRotation, target.rotation, k);
            yield return null;
        }

        transform.SetPositionAndRotation(target.position, target.rotation);
    }
}
