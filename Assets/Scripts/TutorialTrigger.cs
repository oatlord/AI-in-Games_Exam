using UnityEngine;
using System;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public string[] dialogueLines;

    [Header("Trigger Rules")]
    public bool triggerOnce = true;
    public TutorialTriggerTarget triggerTarget;

    private bool triggered = false;

    public enum TutorialTriggerTarget
    {
        Player,
        AI,
        Both
    }

    // 🔹 Call THIS instead of TriggerDialogue directly
    public void TryTrigger(GameObject caller, Action onDialogueEnd = null)
    {
        if (triggerOnce && triggered) return;
        if (!IsValidTrigger(caller)) return;

        triggered = true;

        DialogueManager.instance.StartDialogue(dialogueLines, onDialogueEnd);
    }

    // 🔹 Optional: keep old method if you still use it somewhere
    public void TriggerDialogue(Action onDialogueEnd = null)
    {
        if (triggerOnce && triggered) return;

        triggered = true;

        DialogueManager.instance.StartDialogue(dialogueLines, onDialogueEnd);
    }

    private bool IsValidTrigger(GameObject caller)
    {
        if (triggerTarget == TutorialTriggerTarget.Both)
            return true;

        if (triggerTarget == TutorialTriggerTarget.Player &&
            caller.CompareTag("Player"))
            return true;

        if (triggerTarget == TutorialTriggerTarget.AI &&
            caller.CompareTag("AI"))
            return true;

        return false;
    }

    public float dialogueDelay = 0.5f;

    public void TryTriggerDelayed(GameObject caller, System.Action onDialogueEnd = null)
    {
        if (triggerOnce && triggered) return;
        if (!IsValidTrigger(caller)) return;

        triggered = true;

        DialogueManager.instance.StartDialogueDelayed(
            dialogueLines,
            dialogueDelay,
            onDialogueEnd
        );
    }

}
