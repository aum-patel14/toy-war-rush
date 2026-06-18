// TOY WAR RUSH - AdPlacementConfig.cs
// Ad unit IDs and placement rules.

using UnityEngine;

[CreateAssetMenu(menuName = "ToyWarRush/AdPlacementConfig", fileName = "AdPlacementConfig")]
public class AdPlacementConfig : ScriptableObject
{
    [Header("AdMob App")]
    public string appId = "ca-app-pub-3940256099942544~3347511713";

    [Header("Test Unit IDs (replace before release)")]
    public string bannerUnitId = "ca-app-pub-3940256099942544/6300978111";
    public string interstitialUnitId = "ca-app-pub-3940256099942544/1033173712";
    public string rewardedUnitId = "ca-app-pub-3940256099942544/5224354917";

    [Header("Placement Rules")]
    public int interstitialEveryNLevels = 3;
    public bool adsRemoved;
}
