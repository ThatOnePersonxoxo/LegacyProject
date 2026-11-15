using UnityEngine;

public class QuestSequencer : MonoBehaviour
{
    [Header("Quest Sequence")]
    public Minigame[] quests;

    [Header("Unlock")]
    public GameObject teleporterA;

    private int currentQuest = 0;

    private void Start()
    {
        if (teleporterA != null)
            teleporterA.SetActive(false);

        for (int i = 0; i < quests.Length; i++)
        {
            quests[i].onComplete.AddListener(CompleteCurrentQuest);
        }
    }

    public void OpenNextQuest()
    {
        if (currentQuest >= quests.Length)
            return;

        quests[currentQuest].Open();
    }

    private void CompleteCurrentQuest()
    {
        currentQuest++;

        if (currentQuest >= quests.Length)
        {
            if (teleporterA != null)
                teleporterA.SetActive(true);
        }
    }
}
