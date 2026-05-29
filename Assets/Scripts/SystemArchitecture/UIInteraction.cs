using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class UIInteraction : MonoBehaviour
{
    [Header("LogInPanel")]

    [Space(10)]

    [SerializeField] private GameObject logInPanel;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button quitButton;

    [Header("MainMenuPanel")]

    [Space(10)]

    [SerializeField] private GameObject menuPanel;

    [Space(10)]

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button selectSceneButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button logOutButton;


    [Header("SettingsPanel")]

    [Space(10)]

    [SerializeField] private GameObject settingsPanel;

    [Space(10)]

    [SerializeField] private Slider ambientVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Space(10)]

    [SerializeField] private Slider dialogFontSizeSlider;
    [SerializeField] private TMP_Text sampleText;

    [Space(10)]

    [SerializeField] private Slider brightnessSlider;

    [Space(10)]

    [SerializeField] private Button[] sceneDurationButtons;
    [SerializeField] private Sprite[] activeSprites;
    [SerializeField] private Sprite[] inactiveSprites;

    [Space(10)]

    [SerializeField] private TMP_Text metricsPathText;
    [SerializeField] private Button displayAllPathPanel;
    [SerializeField] private TMP_Text completePathText;
    [SerializeField] private Button showMetricsPathButton;
    [SerializeField] private Button saveMetricsButton;

    [Space(10)]

    [SerializeField] private Button settingsBackButton;

    [Header("SelectScenePanel")]

    [Space(10)]

    [SerializeField] private GameObject selectScenePanel;

    [Space(10)]

    [SerializeField] private Button selectSceneBackButton;

    private void Start()
    {
        LoadCurrentValues();
        SubscribeListeners();
        SetupSceneDurationButtons();
        ShowMetricsPath();
    }

    private void LoadCurrentValues()
    {
        SettingsData s = SettingsManager.Instance.CurrentSettings;

        //Cargar los valores del settings data almacenado
        ambientVolumeSlider.value = s.ambientVolume;
        sfxVolumeSlider.value = s.sfxVolume;
        dialogFontSizeSlider.value = s.dialogFontSize;
        sampleText.fontSize = s.dialogFontSize;
        brightnessSlider.value = s.brightness;
        UpdateSceneDurationButtons(s.sceneDuration);

        //Al volver al menu principal, si hay un JSON de datos creados no aparecerá de nuevo el panel de log in
        if (MetricsManager.Instance.Current != null)
        {
            logInPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
        errorText.gameObject.SetActive(false);
    }

    private void SubscribeListeners()
    {
        ambientVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetAmbientVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSfxVolume);
        brightnessSlider.onValueChanged.AddListener(SettingsManager.Instance.SetBrightness);
        dialogFontSizeSlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SetDialogFontSize(value);
            sampleText.fontSize = value;
        });

        settingsBackButton.onClick.AddListener(OnSettingsBackButton);
        settingsButton.onClick.AddListener(OnSettingsButton);
        selectSceneButton.onClick.AddListener(OnSelectSceneButton);
        selectSceneBackButton.onClick.AddListener(OnSelectSceneBackButton);
        playButton.onClick.AddListener(OnPlayButton);
        quitButton.onClick.AddListener(OnQuitButton);
        saveMetricsButton.onClick.AddListener(OnExportButton);
        logOutButton.onClick.AddListener(OnLogOutButton);
        showMetricsPathButton.onClick.AddListener(ShowMetricsButton);
        displayAllPathPanel.onClick.AddListener(HideMetricsButton);
        acceptButton.onClick.AddListener(OnAcceptButton);
    }

    private void SetupSceneDurationButtons()
    {
        for (int i = 0; i < sceneDurationButtons.Length; i++)
        {
            int index = i;
            sceneDurationButtons[i].onClick.AddListener(() =>
            {
                SettingsManager.Instance.SetSceneDuration(index);
                UpdateSceneDurationButtons(index);
            });
        }
    }

    private void ShowMetricsPath() //Se llama en el start
    {
        string path = SettingsManager.Instance.MetricsPath;
        metricsPathText.text = path;
        completePathText.text = path;
        displayAllPathPanel.gameObject.SetActive(false);
    }

    private void UpdateSceneDurationButtons(int activeIndex)
    {
        for (int i = 0; i < sceneDurationButtons.Length; i++)
        {
            Image buttonImage = sceneDurationButtons[i].GetComponent<Image>();
            buttonImage.sprite = (i == activeIndex) ? activeSprites[i] : inactiveSprites[i];
        }
    }

    private void OnSettingsButton()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void OnSettingsBackButton()
    {
        SettingsManager.Instance.SaveSettings();
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    private void OnSelectSceneButton()
    {
        menuPanel.SetActive(false);
        selectScenePanel.SetActive(true);
    }

    private void OnSelectSceneBackButton()
    {
        menuPanel.SetActive(true);
        selectScenePanel.SetActive(false);
    }

    private void OnLogOutButton()
    {
        menuPanel.SetActive(false);
        logInPanel.SetActive(true);
    }

    private void OnPlayButton()
    {
        SceneManager.LoadScene("Reception");
    }

    private void OnQuitButton()
    {
        Application.Quit();
    }

    private void OnAcceptButton()
    {
        string pseudonym = usernameInput.text.Trim();

        if (string.IsNullOrEmpty(pseudonym))
        {
            errorText.text = "INTRODUCE UN PSEUDONIMO PARA CONTINUAR.";
            errorText.gameObject.SetActive(true);
            return;
        }

        MetricsManager.Instance.StartSession(pseudonym);
        logInPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void OnExportButton()
    {
        string path = MetricsManager.Instance.MetricsFilePath;

        if (!File.Exists(path))
        {
            Debug.LogWarning("No hay métricas guardadas todavía.");
            return;
        }

        new NativeShare()
            .SetSubject("Métricas del juego")
            .AddFile(path)
            .Share();
    }

    private void ShowMetricsButton()
    {
        displayAllPathPanel.gameObject.SetActive(true);
    }

    private void HideMetricsButton()
    {
        displayAllPathPanel.gameObject.SetActive(false);
    }
}
