using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType : MonoBehaviour
{
    public int Type; 
    public List<Sprite> Sprites;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _itemCollider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _itemCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        _spriteRenderer.sprite = Sprites[Type];
        if (this.GetComponentInParent<Slot>().Layer > 0)
            _itemCollider.enabled = false;
    }

    private void Update()
    {

    }

    public void ChangeColorTint(int layer)
    {
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is not initialized!");
            return;
        }

        float tintValue;
        if (layer == 0) tintValue = 0;
        else if (layer == 1) tintValue = 0.6f;
        else if (layer == 2) tintValue = 1;
        else tintValue = 0;

        _spriteRenderer.color = Color.Lerp(Color.white, Color.black, tintValue);

        if (layer == 0)
            _itemCollider.enabled = true;
    }


    public void ChangeOrderInLayer(int layer)
    {
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is not initialized!");
            return;
        }
        
        if (layer == 0) _spriteRenderer.sortingOrder = 6;
        else if (layer == 1) _spriteRenderer.sortingOrder = 5;
        else if (layer == 2) _spriteRenderer.sortingOrder = 4;
        else _spriteRenderer.sortingOrder = 0;
    }


}
