// TOY WAR RUSH - ShopUI.cs
// Shop screen for IAP and coin purchases.

using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;

    public void OnPurchaseCoins(int itemIndex)
    {
        // Wire to ShopItemData array in inspector
    }

    public void OnPurchaseNoAds()
    {
        shopManager?.PurchaseWithIAP("remove_ads");
    }

    public void OnClose() => gameObject.SetActive(false);
}
