// TOY WAR RUSH - ResultScreenUI.cs

// Victory and defeat screens.



using UnityEngine;

using UnityEngine.SceneManagement;

using TMPro;

using System.Collections;



public class ResultScreenUI : MonoBehaviour

{

    [SerializeField] private GameObject victoryPanel;

    [SerializeField] private GameObject defeatPanel;

    [SerializeField] private TextMeshProUGUI coinsRewardText;

    [SerializeField] private TextMeshProUGUI starsText;

    [SerializeField] private string mainMenuScene = "MainMenu";

    [SerializeField] private float coinCountDuration = 1.1f;



    private Coroutine _coinAnimRoutine;



    private void OnDisable()

    {

        if (_coinAnimRoutine != null)

        {

            StopCoroutine(_coinAnimRoutine);

            _coinAnimRoutine = null;

        }

    }



    public void ShowVictory()

    {

        victoryPanel?.SetActive(true);

        defeatPanel?.SetActive(false);



        var levelData = LevelManager.Instance?.CurrentLevelData;

        int reward = levelData?.coinsReward ?? 0;



        if (_coinAnimRoutine != null)

            StopCoroutine(_coinAnimRoutine);

        _coinAnimRoutine = StartCoroutine(AnimateCoinReward(reward));



        if (starsText != null)

        {

            int stars = LevelManager.Instance?.CalculateStars() ?? 1;

            starsText.text = new string('⭐', stars);

        }



        AudioManager.Instance?.PlaySFX("victory_fanfare");

    }



    private IEnumerator AnimateCoinReward(int target)

    {

        if (coinsRewardText == null)

        {

            _coinAnimRoutine = null;

            yield break;

        }



        float elapsed = 0f;

        while (elapsed < coinCountDuration)

        {

            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / coinCountDuration);

            int shown = Mathf.RoundToInt(target * t);

            coinsRewardText.text = $"+{shown}";

            yield return null;

        }



        coinsRewardText.text = $"+{target}";

        _coinAnimRoutine = null;

    }



    public void ShowDefeat()

    {

        victoryPanel?.SetActive(false);

        defeatPanel?.SetActive(true);

    }



    public void OnNextLevel()

    {

        LevelManager.Instance?.LoadNextLevel();

        gameObject.SetActive(false);

    }



    public void OnRetry()

    {

        LevelManager.Instance?.RetryLevel();

        gameObject.SetActive(false);

    }



    public void OnMainMenu()

    {

        SceneManager.LoadScene(mainMenuScene);

    }



    public void OnWatchAdDoubleCoins()

    {

        AdManager.Instance?.ShowRewardedAd(success =>

        {

            if (success)

                RewardSystem.Instance?.GrantDoubleCoinsReward();

        });

    }



    public void OnWatchAdContinue()

    {

        AdManager.Instance?.ShowRewardedContinue(success =>

        {

            if (success)

            {

                RewardSystem.Instance?.GrantContinueReward();

                gameObject.SetActive(false);

            }

        });

    }

}

