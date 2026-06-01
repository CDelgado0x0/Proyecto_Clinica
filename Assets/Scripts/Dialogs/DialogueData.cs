using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string defaultCharacterName;
    public DialogueLine[] lines; // Lista de Frases
}

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    [TextArea(3,10)]
    public string text;
    public AudioClip voiceClip;
}