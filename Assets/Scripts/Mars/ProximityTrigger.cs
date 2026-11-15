using UnityEngine;
using UnityEngine.Events;

// A trigger zone: fires an event when the player walks into it
// (e.g. "arrived at the heat shield" or "reached the resting place").
[RequireComponent(typeof(Collider))]
public class ProximityTrigger : MonoBehaviour
{
    public string playerTag = "Player";
    public bool onlyOnce = true;
    public UnityEvent onEnter;

    private bool fired;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (onlyOnce && fired)
            return;

        fired = true;
        onEnter.Invoke();
    }
}
