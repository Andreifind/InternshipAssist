using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupObject : MonoBehaviour
{
    public int Type;
    public List<Sprite> Sprites;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    public float FloatSpeed = 1f;
    public float VanishDuration = 2f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (Sprites != null && Sprites.Count > 0)
        {
            Type = Random.Range(0, Sprites.Count);
            _spriteRenderer.sprite = Sprites[Type];
            _originalColor = _spriteRenderer.color;
        }
        else
        {
            Debug.LogError("Sprites list is empty or null.");
        }
        StartCoroutine(FloatAndFadeOut());
    }

    private IEnumerator FloatAndFadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < VanishDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / VanishDuration);

            _spriteRenderer.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, alpha);

            transform.Translate(Vector3.up * FloatSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _spriteRenderer.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0f);

        Destroy(gameObject);
    }

    private void Update()
    {

    }
}
