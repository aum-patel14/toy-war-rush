// TOY WAR RUSH - UpgradeMenuBuyButton.cs
// Wires upgrade panel buy buttons to UpgradeMenuUI by index.

using UnityEngine;

public class UpgradeMenuBuyButton : MonoBehaviour
{
    [SerializeField] private UpgradeMenuUI menu;
    [SerializeField] private int upgradeIndex;

    public void Configure(UpgradeMenuUI target, int index)
    {
        menu = target;
        upgradeIndex = index;
    }

    public void Purchase() => menu?.PurchaseUpgrade(upgradeIndex);
}
