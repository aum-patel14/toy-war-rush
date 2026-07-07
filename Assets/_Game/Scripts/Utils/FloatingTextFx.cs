// TOY WAR RUSH - FloatingTextFx.cs
// World-space floating labels for gates and evolution.

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FloatingTextFx : MonoBehaviour
{
    public static FloatingTextFx Instance { get; private set; }

    [SerializeField] private float lifetime = 0.9f;
    [SerializeField] private float riseSpeed = 2.2f;
    [SerializeField] private int poolSize = 12;

    private readonly Queue<TextMeshPro> _pool = new();
    private Transform _poolRoot;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        _poolRoot = new GameObject("FloatingTextPool").transform;
        _poolRoot.SetParent(transform);
        for (int i = 0; i < poolSize; i++)
        {
            var tmp = CreatePooledText();
            tmp.gameObject.SetActive(false);
            _pool.Enqueue(tmp);
        }
    }

    public void Spawn(string text, Vector3 worldPosition, Color color)
    {
        var tmp = GetFromPool();
        tmp.text = text;
        tmp.color = color;
        tmp.transform.position = worldPosition + Vector3.up * 1.2f;
        tmp.transform.rotation = Quaternion.LookRotation(Camera.main != null
            ? Camera.main.transform.forward
            : Vector3.forward);
        tmp.gameObject.SetActive(true);
        StartCoroutine(AnimateAndReturn(tmp));
    }

    private IEnumerator AnimateAndReturn(TextMeshPro tmp)
    {
        float elapsed = 0f;
        Vector3 start = tmp.transform.position;
        Vector3 startScale = tmp.transform.localScale;
        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;
            tmp.transform.position = start + Vector3.up * (riseSpeed * elapsed);
            tmp.transform.localScale = startScale * (1f + t * 0.35f);
            var c = tmp.color;
            c.a = 1f - t;
            tmp.color = c;
            yield return null;
        }
        ReturnToPool(tmp);
    }

    private TextMeshPro GetFromPool()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();

        return CreatePooledText();
    }

    private TextMeshPro CreatePooledText()
    {
        var go = new GameObject("FloatText");
        go.transform.SetParent(_poolRoot);
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.fontSize = 4.5f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.rectTransform.sizeDelta = new Vector2(4f, 1.5f);
        go.transform.localScale = Vector3.one * 0.35f;
        return tmp;
    }

    private void ReturnToPool(TextMeshPro tmp)
    {
        tmp.gameObject.SetActive(false);
        var c = tmp.color;
        c.a = 1f;
        tmp.color = c;
        _pool.Enqueue(tmp);
    }
}
