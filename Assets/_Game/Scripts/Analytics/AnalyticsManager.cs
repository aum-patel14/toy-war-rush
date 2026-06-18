// TOY WAR RUSH - AnalyticsManager.cs
// Firebase Analytics stub — wire up when Firebase SDK is added.

using UnityEngine;
using System.Collections.Generic;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        int level = LevelManager.Instance?.CurrentLevel ?? SaveManager.Instance?.Data.currentLevel ?? 0;
        Debug.Log($"[Analytics] {eventName} (level={level})");
        // TODO: FirebaseAnalytics.LogEvent(eventName, parameters)
    }

    public void LogLevelStart(int level) =>
        LogEvent("level_start", new Dictionary<string, object> { { "level", level } });

    public void LogLevelComplete(int level) =>
        LogEvent("level_complete", new Dictionary<string, object> { { "level", level } });

    public void LogLevelFail(int level) =>
        LogEvent("level_fail", new Dictionary<string, object> { { "level", level } });
}
