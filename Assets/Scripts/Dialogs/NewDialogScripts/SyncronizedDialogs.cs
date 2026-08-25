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

        yield return StartCoroutine(TypeSyncedToAudio(phase.text, animationDuration));

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

    private IEnumerator TypeSyncedToAudio(string text, float audioDuration)
    {
        dialogueText.text = "";
        Canvas.ForceUpdateCanvases();

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
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
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
