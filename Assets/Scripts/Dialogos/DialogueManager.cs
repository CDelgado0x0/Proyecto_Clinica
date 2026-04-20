using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public DialogueUI dialogueUI;

    public AudioSource audioSource;

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>(); // Cola de lineas

    private Queue<string> pageQueue = new Queue<string>();
    public int maxCharactersPerPage = 120; // Editar en el Inspector

    private DialogueData currentDialogue;

    private bool isDialogueActive = false;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentLine;

    void Awake()
    {
        Instance = this;
    }

    // Iniciar dialogo
    public void StartDialogue(DialogueData dialogue)
    {
        linesQueue.Clear();

        foreach (DialogueLine line in dialogue.lines)
        {
            linesQueue.Enqueue(line); // Añade cada linea a la cola
        }
        currentDialogue = dialogue;

        dialogueUI.Show(); // Mostrar panel

        isDialogueActive = true;
        ShowNextLine();
    }

    // Llamado desde el click/tap del panel
    public void OnUserNext()
    {
        if (!isDialogueActive) return;

        //----------------------------------------------------------------------
        /*
        if (audioSource.isPlaying)          //Bloquea avanzar si el audio sigue sonando
        return;*/
        //----------------------------------------------------------------------

        ShowNextLine();
    }

    // Mostrar siguiente línea
    void ShowNextLine()
    {
        // Si el texto aun se esta escribiendo, mostrar completo al clickar
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueUI.SetText(currentLine);
            isTyping = false;
            return;
        }

        // Si hay paginas pendientes muestra la siguiente
        if (pageQueue.Count > 0)
        {
            currentLine = pageQueue.Dequeue();
            typingCoroutine = StartCoroutine(TypeLine(currentLine));
            return;
        }

        // Si no hay mas lineas termina
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Dividir la nueva linea
        DialogueLine line = linesQueue.Dequeue();

        string nameToShow = line.characterName; // Nombre del personaje

        if (string.IsNullOrEmpty(nameToShow))
        {
            nameToShow = currentDialogue.defaultCharacterName;
        }

        dialogueUI.SetName(nameToShow);

        if (line.voiceClip != null)
        {
            audioSource.Stop();
            audioSource.clip = line.voiceClip;
            audioSource.Play();
        }

        List<string> pages = SplitTextIntoPages(line.text);

        pageQueue = new Queue<string>(pages);

        currentLine = pageQueue.Dequeue();
        typingCoroutine = StartCoroutine(TypeLine(currentLine)); // Empezar efecto maquina de escribir
    }

    // Efecto maquina de escribir
    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        dialogueUI.SetText("");

        foreach (char c in text)
        {
            dialogueUI.SetText(dialogueUI.dialogueText.text + c);
            yield return new WaitForSeconds(0.03f); // Tiempo de espera entre cada letra
        }

        isTyping = false;
    }

    // Finalizar dialogo
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