using System;
using System.IO;
using UnityEngine;

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager Instance { get; private set; }

    public MetricsData Current { get; private set; }
    public string MetricsFilePath { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

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

    public void SaveMetrics()
    {
        File.WriteAllText(MetricsFilePath, JsonUtility.ToJson(Current, prettyPrint: true));
    }
}
