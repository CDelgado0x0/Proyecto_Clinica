using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // Asignar este codigo a cada NPC que tenga dialogo.

    public DialogueData dialogue;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo ha entrado en el trigger");

        if (other.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("Es el player!");
            hasTriggered = true;

            //Llama al Manager para iniciar el dialogo
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }
}