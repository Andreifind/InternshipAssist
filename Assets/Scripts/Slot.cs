using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int Layer;
    public bool IsHoldingItem = false;
    private Collider2D _slotCollider;

    private void Awake()
    {
        _slotCollider = GetComponent<Collider2D>();
        if (IsHoldingItem)
        {
            ItemType item = GetComponentInChildren<ItemType>();
            if (item != null)
            {
                GetComponentInParent<Shelf>().CheckAndDestroyItems();
                item.ChangeColorTint(Layer);
                item.ChangeOrderInLayer(Layer);
            }
        }
        if (IsHoldingItem && Layer>0)
        {
            this.GetComponentInChildren<Collider2D>().enabled = false;
        }
        else if (Layer == 0)
            _slotCollider.enabled = !IsHoldingItem;
        if (Layer != 0)
            _slotCollider.enabled = false;
    }

    
    private void Update()
    {
        
    }

    public void HoldTheItem(bool IsHoldingIt)
    {
        Debug.Log("Holding item to " + IsHoldingIt);
        IsHoldingItem = IsHoldingIt;
        if (Layer == 0)
            _slotCollider.enabled = !IsHoldingItem;
        if(IsHoldingItem)
        {
            ItemType item = GetComponentInChildren<ItemType>();
            if (item != null)
            {
                GetComponentInParent<Shelf>().CheckAndDestroyItems();
                item.ChangeColorTint(Layer);
                item.ChangeOrderInLayer(Layer);
                if (Layer != 0)
                    GetComponentInChildren<Collider2D>().enabled = false;
            }
        }
        if (Layer != 0)
            _slotCollider.enabled = false;
    }
}
