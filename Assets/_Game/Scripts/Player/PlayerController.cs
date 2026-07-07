// TOY WAR RUSH - PlayerController.cs
// Handles swipe input and moves the army formation left/right.

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float xClamp = 3.5f;
    [SerializeField] private float swipeSensitivity = 0.05f;

    private float _targetX;
    private float _lastTouchX;
    private bool _isDragging;
    private bool _isPlaying;
    private float _defaultForwardSpeed;

    private void Awake()
    {
        _defaultForwardSpeed = forwardSpeed;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        _isPlaying = state == GameState.Playing;
    }

    private void Update()
    {
        if (!_isPlaying) return;

        HandleInput();
        MoveForward();
        MoveHorizontal();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _lastTouchX = touch.position.x;
                    _isDragging = true;
                    break;
                case TouchPhase.Moved when _isDragging:
                {
                    float delta = (touch.position.x - _lastTouchX) * swipeSensitivity;
                    _targetX = Mathf.Clamp(_targetX + delta, -xClamp, xClamp);
                    _lastTouchX = touch.position.x;
                    break;
                }
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    _isDragging = false;
                    break;
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            float delta = Input.GetAxis("Mouse X") * swipeSensitivity * 10f;
            _targetX = Mathf.Clamp(_targetX + delta, -xClamp, xClamp);
        }
#endif
    }

    private void MoveForward()
    {
        transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;
    }

    private void MoveHorizontal()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, _targetX, moveSpeed * Time.deltaTime);
        transform.position = pos;
    }

    public void SetForwardSpeed(float speed) => forwardSpeed = speed;
    public void ResetForwardSpeed() => forwardSpeed = _defaultForwardSpeed;
    public void ResetPosition() => _targetX = transform.position.x;
}
