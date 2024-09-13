using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType : MonoBehaviour
{
    public int Type; 
    public List<Sprite> Sprites;
    public GameObject ExplosionEffect;
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

        Color targetColor;
        if (layer == 0) targetColor = Color.white; // No tint, fully visible
        else if (layer == 1) targetColor = Color.Lerp(Color.white, Color.black, 0.6f); // Slightly tinted black
        else if (layer == 2) targetColor = new Color(1f, 1f, 1f, 0f); // Fully transparent
        else targetColor = Color.white; // Default to white if layer is outside expected range

        _spriteRenderer.color = targetColor;

        if (layer == 0)
            _itemCollider.enabled = true;
        else
            _itemCollider.enabled = false; // Disable collider for other layers
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

    public void ApplyExplosion()
    {
        Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
    }

}
