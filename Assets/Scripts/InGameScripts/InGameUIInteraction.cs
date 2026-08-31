using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIInteraction : MonoBehaviour
{
    [Header("Escenas")]

    [Space(10)]

    [SerializeField] private string previousScene;
    [SerializeField] private string currentScene;
    [SerializeField] private string nextScene;

    [Header("DialogPanel")]

    [Space(10)]

    [SerializeField] private TMP_Text dialogueText;


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

    [Header("Camera Input Button")]

    [Space(10)]

    [SerializeField] private Image cameraInputImage;
    [SerializeField] private Sprite touchModeSprite;
    [SerializeField] private Sprite gyroModeSprite;
    [SerializeField] private Button cameraInputButton;

    private float timer;
    private bool timerActive = false;


    private void Start()
    {
        BrightnessOverlay.Instance.FadeFromBlack(1f);

        SubscribeListeners();

        

        if (currentScene == "WaitingRoom")
        {
            sceneTime = SettingsManager.Instance.GetSceneDuration();
            timer = sceneTime;
            timerActive = true;
        }
        else if (currentScene == "Consultation")
        {
            AudioManager.Instance.SetClinicalAmbientVolume();
            timerActive = false;
        }
        else
        {
            timer = sceneTime;
            timerActive = true;
        }
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
        cameraInputButton.onClick.AddListener(OnCameraInputButton);
    }

    private void OnEnable()
    {
        SettingsManager.OnFontSizeChanged += UpdateFontSize;
        UpdateFontSize(SettingsManager.Instance.CurrentSettings.dialogFontSize);
        SettingsManager.OnControlModeChanged += UpdateCameraInputImage;
        UpdateCameraInputImage(SettingsManager.Instance.CurrentSettings.controlMode);
    }

    private void OnDisable()
    {
        SettingsManager.OnFontSizeChanged -= UpdateFontSize;
        SettingsManager.OnControlModeChanged -= UpdateCameraInputImage;
    }

    private void UpdateFontSize(float value)
    {
        if (dialogueText != null)
        {
            dialogueText.fontSize = value;
        }
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
            AudioManager.Instance.SetNormalAmbientVolume();
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
        AudioManager.Instance.SetNormalAmbientVolume();
        MetricsManager.Instance.EndGameSession(completed: false, currentScene);
        SceneManager.LoadScene("MainMenu");
    }
    private void OnCameraInputButton()
    {
        SettingsManager.Instance.ToggleControlMode();
    }

    private void UpdateCameraInputImage(int mode)
    {
        cameraInputImage.sprite = mode == 0 ? touchModeSprite : gyroModeSprite;
    }
}
