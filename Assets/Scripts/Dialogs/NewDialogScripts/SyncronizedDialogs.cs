using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ConsultaPhase
{
    public string text;
    public AudioClip audio;
    public int animatorPhaseIndex;
}

public class SyncronizedDialogs : MonoBehaviour
{
    [Header("Fases")]
    [SerializeField] private ConsultaPhase[] phases;

    [Header("Referencias")]
    [SerializeField] private InGameUIInteraction UIManager;
    [SerializeField] private Animator doctorAnimator;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private AudioSource audioSource;

    private int currentPhase = 0;

    private void Start()
    {
        StartCoroutine(RunPhase(currentPhase));
    }

    private IEnumerator RunPhase(int index)
    {
        if (index >= phases.Length) yield break;

        ConsultaPhase phase = phases[index];

        // Activa la animación correspondiente
        doctorAnimator.SetInteger("phaseIndex", phase.animatorPhaseIndex);

        // Muestra el texto con máquina de escribir sincronizada al audio
        audioSource.clip = phase.audio;
        audioSource.Play();

        yield return StartCoroutine(TypeSyncedToAudio(phase.text, phase.audio.length));

        // Espera a que el audio termine por si el texto acabó antes
        while (audioSource.isPlaying)
            yield return null;

        // Fundido a negro, cambia de fase, fundido inverso
        bool phaseChanged = false;
        BrightnessOverlay.Instance.FadeToBlack(1f, () => phaseChanged = true);

        yield return new WaitUntil(() => phaseChanged);

        currentPhase++;

        if (currentPhase < phases.Length)
        {
            BrightnessOverlay.Instance.FadeFromBlack(1f);
            yield return StartCoroutine(RunPhase(currentPhase));
        }
        else
        {
            // Todas las fases terminadas, vuelve al menú
            UIManager.OnNextButton();
        }
    }

    private IEnumerator TypeSyncedToAudio(string text, float audioDuration)
    {
        dialogueText.text = "";

        if (audioDuration <= 0 || text.Length == 0) yield break;

        float charsPerSecond = text.Length / audioDuration;
        float elapsed = 0f;
        int charsShown = 0;

        while (charsShown < text.Length)
        {
            elapsed += Time.deltaTime;
            int target = Mathf.Min(Mathf.FloorToInt(elapsed * charsPerSecond), text.Length);

            if (target > charsShown)
            {
                charsShown = target;
                dialogueText.text = text.Substring(0, charsShown);
            }

            yield return null;
        }

        dialogueText.text = text;
    }
}
