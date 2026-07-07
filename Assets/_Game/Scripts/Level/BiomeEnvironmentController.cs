// TOY WAR RUSH - BiomeEnvironmentController.cs
// Applies desert / cave / outdoor palettes per level (Mob Control parity).

using UnityEngine;

public class BiomeEnvironmentController : MonoBehaviour
{
    [SerializeField] private Light sunLight;
    [SerializeField] private Camera mainCamera;

    private Renderer _laneGround;
    private Renderer _sandLeft;
    private Renderer _sandRight;
    private Material _laneMatInstance;
    private Material _sandMatInstance;

    private void Awake()
    {
        var ground = GameObject.Find("Ground");
        if (ground != null) _laneGround = ground.GetComponent<Renderer>();
        var sandL = GameObject.Find("SandLeft");
        if (sandL != null) _sandLeft = sandL.GetComponent<Renderer>();
        var sandR = GameObject.Find("SandRight");
        if (sandR != null) _sandRight = sandR.GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<int>(GameEvents.LevelLoaded, OnLevelLoaded);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<int>(GameEvents.LevelLoaded, OnLevelLoaded);
    }

    private void OnLevelLoaded(int levelNumber)
    {
        ApplyBiome(levelNumber);
    }

    public void ApplyBiome(int levelNumber)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (levelNumber <= 10)
            ApplyDesert();
        else if (levelNumber <= 20)
            ApplyCave();
        else
            ApplyOutdoor();
    }

    private void ApplyDesert()
    {
        if (mainCamera != null)
            mainCamera.backgroundColor = new Color(0.49f, 0.78f, 0.89f);
        RenderSettings.fog = false;
        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.55f);
        TintLane(new Color(0.36f, 0.38f, 0.4f), new Color(0.86f, 0.73f, 0.52f));
        if (sunLight != null)
        {
            sunLight.color = new Color(1f, 0.96f, 0.88f);
            sunLight.intensity = 1.2f;
        }
    }

    private void ApplyCave()
    {
        if (mainCamera != null)
            mainCamera.backgroundColor = new Color(0.05f, 0.04f, 0.1f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.08f, 0.06f, 0.12f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 15f;
        RenderSettings.fogEndDistance = 75f;
        RenderSettings.ambientLight = new Color(0.22f, 0.18f, 0.32f);
        if (sunLight != null)
        {
            sunLight.color = new Color(0.62f, 0.48f, 0.98f);
            sunLight.intensity = 0.55f;
        }
        TintLane(new Color(0.1f, 0.18f, 0.38f), new Color(0.08f, 0.06f, 0.12f));
    }

    private void ApplyOutdoor()
    {
        if (mainCamera != null)
            mainCamera.backgroundColor = new Color(0.55f, 0.82f, 0.98f);
        RenderSettings.fog = false;
        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.55f);
        if (sunLight != null)
        {
            sunLight.color = new Color(1f, 1f, 0.92f);
            sunLight.intensity = 1.35f;
        }
        TintLane(new Color(0.32f, 0.55f, 0.38f), new Color(0.45f, 0.72f, 0.42f));
    }

    private void TintLane(Color lane, Color sides)
    {
        if (_laneGround != null)
        {
            _laneMatInstance ??= _laneGround.material;
            _laneMatInstance.color = lane;
        }
        if (_sandLeft != null)
        {
            _sandMatInstance ??= _sandLeft.material;
            _sandMatInstance.color = sides;
        }
        if (_sandRight != null && _sandMatInstance != null)
            _sandRight.material.color = sides;
    }

    private void OnDestroy()
    {
        if (_laneMatInstance != null) Destroy(_laneMatInstance);
        if (_sandMatInstance != null) Destroy(_sandMatInstance);
    }
}
