using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject selectScenarioPanel;
    public GameObject settingsPanel;

    public SceneLoader sceneLoader;

    public UISequentialAppear scenarioButtons;

    // Abrir seleccion de escenario
    public void OpenScenarioSelection()
    {
        mainMenuPanel.SetActive(false);
        selectScenarioPanel.SetActive(true);

        scenarioButtons.PlayAnimation();
    }

    // Abrir ajustes
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Volver al menú principal
    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        selectScenarioPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    // Empezar simulacion completa
    public void StartSimulation()
    {   
        SimulationManager.fullSimulation = true;
        sceneLoader.LoadScene(2); // Recepcion
    }

    // Salir del juego
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        sceneLoader.QuitGame();
    }
}