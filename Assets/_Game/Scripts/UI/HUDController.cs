// TOY WAR RUSH - HUDController.cs
// In-game HUD: army count, tier, level, coins, progress, fortress bar, brave banner.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI armyCountText;
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Image progressFill;
    [SerializeField] private GameObject fortressBarRoot;
    [SerializeField] private Image fortressBarFill;
    [SerializeField] private TextMeshProUGUI fortressBarLabel;
    [SerializeField] private GameObject braveBannerRoot;
    [SerializeField] private TextMeshProUGUI braveBannerText;
    [SerializeField] private Color normalArmyColor = new(0.13f, 0.45f, 0.85f);
    [SerializeField] private Color dangerArmyColor = new(0.9f, 0.2f, 0.2f);

    private Transform _player;
    private Vector3 _armyTextScale = Vector3.one;

    private void OnEnable()
    {
        ArmyManager.OnArmyCountChanged += UpdateArmyCount;
        ArmyManager.OnEvolutionTriggered += UpdateTier;
        EventBus.Subscribe<FortressBattleState>(GameEvents.FortressBattleTick, OnFortressBattleTick);
        EventBus.Subscribe<int>(GameEvents.LevelLoaded, OnLevelLoaded);
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCoinsChanged += UpdateCoins;
        UpdateLevel();
        UpdateCoins(CurrencyManager.Instance?.Coins ?? 0);
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetFortressBarVisible(false);
        UpdateBraveBanner(LevelManager.Instance?.CurrentLevel ?? 1);
    }

    private void OnDisable()
    {
        ArmyManager.OnArmyCountChanged -= UpdateArmyCount;
        ArmyManager.OnEvolutionTriggered -= UpdateTier;
        EventBus.Unsubscribe<FortressBattleState>(GameEvents.FortressBattleTick, OnFortressBattleTick);
        EventBus.Unsubscribe<int>(GameEvents.LevelLoaded, OnLevelLoaded);
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCoinsChanged -= UpdateCoins;
    }

    private void OnLevelLoaded(int level) => UpdateBraveBanner(level);

    private void UpdateBraveBanner(int level)
    {
        if (braveBannerRoot != null)
            braveBannerRoot.SetActive(true);
        if (braveBannerText == null) return;
        if (level >= 11 && level <= 20)
            braveBannerText.text = "BE BRAVE!";
        else if (level > 20)
            braveBannerText.text = "CHARGE!";
        else
            braveBannerText.text = "MULTIPLY & RUSH!";
    }

    private void UpdateArmyCount(int count)
    {
        if (armyCountText == null) return;
        armyCountText.text = count.ToString();
        armyCountText.color = count <= 8 ? dangerArmyColor : normalArmyColor;
        armyCountText.transform.localScale = _armyTextScale * 1.16f;
    }

    private void UpdateTier(UnitTier tier)
    {
        if (tierText != null)
            tierText.text = tier.ToString().ToUpper();
    }

    private void UpdateLevel()
    {
        if (levelText != null)
            levelText.text = $"Level {LevelManager.Instance?.CurrentLevel ?? 1}";
    }

    private void UpdateCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }

    private void OnFortressBattleTick(FortressBattleState state)
    {
        SetFortressBarVisible(true);
        if (fortressBarFill != null)
        {
            float denom = Mathf.Max(1, state.FortressHp);
            fortressBarFill.fillAmount = Mathf.Clamp01((float)state.BluePower / denom);
        }
        if (fortressBarLabel != null)
            fortressBarLabel.text = $"⚔ {state.BluePower} vs 🏰 {state.RedPower}";
    }

    private void SetFortressBarVisible(bool visible)
    {
        if (fortressBarRoot != null)
            fortressBarRoot.SetActive(visible);
    }

    private void Update()
    {
        if (armyCountText != null)
            armyCountText.transform.localScale = Vector3.Lerp(armyCountText.transform.localScale, _armyTextScale, Time.deltaTime * 10f);

        if (progressFill == null || _player == null || LevelManager.Instance == null) return;
        float start = LevelManager.Instance.PlayerStartZ;
        float end = LevelManager.Instance.LevelEndZ;
        if (end <= start) return;
        progressFill.fillAmount = Mathf.Clamp01((_player.position.z - start) / (end - start));

        if (fortressBarRoot != null && fortressBarRoot.activeSelf)
        {
            float playerZ = _player.position.z;
            float fortressZ = LevelManager.Instance.LevelEndZ - 5f;
            if (playerZ < fortressZ - 25f)
                SetFortressBarVisible(false);
        }
    }

    public void OnPausePressed() => GameManager.Instance?.SetState(GameState.Paused);
}
