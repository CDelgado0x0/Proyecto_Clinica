using UnityEngine;

public class LlamadaDePrueba : MonoBehaviour
{
    [SerializeField] DialogueData textoPrueba;
    
    private bool hasBeenCalled = false;
    public void OnDialogStart()
    {
        if (hasBeenCalled) return; // Evita llamadas múltiples
        DialogueManager.Instance.StartDialogue(textoPrueba);
    }
}
