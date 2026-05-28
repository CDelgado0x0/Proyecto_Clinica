using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public TMP_Text dialogueText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private AudioSource audioSource;

    private StringBuilder sb = new StringBuilder();

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        sb.Clear();
        sb.Append(text);
        dialogueText.text = text;
    }

    public void AppendChar(char c)
    {
        sb.Append(c);
        dialogueText.text = sb.ToString();
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void PlayVoice(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
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
        DialogueManager.Instance.OnUserNext();
    }
}