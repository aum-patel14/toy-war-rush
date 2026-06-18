// TOY WAR RUSH - UIManager.cs
// Central UI controller for all screens.

using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private HUDController hud;
    [SerializeField] private MainMenuUI mainMenu;
    [SerializeField] private ResultScreenUI resultScreen;
    [SerializeField] private UpgradeMenuUI upgradeMenu;
    [SerializeField] private ShopUI shopUI;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        hud?.gameObject.SetActive(state == GameState.Playing);
        mainMenu?.gameObject.SetActive(state == GameState.MainMenu);
        resultScreen?.gameObject.SetActive(state == GameState.Victory || state == GameState.Defeat);
        upgradeMenu?.gameObject.SetActive(false);
        shopUI?.gameObject.SetActive(false);

        if (state == GameState.Victory)
            resultScreen?.ShowVictory();
        else if (state == GameState.Defeat)
            resultScreen?.ShowDefeat();
    }

    public void ShowUpgradeMenu() => upgradeMenu?.gameObject.SetActive(true);
    public void ShowShop() => shopUI?.gameObject.SetActive(true);
}
