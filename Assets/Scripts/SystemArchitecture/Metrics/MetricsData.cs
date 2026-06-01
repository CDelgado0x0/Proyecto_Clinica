using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MetricsData
{
    public string username;
    public string sessionStart;
    public List<MetricEvent> events = new List<MetricEvent>();
    public List<string> sessionSummaries = new List<string>();
}

[System.Serializable]
public class MetricEvent
{
    public string description;
    public string timestamp;
}