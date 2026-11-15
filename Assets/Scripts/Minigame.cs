using UnityEngine;
using UnityEngine.Events;

// Base class for every minigame. Handles opening/closing the panel,
// freezing the player, and freeing the mouse cursor.
// Put the script on an always-active object (like the Canvas), NOT on the panel itself.
public abstract class Minigame : MonoBehaviour
{
    public GameObject panel;                    // the minigame's UI panel
    public MonoBehaviour[] disableWhileOpen;    // drag FirstPersonController and PlayerInteract here
    public UnityEvent onComplete;               // what happens when the player wins

    public bool IsOpen
    {
        get { return panel.activeSelf; }
    }

    protected virtual void Start()
    {
        panel.SetActive(false);
    }

    public void Open()
    {
        panel.SetActive(true);
        SetPlayerControl(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnOpened();
    }

    public void Close()
    {
        OnClosed();
        panel.SetActive(false);
        SetPlayerControl(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected void Complete()
    {
        Close();
        onComplete.Invoke();
    }

    protected abstract void OnOpened();

    protected virtual void OnClosed()
    {

    }

    private void SetPlayerControl(bool allowed)
    {
        foreach (MonoBehaviour script in disableWhileOpen)
        {
            script.enabled = allowed;
        }

        Cursor.lockState = allowed ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !allowed;
    }
}
