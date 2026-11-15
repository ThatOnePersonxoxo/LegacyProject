using UnityEngine;
using TMPro;
using System.Collections;

public class MinigameFinish : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

    [Header("Player Controls")]
    public MonoBehaviour[] disableWhileOpen;

    private bool isOpen;

    private void Start()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (isOpen && Input.GetMouseButtonDown(0))
        {
            Close();
        }
    }

    public void Open()
    {
        StartCoroutine(OpenAfterDelay());
    }

    private IEnumerator OpenAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        panel.SetActive(true);
        isOpen = true;

        foreach (MonoBehaviour control in disableWhileOpen)
        {
            if (control != null)
            {
                control.enabled = false;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Close()
    {
        panel.SetActive(false);
        isOpen = false;

        foreach (MonoBehaviour control in disableWhileOpen)
        {
            if (control != null)
            {
                control.enabled = true;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
