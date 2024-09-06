using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType : MonoBehaviour
{
    public int Type; 
    public List<Sprite> Sprites;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Sprites[Type];
    }

    private void Update()
    {

    }

    public void ChangeColorTint(int layer)
    {
        float tintValue;
        if (layer == 0) 
            tintValue = 0;
        else if (layer == 1) 
            tintValue = 0.6f;
        else 
            tintValue = 1;
        // tintValue is 0 (white) and 1 (black)
        tintValue = Mathf.Clamp01(tintValue);

        _spriteRenderer.color = Color.Lerp(Color.white, Color.black, tintValue);
    }

    public void ChangeOrderInLayer(int layer)
    {
        if(layer == 0)
            _spriteRenderer.sortingOrder = 6;
        else if (layer == 1)
            _spriteRenderer.sortingOrder = 5;
        else
            _spriteRenderer.sortingOrder = 4;
    }
}
