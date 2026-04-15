using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueUI : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI dialogueText;
    private DialogueManager manager;

    void Start()
    {
        gameObject.SetActive(false); // Ocultar panel al inicio
        manager = DialogueManager.Instance;
    }

    public void SetText(string text)
    {
        dialogueText.text = text;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.OnUserNext();
    }
}