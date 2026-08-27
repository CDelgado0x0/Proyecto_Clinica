using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIInteraction : MonoBehaviour, IPointerClickHandler
{
    [Header("Escenas")]

    [Space(10)]

    [SerializeField] private string previousScene;
    [SerializeField] private string currentScene;
    [SerializeField] private string nextScene;

    [Header("DialogPanel")]

    [Space(10)]

    [SerializeField] private GameObject dialogPanel;
    [SerializeField] public TMP_Text dialogueText;


    [Header("RegularButtons")]

    [Space(10)]

    [SerializeField] private Button returnButton;
    [SerializeField] private Button nextButton;

    [Header("SAAC")]

    [Space(10)]

    [SerializeField] private Button sadButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button happyButton;

    [Header("Scene time")]

    [Space(10)]

    [SerializeField] private float sceneTime;

    [Header("ExitToMainMenuButton")]

    [Space(10)]

    [SerializeField] private Button exitToMainButton;


    private StringBuilder sb = new StringBuilder();
    private float timer;
    private bool timerActive = false;


    private void Start()
    {
        BrightnessOverlay.Instance.FadeFromBlack(1f);

        SubscribeListeners();

        if(currentScene == "WaitingRoom")
        {
            sceneTime = SettingsManager.Instance.GetSceneDuration();
        }

        timer = sceneTime;
        timerActive = (currentScene != "Consultation");
    }

    private void Update()
    {
        if (!timerActive) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            OnNextButton();
        }
    }

    private void SubscribeListeners()
    {
        returnButton.onClick.AddListener(OnReturnButton);
        nextButton.onClick.AddListener(OnNextButton);
        sadButton.onClick.AddListener(OnSadButton);
        normalButton.onClick.AddListener(OnNormalButton);
        happyButton.onClick.AddListener(OnHappyButton);
        exitToMainButton.onClick.AddListener(OnExitToMainButton);
    }

    private void OnEnable()
    {
        SettingsManager.OnFontSizeChanged += UpdateFontSize;
        UpdateFontSize(SettingsManager.Instance.CurrentSettings.dialogFontSize);
    }

    private void OnDisable()
    {
        SettingsManager.OnFontSizeChanged -= UpdateFontSize;
    }

    private void UpdateFontSize(float value)
    {
        dialogueText.fontSize = value;
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

    public void Show()
    {
        dialogPanel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        dialogPanel.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DialogueManager.Instance.OnUserNext();
    }





    //A partir de aqui comienzan las interacciones de los botones de la funcion SuscribeListeners

    private void OnReturnButton()
    {
        SceneManager.LoadScene(previousScene);
    }

    public void OnNextButton()
    {
        timerActive = false;

        if (nextScene == "MainMenu")
        {
            MetricsManager.Instance.EndGameSession(completed: true);
        }

        BrightnessOverlay.Instance.FadeToBlack(1f, () =>
        {
            SceneManager.LoadScene(nextScene);
        });
    }

    private void OnSadButton()
    {
        MetricsManager.Instance.RegisterEvent("El jugador se siente mal");
    }

    private void OnNormalButton()
    {
        MetricsManager.Instance.RegisterEvent("El jugador se siente tranquilo");
    }

    private void OnHappyButton()
    {
        MetricsManager.Instance.RegisterEvent("El jugador se siente alegre");
    }

    private void OnExitToMainButton()
    {
        MetricsManager.Instance.EndGameSession(completed: false, currentScene);
        SceneManager.LoadScene("MainMenu");
    }
}
