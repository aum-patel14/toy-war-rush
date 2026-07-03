// TOY WAR RUSH - CameraFollow.cs
// Smooth runner camera behind the player formation.

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new(0f, 12f, -10f);
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float lookAhead = 6f;
    [SerializeField] private float defaultFov = 52f;
    [SerializeField] private float runFov = 56f;
    [SerializeField] private float battleFov = 61f;
    [SerializeField] private float fovSmooth = 5f;

    private Vector3 _velocity;
    private float _shake;
    private float _shakeX;
    private float _shakeY;
    private Camera _cam;

    private void Awake()
    {
        Instance = this;
        _cam = GetComponent<Camera>();
        if (_cam != null && defaultFov <= 0f)
            defaultFov = _cam.fieldOfView;
    }

    public void SetTarget(Transform t) => target = t;

    public void Shake(float intensity)
    {
        _shake = Mathf.Max(_shake, intensity);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        if (_shake > 0.05f)
        {
            _shake *= 0.85f;
            _shakeX = (Mathf.PerlinNoise(Time.time * 40f, 0f) - 0.5f) * _shake;
            _shakeY = (Mathf.PerlinNoise(0f, Time.time * 40f) - 0.5f) * _shake * 0.5f;
        }
        else
        {
            _shakeX = _shakeY = 0f;
        }

        var desired = target.position + offset + Vector3.forward * lookAhead;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Victory)
            desired += new Vector3(0f, 1.4f, -1.2f);
        desired += new Vector3(_shakeX, _shakeY, 0f);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);

        var lookAt = target.position + Vector3.forward * lookAhead;
        transform.LookAt(lookAt);

        if (_cam != null)
        {
            float targetFov = defaultFov;
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                float playerZ = target.position.z;
                float endZ = LevelManager.Instance?.LevelEndZ ?? (playerZ + 200f);
                float fortressBlend = Mathf.Clamp01((playerZ - (endZ - 30f)) / 30f);
                targetFov = Mathf.Lerp(runFov, battleFov, fortressBlend);
            }
            else if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Defeat)
            {
                targetFov = defaultFov - 3f;
            }
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFov, Time.deltaTime * fovSmooth);
        }
    }
}
