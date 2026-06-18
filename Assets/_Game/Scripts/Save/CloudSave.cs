// TOY WAR RUSH - CloudSave.cs
// Unity Gaming Services cloud save stub — implement when UGS is configured.

using UnityEngine;
using System;

public class CloudSave : MonoBehaviour
{
    public static CloudSave Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PushSave(GameSaveData data, Action<bool> onComplete = null)
    {
        // TODO: Unity Gaming Services Cloud Save integration
        Debug.Log("[CloudSave] Push deferred — UGS not configured.");
        onComplete?.Invoke(false);
    }

    public void PullSave(Action<GameSaveData> onComplete)
    {
        // TODO: Pull from cloud, compare timestamps, load newer version
        Debug.Log("[CloudSave] Pull deferred — UGS not configured.");
        onComplete?.Invoke(null);
    }
}
