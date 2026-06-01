using System;
using System.IO;
using UnityEngine;

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager Instance { get; private set; }

    public MetricsData Current { get; private set; }
    public string MetricsFilePath { get; private set; }

    private string startScene;
    private float sessionStartTime;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (Instance != null) return; // ya existe, no hace nada

            GameObject obj = new GameObject("MetricsManager_Auto");
            obj.AddComponent<MetricsManager>();
        }
    #endif

    public void StartSession(string pseudonym)
    {
        Current = new MetricsData
        {
            username = pseudonym,
            sessionStart = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        };

        // Ruta y archivo a guardar
        string fileName = $"{pseudonym}_{Current.sessionStart}.json";
        MetricsFilePath = Path.Combine(SettingsManager.Instance.MetricsPath, fileName);

        SaveMetrics();
    }

    public void StartGameSession(string sceneName)
    {
        startScene = sceneName;
        sessionStartTime = Time.time;
    }

    public void EndGameSession(bool completed, string currentScene = "")
    {
        float totalTime = Time.time - sessionStartTime;
        int waitTime = (int)(SettingsManager.Instance.GetSceneDuration() / 60f);
        string timeFormatted = System.TimeSpan.FromSeconds(totalTime).ToString(@"mm\:ss");

        string summary;

        if (completed)
        {
            summary = startScene == "Reception"
                ? $"El jugador ha elegido el modo completo con un tiempo de espera de {waitTime} minutos y ha completado la aplicación en {timeFormatted}"
                : $"El jugador ha elegido jugar desde {startScene} con un tiempo de espera de {waitTime} minutos y ha completado la aplicación en {timeFormatted}";
        }
        else
        {
            summary = $"El jugador ha abandonado la partida en {currentScene} " +
                      $"habiendo empezado desde {startScene} " +
                      $"con un tiempo de espera de {waitTime} minutos " +
                      $"y habiendo tardado {timeFormatted} antes de salir";
        }

        Current.sessionSummaries.Add(summary);
        SaveMetrics();
    }

    public void RegisterAgitation(float value)
    {
        RegisterEvent($"Agitacion detectada: {value:F2} grados/frame");
    }

    public void RegisterEvent(string description)
    {
        MetricEvent newEvent = new MetricEvent
        {
            description = description,
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        Current.events.Add(newEvent);
        SaveMetrics();
    }

    public void SaveMetrics()
    {
        File.WriteAllText(MetricsFilePath, JsonUtility.ToJson(Current, prettyPrint: true));
    }
}
