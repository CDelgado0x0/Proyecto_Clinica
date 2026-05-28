using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private float typingSpeed = 0.03f;
    public int maxCharactersPerPage = 120;

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>();
    private Queue<string> pageQueue = new Queue<string>();
    private DialogueData currentDialogue;
    private Coroutine typingCoroutine;
    private WaitForSeconds typingDelay;
    private string currentLine;
    private bool isDialogueActive;
    private bool isTyping;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        typingDelay = new WaitForSeconds(typingSpeed);
    }

    // ── API pública ───────────────────────────────────────────

    public void StartDialogue(DialogueData dialogue)
    {
        linesQueue.Clear();
        pageQueue.Clear();

        foreach (DialogueLine line in dialogue.lines)
            linesQueue.Enqueue(line);

        currentDialogue = dialogue;
        isDialogueActive = true;

        dialogueUI.Show();
        ShowNextLine();
    }

    public void OnUserNext()
    {
        if (!isDialogueActive) return;
        ShowNextLine();
    }

    // ── Lógica interna ────────────────────────────────────────

    private void ShowNextLine()
    {
        // Si está escribiendo, muestra la línea completa al instante
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueUI.SetText(currentLine);
            isTyping = false;
            return;
        }

        // Si hay páginas pendientes de la línea actual
        if (pageQueue.Count > 0)
        {
            currentLine = pageQueue.Dequeue();
            typingCoroutine = StartCoroutine(TypeLine(currentLine));
            return;
        }

        // Si no hay más líneas, termina el diálogo
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Procesa la siguiente línea
        DialogueLine line = linesQueue.Dequeue();

        string nameToShow = string.IsNullOrEmpty(line.characterName)
            ? currentDialogue.defaultCharacterName
            : line.characterName;

        dialogueUI.SetName(nameToShow);

        if (line.voiceClip != null)
            dialogueUI.PlayVoice(line.voiceClip); // delegamos el audio al DialogueUI

        List<string> pages = SplitTextIntoPages(line.text);
        pageQueue = new Queue<string>(pages);

        currentLine = pageQueue.Dequeue();
        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        dialogueUI.SetText("");

        foreach (char c in text)
        {
            dialogueUI.AppendChar(c); // evita concatenar strings en cada letra
            yield return typingDelay;
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueUI.Hide();
    }

    private int GetMaxCharacters()
    {
        float fontSize = SettingsManager.Instance.CurrentSettings.dialogFontSize;
        return Mathf.RoundToInt(maxCharactersPerPage * (24f / fontSize));
    }

    private List<string> SplitTextIntoPages(string text)
    {
        List<string> pages = new List<string>();
        string[] words = text.Split(' ');
        string currentPage = "";
        int maxChars = GetMaxCharacters();

        foreach (string word in words)
        {
            if (currentPage.Length + word.Length + 1 > maxChars)
            {
                if (!string.IsNullOrWhiteSpace(currentPage))
                    pages.Add(currentPage.Trim());

                currentPage = "";
            }

            currentPage += word + " ";
        }

        if (!string.IsNullOrWhiteSpace(currentPage))
            pages.Add(currentPage.Trim());

        return pages;
    }
}