using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public DialogueUI dialogueUI;

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>(); // Cola de lineas

    private Queue<string> pageQueue = new Queue<string>();
    public int maxCharactersPerPage = 120; // ajustable

    private bool isDialogueActive = false;

    // Typewriterz
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentLine;

    void Awake()
    {
        Instance = this;
    }

    // 👉 Iniciar diálogo
    public void StartDialogue(DialogueData dialogue)
    {
        linesQueue.Clear();

        foreach (DialogueLine line in dialogue.lines)
        {
            linesQueue.Enqueue(line);
        }

        dialogueUI.Show();

        isDialogueActive = true;
        ShowNextLine();
    }

    // Llamado desde el click/tap del panel
    public void OnUserNext()
    {
        if (!isDialogueActive) return;

        ShowNextLine();
    }

    // Mostrar siguiente línea
    void ShowNextLine()
    {
        // 👉 Si está escribiendo → completar texto
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueUI.SetText(currentLine);
            isTyping = false;
            return;
        }

        // 👉 Si hay páginas pendientes → mostrar siguiente
        if (pageQueue.Count > 0)
        {
            currentLine = pageQueue.Dequeue();
            typingCoroutine = StartCoroutine(TypeLine(currentLine));
            return;
        }

        // 👉 Si no hay más líneas → terminar
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // 👉 Nueva línea → dividir en páginas
        DialogueLine line = linesQueue.Dequeue();

        List<string> pages = SplitTextIntoPages(line.text);

        pageQueue = new Queue<string>(pages);

        currentLine = pageQueue.Dequeue();
        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }

    // Efecto máquina de escribir
    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        dialogueUI.SetText("");

        foreach (char c in text)
        {
            dialogueUI.SetText(dialogueUI.dialogueText.text + c);
            yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
    }

    // Finalizar diálogo
    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueUI.Hide();
    }

    List<string> SplitTextIntoPages(string text)
    {
        List<string> pages = new List<string>();

        string[] words = text.Split(' ');
        string currentPage = "";

        foreach (string word in words)
        {
            // +1 por el espacio
            if (currentPage.Length + word.Length + 1 > maxCharactersPerPage)
            {
                pages.Add(currentPage.Trim());
                currentPage = "";
            }

            currentPage += word + " ";
        }

        if (!string.IsNullOrWhiteSpace(currentPage))
        {
            pages.Add(currentPage.Trim());
        }

        return pages;
    }
}