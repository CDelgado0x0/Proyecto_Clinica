using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class PauseInterval
{
    public float start;
    public float duration;
}

[System.Serializable]
public class ConsultaPhase
{
    public string text;
    public AudioClip audio;
    public int animatorPhaseIndex;
    public PauseInterval[] pauses;
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
    [SerializeField] private ScrollRect scrollRect;

    private bool phaseChanged;

    private int currentPhase = 0;

    private void Start()
    {
        StartCoroutine(RunPhase(currentPhase));
    }

    private IEnumerator RunPhase(int index)
    {
        if (index >= phases.Length) yield break;

        ConsultaPhase phase = phases[index];

        phaseChanged = false;

        // Activa la animación correspondiente
        doctorAnimator.SetInteger("phaseIndex", phase.animatorPhaseIndex);

        // Espera un frame para que el Animator procese el cambio
        yield return null;

        BrightnessOverlay.Instance.FadeFromBlack(1f);

        // Obtiene la duración real de la animación actual
        float animationDuration = doctorAnimator.GetCurrentAnimatorStateInfo(0).length;

        // Muestra el texto con máquina de escribir sincronizada al audio
        audioSource.clip = phase.audio;
        audioSource.Play();

        yield return StartCoroutine(TypeSyncedToAudio(phase.text, animationDuration, phase.pauses));

        // Espera a que la animación termine completamente
        yield return StartCoroutine(WaitForAnimationComplete());
        
        BrightnessOverlay.Instance.FadeToBlack(1f, () => phaseChanged = true);

        yield return new WaitUntil(() => phaseChanged);

        currentPhase++;

        if (currentPhase < phases.Length)
        {
            
            yield return StartCoroutine(RunPhase(currentPhase));
        }
        else
        {
            // Todas las fases terminadas, vuelve al menú
            UIManager.OnNextButton();
        }
    }
    private float GetSpeakingElapsed(float rawTime, PauseInterval[] pauses)
    {
        if (pauses == null) return rawTime;

        float subtract = 0f;
        foreach (PauseInterval p in pauses)
        {
            if (rawTime > p.start)
            {
                float overlapEnd = Mathf.Min(rawTime, p.start + p.duration);
                subtract += overlapEnd - p.start;
            }
        }
        return rawTime - subtract;
    }

    private IEnumerator TypeSyncedToAudio(string text, float audioDuration, PauseInterval[] pauses)
    {
        dialogueText.text = "";
        Canvas.ForceUpdateCanvases();

        if (audioDuration <= 0 || text.Length == 0) yield break;

        float totalPauseDuration = 0f;
        if (pauses != null)
        {
            foreach (PauseInterval p in pauses) totalPauseDuration += p.duration;
        }
        float totalSpeakingDuration = Mathf.Max(0.01f, audioDuration - totalPauseDuration);

        int charsShown = 0;

        while (charsShown < text.Length)
        {
            float rawTime = audioSource.isPlaying ? audioSource.time : audioDuration;
            float speakingElapsed = GetSpeakingElapsed(rawTime, pauses);

            int target = Mathf.Min(
                Mathf.FloorToInt((speakingElapsed / totalSpeakingDuration) * text.Length),
                text.Length
            );

            if (target > charsShown)
            {
                charsShown = target;
                dialogueText.text = text.Substring(0, charsShown);
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }

            if (!audioSource.isPlaying && rawTime >= audioDuration)
            {
                charsShown = text.Length;
                dialogueText.text = text;
                break;
            }

            yield return null;
        }

        dialogueText.text = text.Substring(0, charsShown);
    }

    private IEnumerator WaitForAnimationComplete()
    {
        yield return null;

        while (doctorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }
}
