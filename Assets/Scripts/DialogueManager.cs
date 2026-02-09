using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button continueButton;

    private string[] lines;
    private int index;
    private System.Action onDialogueEnd;

    void Awake()
    {
        instance = this;
        dialoguePanel.SetActive(false);
        continueButton.onClick.AddListener(NextLine);
    }

    public void StartDialogue(string[] dialogueLines, System.Action onEnd = null)
    {
        Time.timeScale = 0f; // 🔒 freeze game

        lines = dialogueLines;
        index = 0;
        onDialogueEnd = onEnd;

        dialoguePanel.SetActive(true);
        dialogueText.text = lines[index];
    }

    void NextLine()
    {
        index++;

        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = lines[index];
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f; // 🔓 unfreeze game
        onDialogueEnd?.Invoke();
    }

    public void StartDialogueDelayed(
        string[] dialogueLines,
        float delay,
        System.Action onEnd = null
    )
    {
        StartCoroutine(DialogueDelayRoutine(dialogueLines, delay, onEnd));
    }

    IEnumerator DialogueDelayRoutine(
        string[] dialogueLines,
        float delay,
        System.Action onEnd
    )
    {
        yield return new WaitForSeconds(delay);

        StartDialogue(dialogueLines, onEnd);
    }

}
