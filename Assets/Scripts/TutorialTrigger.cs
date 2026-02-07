using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string[] dialogueLines;
    public bool triggerOnce = true;

    private bool triggered = false;

    public void TriggerDialogue(System.Action onDialogueEnd = null)
    {
        if (triggerOnce && triggered) return;

        triggered = true;

        DialogueManager.instance.StartDialogue(dialogueLines, onDialogueEnd);
    }
}
