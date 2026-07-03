// TOY WAR RUSH - AdManager.cs
// AdMob wrapper stub — replace with Google Mobile Ads SDK.

using UnityEngine;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [SerializeField] private AdPlacementConfig config;

    private int _levelsCompletedSinceInterstitial;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAds();
    }

    private void InitializeAds()
    {
        // TODO: MobileAds.Initialize(config.appId)
        Debug.Log("[AdManager] AdMob stub initialized.");
    }

    public void ShowBanner()
    {
        if (config != null && config.adsRemoved) return;
        Debug.Log("[AdManager] Show banner (stub).");
    }

    public void HideBanner() => Debug.Log("[AdManager] Hide banner (stub).");

    public void ShowRewardedAd(Action<bool> callback, string placement = "default")
    {
        if (config != null && config.adsRemoved)
        {
            callback?.Invoke(true);
            return;
        }

        Debug.Log($"[AdManager] Show rewarded ad (stub): {placement}");
        AnalyticsManager.Instance?.LogEvent("ad_rewarded_watch", new System.Collections.Generic.Dictionary<string, object>
        {
            { "placement", placement },
            { "result", "stub_success" }
        });
        callback?.Invoke(true);
    }

    public void ShowRewardedAd(Action<bool> callback) => ShowRewardedAd(callback, "default");

    public void PurchaseRemoveAds()
    {
        if (config == null) return;
        config.adsRemoved = true;
        HideBanner();
        Debug.Log("[AdManager] Remove ads IAP applied (stub).");
        AnalyticsManager.Instance?.LogEvent("iap_remove_ads", null);
    }

    public void ShowRewardedContinue(Action<bool> callback) =>
        ShowRewardedAd(callback, "continue");

    public void ShowInterstitial(Action onComplete = null)
    {
        if (config != null && config.adsRemoved)
        {
            onComplete?.Invoke();
            return;
        }
        Debug.Log("[AdManager] Show interstitial (stub).");
        onComplete?.Invoke();
    }

    public void ShowContinueOffer()
    {
        // UI handles the actual offer via ResultScreenUI
    }

    public void OnLevelComplete()
    {
        _levelsCompletedSinceInterstitial++;
        if (config != null && _levelsCompletedSinceInterstitial >= config.interstitialEveryNLevels)
        {
            ShowInterstitial();
            _levelsCompletedSinceInterstitial = 0;
        }
    }
}
