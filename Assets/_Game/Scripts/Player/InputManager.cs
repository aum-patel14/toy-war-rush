// TOY WAR RUSH - InputManager.cs
// Centralized touch input with swipe detection.

using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private float swipeThreshold = 50f;

    public event Action<Vector2> OnSwipe;
    public event Action<Vector2> OnDrag;

    private Vector2 _touchStart;
    private bool _isTouching;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (Input.touchCount == 0)
        {
#if UNITY_EDITOR
            HandleMouseInput();
#endif
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _touchStart = touch.position;
                _isTouching = true;
                break;
            case TouchPhase.Moved when _isTouching:
                OnDrag?.Invoke(touch.position - _touchStart);
                break;
            case TouchPhase.Ended when _isTouching:
            {
                Vector2 delta = touch.position - _touchStart;
                if (delta.magnitude >= swipeThreshold)
                    OnSwipe?.Invoke(delta.normalized);
                _isTouching = false;
                break;
            }
        }
    }

#if UNITY_EDITOR
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _touchStart = Input.mousePosition;
            _isTouching = true;
        }
        else if (Input.GetMouseButton(0) && _isTouching)
        {
            OnDrag?.Invoke((Vector2)Input.mousePosition - _touchStart);
        }
        else if (Input.GetMouseButtonUp(0) && _isTouching)
        {
            Vector2 delta = (Vector2)Input.mousePosition - _touchStart;
            if (delta.magnitude >= swipeThreshold)
                OnSwipe?.Invoke(delta.normalized);
            _isTouching = false;
        }
    }
#endif
}
