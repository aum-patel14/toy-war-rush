// TOY WAR RUSH - FortressController.cs

// End-of-level fortress battle — visible clash with army vs defenders.



using UnityEngine;

using System.Collections;



public class FortressController : MonoBehaviour

{

    [SerializeField] private int maxHP = 100;

    [SerializeField] private float battleDuration = 3.5f;

    [SerializeField] private Transform battleZone;



    private int _currentHP;

    private int _defenderPower;

    private bool _battleStarted;



    public int DefenderPowerRemaining => _defenderPower;



    public void Initialize(int hp)

    {

        maxHP = hp;

        _currentHP = hp;

        _defenderPower = 0;

        _battleStarted = false;

    }



    private void OnTriggerEnter(Collider other)

    {

        if (_battleStarted) return;

        if (!other.CompareTag("Player")) return;



        _battleStarted = true;

        StartCoroutine(ResolveBattle());

    }



    private IEnumerator ResolveBattle()

    {

        yield return new WaitForSeconds(0.15f);



        var levelData = LevelManager.Instance?.CurrentLevelData;

        int blue = ArmyManager.Instance?.ArmyCount ?? 0;

        _defenderPower = levelData?.fortressDefenderCount ?? Mathf.Max(10, maxHP / 5);

        if (levelData?.hasBoss == true)

            _defenderPower = Mathf.RoundToInt(_defenderPower * 1.5f);



        var player = GameObject.FindGameObjectWithTag("Player");

        var playerCtrl = player != null ? player.GetComponent<PlayerController>() : null;

        if (playerCtrl != null)

            playerCtrl.SetForwardSpeed(1.2f);



        Vector3 clashPoint = battleZone != null ? battleZone.position : transform.position;

        float elapsed = 0f;



        while (elapsed < battleDuration && blue > 0 && _defenderPower > 0)

        {

            int rate = Mathf.Max(1, (blue + _defenderPower) / 18);

            int blueLoss = Mathf.Min(rate, blue);

            int redLoss = Mathf.Min(rate, _defenderPower);



            blue -= blueLoss;

            _defenderPower -= redLoss;



            ArmyManager.Instance?.RemoveUnits(blueLoss);

            EnemyArmyManager.Instance?.RemoveEnemies(redLoss);



            FXManager.Instance?.PlayEffect("FortressHit", clashPoint + Random.insideUnitSphere * 0.8f);

            CameraFollow.Instance?.Shake(0.35f);

            EventBus.Publish(GameEvents.FortressBattleTick, new FortressBattleState(blue, _defenderPower, maxHP));



            yield return new WaitForSeconds(0.07f);

            elapsed += 0.07f;

            blue = ArmyManager.Instance?.ArmyCount ?? 0;

        }



        if (playerCtrl != null)

            playerCtrl.ResetForwardSpeed();



        AudioManager.Instance?.PlaySFX("fortress_explode");



        if (blue > _defenderPower || blue > 0)

        {

            EventBus.Publish(GameEvents.FortressDestroyed);

            Destroy(gameObject, 0.8f);

        }

        else

        {

            ArmyManager.Instance?.ClearArmy();

            GameManager.Instance?.SetState(GameState.Defeat);

        }

    }

}



public readonly struct FortressBattleState

{

    public readonly int BluePower;

    public readonly int RedPower;

    public readonly int FortressHp;



    public FortressBattleState(int blue, int red, int fortressHp)

    {

        BluePower = blue;

        RedPower = red;

        FortressHp = fortressHp;

    }

}

